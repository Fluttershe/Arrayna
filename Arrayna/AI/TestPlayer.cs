using UnityEngine;
using System.Collections;

public class TestPlayer : MonoBehaviour
{
    //可编辑血量
    public int HealthP;

    //血量
    public static int HP;

    //速度
    public int speed;

    void Awake()
    {
        HP = HealthP;
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
        //获取鼠标的坐标，鼠标是屏幕坐标，Z轴为0，这里不做转换  
        Vector3 mouse = Input.mousePosition;
        //当目标向量的Y轴大于等于0时候  
        if (mouse.x >= 0)
        {
            transform.localScale = new Vector3(1, 1,1);
        }
        else if (mouse.x < 0)
        {
            transform.localScale = new Vector3(-1, 1,1);
        }

        //死亡
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
}
