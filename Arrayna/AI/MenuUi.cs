using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuUi : MonoBehaviour
{
    public Text code;
    public Text mission1;
    public Text mission2;
    public Text mission3;
    public Image star;

    void Update()
    {
        //显示任务
        switch (Menu.level)
        {
            case 1:
                code.text = "代号：本能";
                mission1.text = "任务1：坚持10秒\n(解锁新关卡)";
                mission2.text = "任务2：击杀3个敌人\n(解锁新零件)";
                mission3.text = "任务3：与敌人距离不小于2米\n(解锁新零件)";
                break;
        }

        //显示评分
        if (Menu.roomNum[0, Menu.level] )
        {
            if (Menu.roomNum[1, Menu.level])
            {
                if (Menu.roomNum[2, Menu.level])
                {
                    if (Menu.roomNum[3, Menu.level])
                    {
                        star.fillAmount = 1f;
                        mission1.color = Color.yellow;
                        mission2.color = Color.yellow;
                        mission3.color = Color.yellow;
                    }
                    star.fillAmount = 0.666f;
                    mission1.color = Color.yellow;
                    mission2.color = Color.yellow;
                    mission3.color = Color.black;
                }
                else if (Menu.roomNum[3, Menu.level])
                {
                    if (Menu.roomNum[2, Menu.level])
                    {
                        star.fillAmount = 1f;
                        mission1.color = Color.yellow;
                        mission2.color = Color.yellow;
                        mission3.color = Color.yellow;
                    }
                    star.fillAmount = 0.666f;
                    mission1.color = Color.yellow;
                    mission2.color = Color.black;
                    mission3.color = Color.yellow;
                }
                else
                {
                    star.fillAmount = 0.333f;
                    mission1.color = Color.yellow;
                    mission2.color = Color.black;
                    mission3.color = Color.black;
                }
            }
            else
            {
                star.fillAmount = 0f;
                mission1.color = Color.black;
                mission2.color = Color.black;
                mission3.color = Color.black;
            }
        }
    }

    public void OnStartGame()
    {
        SceneManager.LoadScene("WeaponAssemblageWIP");
    }
}
