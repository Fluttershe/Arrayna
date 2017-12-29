using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneAI : MonoBehaviour
{
    public int speed;
    public GameObject Player;

    public bool jump;

    private void Update()
    {
        jump = false;
        transform.Translate(new Vector2(-1, 0) * speed * Time.deltaTime);

        //发射射线
        var hit = Physics2D.Raycast(transform.position, -transform.right*6);
        Debug.DrawLine(transform.position, hit.point);
        //检测如果是玩家，靠近

        if(hit.transform!=null)
        {
            if (hit.collider?.gameObject.tag == "Player")
            {
                transform.Translate(new Vector2(Player.transform.position.x - transform.position.x, 0) * speed * Time.deltaTime);
                jump = false;
            }
            else
            {
                jump = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "jump")
        {
            if (transform.position.y < Player.transform.position.y)
            {
                transform.Translate(new Vector2(-1, 30) * speed * Time.deltaTime);
            }
            if (jump == true)
            {
                transform.Translate(new Vector2(-1, 30) * speed * Time.deltaTime);
            }
        }
    }
}
