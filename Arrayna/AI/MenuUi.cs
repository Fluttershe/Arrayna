using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuUi : MonoBehaviour
{
    public Text code;
    public Text mission;
    public Image star;

    void Update()
    {
        //显示任务
        switch (Menu.level)
        {
            case 1:
                code.text = "代号：本能";
                mission.text = "任务1：坚持10秒\n(解锁新关卡)\n任务2：击杀3个敌人\n(解锁新零件)\n任务3：与敌人距离不小于2米\n(解锁新零件)";                              
                break;
        }

        //显示评分
        if (Menu.roomNum[0, Menu.level] && Menu.roomNum[1, Menu.level])
        {
            star.fillAmount = 0.333f;
        }
        else if (Menu.roomNum[0, Menu.level] && Menu.roomNum[1, Menu.level] && Menu.roomNum[2, Menu.level])
        {
            star.fillAmount = 0.666f;
        }
        else if (Menu.roomNum[0, Menu.level] && Menu.roomNum[1, Menu.level] && Menu.roomNum[3, Menu.level])
        {
            star.fillAmount = 0.666f;
        }
        else if (Menu.roomNum[0, Menu.level] && Menu.roomNum[1, Menu.level] && Menu.roomNum[2, Menu.level] && Menu.roomNum[3, Menu.level])
        {
            star.fillAmount = 1f;
        }
        else
        {
            star.fillAmount = 0f;
        }
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene("WeaponAssemblageWIP");
    }
}
