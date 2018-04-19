using UnityEngine;
using System;
using System.Collections;

public class Menu : MonoBehaviour
{
    //总房间数
    public int num;

    //房间信息
    public static bool[,] roomNum;
    //房间编号
    public static int level;

    public static float[] balabala;

    void Awake()
    {
        //禁止删除
        GameObject.DontDestroyOnLoad(gameObject);
        //更新房间信息
        roomNum = new bool[4,num+1];
        balabala = new float[num + 1];
        //解锁第一个房间
        roomNum[0,1] = true;
        level = 1;
    }

    void Update()
    {
        //解锁
        if (!roomNum[0, 2])
        {
            if (roomNum[1, 1])
            {
                roomNum[0, 2] = true;
                roomNum[0, 3] = true;
                roomNum[0, 4] = true;
                roomNum[0, 5] = true;
            }
        }
    }
}
