using UnityEngine;
using System.Collections;

public class CreatPlayer : MonoBehaviour
{
    //生成玩家
    public GameObject Player;
    //生成地形


    //游戏时间
    public static int time;
    //击杀目标
    public int kill;
    //击杀数量
    public static int killNum;
    //目标开关
    public static bool anquanjuli;

    //欢呼声
    public AudioSource huanhu;
    //胜利声
    public AudioClip shengli;
    //过关开关
    public static bool win;

    void Start()
    {
        KaiShi();
        anquanjuli = false;
        win = false;
        Instantiate(Player, new Vector3(0, 0, -2), Quaternion.identity);
    }

    void KaiShi()
    {
        if (Menu.level == 1)
        {
            time = 10;
            kill = 3;
            Invoke("GameOver", time);
        }
        InvokeRepeating("TimeDown", 0, 1);
    }

    void TimeDown()
    {
        if (time > 0)
        {
            time -= 1;
        }
    }

    void GameOver()
    {
        huanhu.Play();
        AudioSource.PlayClipAtPoint(shengli, transform.position);
        if (Menu.level == 1)
        {
            Menu.roomNum[1, Menu.level] = true;
            if (killNum >= kill)
            {
                Menu.roomNum[2, Menu.level] = true;
            }
            if (!anquanjuli)
            {
                Menu.roomNum[3, Menu.level] = true;
            }
        }
        win = true;
    }
}
