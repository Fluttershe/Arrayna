using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour
{
    public int roomNum;

    public Animator bianse;

    void Start()
    {
        if (Menu.roomNum[0,roomNum])
        {
            bianse.SetBool("bianse",true);
        }
    }

    void Update()
    {
        if (Menu.level!=roomNum)
        {
            bianse.SetBool("xuanzhong", false);
        }
        else
        {
            bianse.SetBool("xuanzhong", true);
        }
    }

    void OnMouseDown()
    {
        if (Menu.roomNum[0, roomNum])
        {
            if (Input.GetMouseButton(0))
            {
                Menu.level = roomNum;
            }
        }
    }
}
