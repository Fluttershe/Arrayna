using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using WeaponAssemblage;

public class TestPlayer : MonoBehaviour
{
    //可编辑血量
	[SerializeField]
    int HealthP;

    //血量
    public static int HP;

	//速度
	[SerializeField]
	int speed;

	[SerializeField]
	public float weightMultiplier;
	
	[SerializeField]
	MonoWeapon weapon;

    void Awake()
    {
        HP = HealthP;

        weapon = PlayerWeaponStorage.TakeWeapon(0);
    }

    void FixedUpdate()
    {
		Vector2 moveVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		if (moveVector.sqrMagnitude > 1) moveVector.Normalize();
		moveVector *= speed * Time.deltaTime;
		moveVector *= 1 - Mathf.Clamp01(weightMultiplier * weapon.FinalValue[WpnAttrType.Weight]);
		print(Mathf.Clamp01(weightMultiplier * weapon.FinalValue[WpnAttrType.Weight]));
		print(moveVector);

		//移动
        transform.Translate(moveVector);

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
	}
}
