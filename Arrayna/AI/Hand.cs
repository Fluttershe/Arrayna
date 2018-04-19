using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using WeaponAssemblage;

public class Hand : MonoBehaviour
{
    public MonoWeapon weapon;

    public AudioClip reload;
    public Animator reloading;
    bool huandan;

    float fen;
    float zong;

    public Text code;
    public Text ammoNum;

    public Text kill;
    public Text gameTime;

    void Awake()
    {
        huandan = false;
        weapon = PlayerWeaponStorage.TakeWeapon(0);
        weapon.transform.SetParent(transform);
        weapon.transform.localPosition = new Vector3(-0.5f,0.8f,-3);
        weapon.transform.localRotation = Quaternion.Euler(0, 0, 90);
		var ang = weapon.transform.rotation * weapon.transform.rotation;
        code.text = "ID:" + Random.Range(0, 10) + Random.Range(0, 10) +  Random.Range(0, 10) + Random.Range(0, 10) + Random.Range(0, 10) + Random.Range(0, 10) + Random.Range(0, 10);
    }

    void Update()
    {
        kill.text = "杀敌数：" + CreatPlayer.killNum;
        gameTime.text = "挑战时间：" + CreatPlayer.time;

        if (!TestPlayer.kaiguan)
        {
            //获取鼠标的坐标，鼠标是屏幕坐标，Z轴为0，这里不做转换  
            Vector3 mouse = Input.mousePosition;
            //获取物体坐标，物体坐标是世界坐标，将其转换成屏幕坐标，和鼠标一直  
            Vector3 obj = Camera.main.WorldToScreenPoint(transform.position);
            //屏幕坐标向量相减，得到指向鼠标点的目标向量，即黄色线段  
            Vector3 direction = mouse - obj;
            //将Z轴置0,保持在2D平面内  
            direction.z = 0f;
            //将目标向量长度变成1，即单位向量，这里的目的是只使用向量的方向，不需要长度，所以变成1  
            direction = direction.normalized;
            if (direction.y <= 2f && direction.y >= -2f)
            {
                //物体自身的Y轴和目标向量保持一直，这个过程XY轴都会变化数值  
                transform.up = direction;
            }

            if (Input.GetMouseButtonDown(0))
            {
                weapon.PrimaryFireDown();
            }
            if (Input.GetMouseButtonUp(0))
            {
                weapon.PrimaryFireUp();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                weapon.Reload();
            }
        }
        else
        {
            weapon.PrimaryFireUp();
        }

        //换弹
        if (weapon.RuntimeValues.ReloadTime>0)
        {
            if (!huandan)
            {
                AudioSource.PlayClipAtPoint(reload, transform.position);
                reloading.SetBool("huandan", true);
                huandan = true;
            }
        }
        else
        {
            reloading.SetBool("huandan", false);
            huandan = false;
        }

        //获取子弹数
        zong = weapon.FinalValue[WpnAttrType.Capacity];
        fen = weapon.FinalValue[WpnAttrType.Capacity]-weapon.RuntimeValues.ShotAmmo;
        ammoNum.text = fen + "/" + zong;
    }
}
