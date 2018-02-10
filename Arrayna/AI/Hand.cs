using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour
{
    void Update()
    {
        //获取鼠标的坐标，鼠标是屏幕坐标，Z轴为0，这里不做转换  
        Vector3 mouse = Input.mousePosition;
        //屏幕坐标向量相减，得到指向鼠标点的目标向量，即黄色线段  
        Vector2 direction = mouse - transform.position;
        //将目标向量长度变成1，即单位向量，这里的目的是只使用向量的方向，不需要长度，所以变成1  
        direction = direction.normalized;
        //物体自身的Y轴和目标向量保持一直，这个过程XY轴都会变化数值  
        transform.up = direction;

        //当目标向量的Y轴大于等于0F时候
        if (mouse.y >= 0)
        {
            transform.localPosition = new Vector3(0.5f, 0, 1);
        }
        else if (mouse.y < 0)
        {
            transform.localPosition = new Vector3(0.5f, 0, -1);
        }
    }
}
