using UnityEngine;
using System.Collections;
using WeaponAssemblage;

public class CreatPlayer : MonoBehaviour
{
    //生成玩家
    public GameObject Player;
    //生成地形
    public GameObject SceneZero;
    public GameObject SceneOne;
    public GameObject SceneTwo;

    //生成小熊和风暴
    public GameObject PinkBear;
    public GameObject FengBao;

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
        time = 0;
        killNum = 0;
        Instantiate(Player, new Vector3(0, 0, -2), Quaternion.identity);
    }

    void KaiShi()
    {
        if (Menu.level == 1)
        {
            Instantiate(SceneZero,transform.position,Quaternion.identity);
            kill = 10;
            Invoke("GameOver", 30);
        }
        if (Menu.level == 2)
        {
            Instantiate(SceneOne, transform.position, Quaternion.identity);
            kill = 30;
        }
        if (Menu.level == 3)
        {
            Instantiate(SceneTwo, transform.position, Quaternion.identity);
            kill = 40;
            Invoke("GameOver", 120);
        }
        if (Menu.level == 4)
        {
            Instantiate(SceneOne, transform.position, Quaternion.identity);
            Instantiate(PinkBear, new Vector3(10, 3, -2), Quaternion.identity);
            kill = 20;
            Invoke("GameOver", 60);
        }
        if (Menu.level == 5)
        {
            Instantiate(SceneZero, transform.position, Quaternion.identity);
            Instantiate(PinkBear, new Vector3(13, -4, -2), Quaternion.identity);
            Instantiate(FengBao, new Vector3(0, 0, -16), Quaternion.identity);
            kill = 30;
        }

        InvokeRepeating("TimeDown", 0, 1);
    }

    void TimeDown()
    {
            time += 1;
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
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A1_Si_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A1_Ba_V1")));
            }
            if (!anquanjuli)
            {
                Menu.roomNum[3, Menu.level] = true;
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("M4A1_R_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A1_M_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("M4A1_B_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("M4A1_St_V1")));
            }
        }
        if (Menu.level == 2)
        {
            Menu.roomNum[1, Menu.level] = true;
            if (time <= 60)
            {
                Menu.roomNum[2, Menu.level] = true;
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A3_Si_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A1_A_V1")));
            }
            if (time <= 35)
            {
                Menu.roomNum[3, Menu.level] = true;
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A3_R_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A3_M_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A3_B_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A1_St_V1")));
            }
        }
        if (Menu.level == 3)
        {
            Menu.roomNum[1, Menu.level] = true;
            if (killNum <= kill)
            {
                Menu.roomNum[2, Menu.level] = true;
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("AWM_Si_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A1_A_V2")));
            }
            if (!anquanjuli)
            {
                Menu.roomNum[3, Menu.level] = true;
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("AWM_R_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A4_M_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A5_M_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A1_St_V2")));
            }
        }
        if (Menu.level == 4)
        {
            Menu.roomNum[1, Menu.level] = true;
            if (killNum <= kill)
            {
                Menu.roomNum[2, Menu.level] = true;
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("7.62mm")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A2_Si_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A1_Ba_V2")));
            }
            if (anquanjuli)
            {
                Menu.roomNum[3, Menu.level] = true;
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A2_R_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A3_M_V2")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A2_B_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("AK47_St_V2")));
            }
        }
        if (Menu.level == 5)
        {
            Menu.roomNum[1, Menu.level] = true;
            if (anquanjuli)
            {
                Menu.roomNum[3, Menu.level] = true;
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A1_Ad_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A2_St_V1")));
            }
            if (time <= 30)
            {
                Menu.roomNum[3, Menu.level] = true;
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A1_R_V1")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A3_M_V3")));
                PlayerWeaponStorage.ReturnPart(Instantiate(WAPrefabStore.GetPartPrefab("A1_B_V1")));
            }
        }

        win = true;
    }

    void Update()
    {
        if (Menu.level == 2||Menu.level==5)
        {
            if (killNum >= kill)
            {
                GameOver();
            }
        }
    }
}
