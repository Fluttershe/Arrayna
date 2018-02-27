using UnityEngine;
using System.Collections;

public class dianti : MonoBehaviour
{
    //目标位置
    Transform Player;

    public int chaju;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        chaju = chaju * chaju;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 juli = Player.position - transform.position;
        float julishu = juli.sqrMagnitude;

        //距离检测
        if (julishu < chaju)
        {
            if (key.zouba)
            {
                TestCreat.level = 0;
                Application.LoadLevel(0);
            }
        }
    }
}
