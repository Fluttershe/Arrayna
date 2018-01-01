using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneAI : MonoBehaviour
{
    public int speed;
    public GameObject Player;

    private void FixedUpdate()
    {
        transform.Translate((Player.transform.position - transform.position) * speed * Time.deltaTime);
    }
}
