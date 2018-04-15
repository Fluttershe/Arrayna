using System.Collections;
using UnityEngine;
using WeaponAssemblage;

public class oneAI : MonoBehaviour
{
    //AI血量
    public float HP;

    //AI攻击力
    public int DP;

    //AI速度
    public int speed;

    //AI攻击就离
    public int gongjichaju;

    //攻击延迟
    public int gongjiyanchi;

    //目标位置
    Transform Player;

    //AI状态
    int AiSi=-1;

    //攻击时间
    int num;

    //移动速度
    int sudu;

    public Animator Enemy;

    public AudioClip siwang;

    public AudioClip gongji;

    bool kaiguan;

    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        gongjichaju = gongjichaju * gongjichaju;
        num = gongjiyanchi;
        sudu = speed;
        kaiguan = false;
    }

    void FixedUpdate()
    {
        Vector2 juli = Player.position - transform.position;
        float julishu = juli.sqrMagnitude;

        //距离检测
        if (julishu <= gongjichaju)
        {
            AiSi = 2;
        }
        else
        {
            AiSi = 1;
        }

        if (HP <= 0)
        {
            AiSi = 3;
        }

        if (julishu < 4)
        {
            CreatPlayer.anquanjuli = true;
        }

        if (CreatPlayer.win)
        {
            SiWang();
        }

        //状态检测
        switch (AiSi)
        {
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

    //追击状态
    void ZhuiJi()
    {
        num = gongjiyanchi;

        if (TestPlayer.kaiguan)
        {
            sudu = 0;
            Enemy.SetBool("attack", true);
        }
        else
        {
            sudu = speed;
            Enemy.SetBool("attack", false);
        }

        transform.position = Vector3.MoveTowards(transform.position, Player.position, sudu * Time.deltaTime);

        Vector3 player = Player.transform.position;
        //当目标向量的Y轴大于等于0时候  
        if (player.x >= transform.position.x)
        {
            transform.localScale = new Vector3(-0.6f, 0.6f, 0.6f);
        }
        else if (player.x < transform.position.x)
        {
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
    }

    //攻击状态
    void GongJi()
    {
        Enemy.SetBool("attack", true);
        sudu = 0;
        if (num > 0)
        {
            Enemy.SetBool("gongji", false);
            num--;
        }
        else
        {
            Enemy.SetBool("gongji", true);
            AudioSource.PlayClipAtPoint(gongji, transform.localPosition);
            TestPlayer.HP -= DP;
            num = gongjiyanchi;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "bullet")
        {
            HP -= collision.GetComponent<Projectile>().CriticizedDamage();
        }
    }

    //死亡状态
    void SiWang()
    {
        if (!CreatPlayer.win)
        {
            CreatAI.ranD--;
            CreatPlayer.killNum++;
        }
        Enemy.SetBool("dead", true);
        if (!kaiguan)
        {
            AudioSource.PlayClipAtPoint(siwang, transform.localPosition);
            kaiguan = true;
        }
        sudu = 0;
        InvokeRepeating("Dead", 0.5f, 0);
    }

    void Dead()
    {
        Destroy(gameObject);
    }
}
