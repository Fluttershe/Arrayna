using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using WeaponAssemblage;

public class dianti : MonoBehaviour
{
    //目标位置
    Transform Player;

    public int chaju;

    public MonoWeapon weapon;

    void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        chaju = chaju * chaju;
        weapon = PlayerWeaponStorage.TakeWeapon(0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 juli = Player.position - transform.position;
        float julishu = juli.sqrMagnitude;

        //距离检测
        if (julishu < chaju)
        {
            if (key.zouba)
            {
                TestCreat.level = 0;
                PlayerWeaponStorage.ReturnWeapon(weapon);
                SceneManager.LoadScene("WeaponAssemblageWIP");
            }
        }
    }
}
