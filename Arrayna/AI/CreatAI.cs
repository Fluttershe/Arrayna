using UnityEngine;
using System.Collections;

public class CreatAI : MonoBehaviour
{
    //随机都靠房间等级啦
    int num;

    //多少
    public static int ranD;

    //什么
    int ranS;

    //AI种类
    public GameObject oneAI;
    public GameObject twoAI;

    void Awake()
    {
        //房间等级
        if (Menu.level < 6)
        {
            num = 1;
        }

        ranD = 0;
        InvokeRepeating("SuiJiShengCheng", 5,Random.Range(1,4));
    }

    void SuiJiShengCheng()
    {
        if (!CreatPlayer.win)
        {
            if (ranD < 30)
            {
                ranS = Random.Range(num, num + 2);

                switch (ranS)
                {
                    case 1:
                        Instantiate(oneAI, transform.position, Quaternion.identity);
                        break;
                    case 2:
                        Instantiate(twoAI, transform.position, Quaternion.identity);
                        break;
                }

                ranD += 1;
            }
        }
    }
}
