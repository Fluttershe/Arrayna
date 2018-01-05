using System.Collections;
using UnityEngine;

public class oneAI : MonoBehaviour
{
    public int speed;
    public Transform Player;
    public Transform AI;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(AI.position, Player.position, speed * Time.deltaTime);
    }
}
