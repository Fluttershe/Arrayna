using System.Collections;
using UnityEngine;

public class oneAI : MonoBehaviour
{
    //AI血量
    public int HP;

    //AI攻击力
    public int DP;

    //AI速度
    public int speed;

    //AI距离
    public int chaju;

    //AI攻击就离
    public int gongjichaju;

    //攻击延迟
    public int gongjiyanchi;

    //目标位置
    Transform Player;

    //AI状态
    int AiSi=0;

    //随机方向
    int ran;

    //攻击时间
    int num ;

    void Awake()
    {
        InvokeRepeating("SuiJiYiXia", 0, Random.Range(1,4));
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        chaju = chaju * chaju;
        gongjichaju = gongjichaju * gongjichaju;
        int num = gongjiyanchi;
    }

    void FixedUpdate()
    {
        Vector2 juli = Player.position - transform.position;
        float julishu = juli.sqrMagnitude;

        //距离检测
        if (julishu > chaju)
        {
            AiSi = 0;
        }
        else if (julishu <= chaju && julishu>gongjichaju)
        {
            AiSi= 1;
        }
        else if (julishu<=gongjichaju)
        {
            AiSi = 2;
        }

        if (HP<=0)
        {
            AiSi = 3;
        }

        //状态检测
        switch (AiSi)
        {
            case 0:
            YouZou();
                break;
            case 1:
            ZhuiJi();
                break;
            case 2:
            GongJi();
                break;
            case 3:
            SiWang();
                break;
        }
    }

    //随机方向
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

    //攻击状态
    void GongJi()
    {
        if (num > 0)
        {
            num--;
        }
        else if(num<=0)
        {
            TestPlayer.HP -= DP;
            num = gongjiyanchi;
        }
    }

    //死亡状态
    void SiWang()
    {
        Destroy(gameObject);
    }
}
