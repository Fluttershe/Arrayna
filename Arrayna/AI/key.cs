using UnityEngine;
using System.Collections;

public class key : MonoBehaviour
{
    //目标位置
    Transform Player;

    public int chaju;

    public static bool zouba;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        chaju = chaju * chaju;
        zouba = false;
    }

    void Update()
    {
        Vector2 juli = Player.position - transform.position;
        float julishu = juli.sqrMagnitude;

        //距离检测
        if (julishu < chaju)
        {
            zouba = true;
            Destroy(gameObject);
        }
    }
}
