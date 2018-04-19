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
    public GameObject threeAI;
    public GameObject fourAI;

    void Awake()
    {
        //房间等级
        switch (Menu.level)
        {
            case 1:
                num = 1;
                break;
            case 2:
                num = 2;
                break;
            case 3:
                num = 1;
                break;
            case 4:
                num = 3;
                break;
            case 5:
                num = 4;
                break;
        }

        ranD = 0;
        InvokeRepeating("SuiJiShengCheng", 5,Random.Range(2,5));
    }

    void SuiJiShengCheng()
    {
        if (!CreatPlayer.win)
        {
            if (ranD < 30)
            {
                ranS = Random.Range(1,11);

                switch (num)
                {
                    case 1:
                        if (Menu.level == 1)
                        {
                            if (ranS >= 8)
                            {
                                Instantiate(oneAI, transform.position, Quaternion.identity);
                            }
                            else
                            {
                                Instantiate(twoAI, transform.position, Quaternion.identity);
                            }
                        }
                        else
                        {
                            if (ranS >= 8)
                            {
                                Instantiate(threeAI, transform.position, Quaternion.identity);
                            }
                            else
                            {
                                Instantiate(fourAI, transform.position, Quaternion.identity);
                            }
                        }
                        break;
                    case 2:
                        if (ranS >= 7)
                        {
                            Instantiate(threeAI, transform.position, Quaternion.identity);
                        }
                        else
                        {
                            Instantiate(fourAI, transform.position, Quaternion.identity);
                        }
                        break;
                    case 3:
                        if (ranS >= 6)
                        {
                            Instantiate(oneAI, transform.position, Quaternion.identity);
                        }
                        else
                        {
                            Instantiate(twoAI, transform.position, Quaternion.identity);
                        }
                        break;
                    case 4:
                        ranS = Random.Range(1,5);
                        switch (ranS)
                        {
                            case 1:
                                Instantiate(oneAI, transform.position, Quaternion.identity);
                                break;
                            case 2:
                                Instantiate(twoAI, transform.position, Quaternion.identity);
                                break;
                            case 3:
                                Instantiate(threeAI, transform.position, Quaternion.identity);
                                break;
                            case 4:
                                Instantiate(fourAI, transform.position, Quaternion.identity);
                                break;
                        }
                        break;
                }

                ranD += 1;
            }
        }
    }
}
