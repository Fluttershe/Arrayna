using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using WeaponAssemblage;

public class TestPlayer : MonoBehaviour
{
    //可编辑血量
    public int HealthP;

    //血量
    public static int HP;

    //速度
    public int speed;

	public MonoWeapon weapon;

    void Awake()
    {
        HP = HealthP;

		weapon = PlayerWeaponStorage.GetWeapon(0);
		weapon.transform.SetParent(this.transform);
		weapon.transform.localPosition = Vector3.zero;
		weapon.transform.localRotation = Quaternion.identity;
	}

    void FixedUpdate()
    {
        //移动
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }

        //转向
        Vector3 worldPos2 = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        //获取鼠标的坐标，鼠标是屏幕坐标，Z轴为0，这里不做转换  
        Vector3 mouse = Input.mousePosition;
        //当目标向量的Y轴大于等于0时候  
        if (mouse.x >= worldPos2.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (mouse.x < worldPos2.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //死亡
        if (HP <= 0)
        {
            TestCreat.level = 0;
			PlayerWeaponStorage.ReturnWeapon(weapon);
			SceneManager.LoadScene("WeaponAssemblageWIP");
        }

		if (Input.GetMouseButtonDown(0))
		{
			weapon.PrimaryFireDown();
		}

		if (Input.GetMouseButtonUp(0))
		{
			weapon.PrimaryFireUp();
		}
	}
}
