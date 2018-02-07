using UnityEngine;
using System.Collections;

public class CreatAI : MonoBehaviour
{
    //随机都靠房间等级啦
    int num;

    //多少
    int ranD=0;

    //什么
    int ranS=0;

    //AI种类
    public GameObject oneAI;
    public GameObject twoAI;
    public GameObject threeAI;

    void Awake()
    {
        num = TestCreat.level;
        ranD = Random.Range(num + 2, num + 6);
    }

    void Update()
    {
        if (ranD > 0)
        {
            SuiJiShengCheng();
            ranD--;
        }
    }

    void SuiJiShengCheng()
    {
        ranS = Random.Range(num , num + 3);

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
        }
    }
}
