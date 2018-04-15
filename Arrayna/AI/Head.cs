using UnityEngine;
using System.Collections;

public class Head : MonoBehaviour
{
    void Update()
    {
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

            if (direction.y <= 0.6f && direction.y >= -0.6f)
            {
                //物体自身的Y轴和目标向量保持一直，这个过程XY轴都会变化数值  
                transform.up = direction;
            }
        }
    }
}
