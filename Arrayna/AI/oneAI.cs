﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneAI : MonoBehaviour
{
    public int speed;
    public Transform Player;
    public Transform AI;

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(AI.position, Player.position, speed * Time.deltaTime);
    }
}
