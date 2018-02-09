using System.Collections;
using UnityEngine;

public class oneAI : MonoBehaviour
{
    //AI速度
    public int speed;

    //AI距离
    public int chaju;

    //目标位置
    Transform Player;

    //AI状态
    int AS=0;

    //随机方向
    int ran;

    void Awake()
    {
        InvokeRepeating("SuiJiYiXia", 0, Random.Range(1,4));
    }

    void Update()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector2 juli = Player.position - transform.position;
        float julishu = juli.sqrMagnitude;

        if (julishu > chaju)
        {
            AS = 0;
        }
        else if (julishu <= chaju)
        {
            AS = 1;
        }

        switch (AS)
        {
            case 0:
            YouZou();
                break;
            case 1:
            ZhuiJi();
                break;
        }
    }

    void SuiJiYiXia()
    {
        ran = Random.Range(1, 9);
    }

    //游走状态
    void YouZou()
    {
        switch (ran)
        {
            case 1:
                    transform.Translate(new Vector2(0, 1) * speed * Time.deltaTime);
                break;
            case 2:
                    transform.Translate(new Vector2(1, 1) * speed * Time.deltaTime);
                break;
            case 3:
                    transform.Translate(new Vector2(1, 0) * speed * Time.deltaTime);
                break;
            case 4:
                    transform.Translate(new Vector2(1, -1) * speed * Time.deltaTime);
                break;
            case 5:
                    transform.Translate(new Vector2(0, -1) * speed * Time.deltaTime);
                break;
            case 6:
                    transform.Translate(new Vector2(-1, -1) * speed * Time.deltaTime);
                break;
            case 7:
                    transform.Translate(new Vector2(-1, 0) * speed * Time.deltaTime);
                break;
            case 8:
                    transform.Translate(new Vector2(-1, 1) * speed * Time.deltaTime);
                break;
        }
    }

    //追击状态
    void ZhuiJi()
    {
        transform.position = Vector3.MoveTowards(transform.position, Player.position, speed * Time.deltaTime);
    }
}
