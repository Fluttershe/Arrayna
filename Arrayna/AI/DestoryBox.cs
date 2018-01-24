using UnityEngine;
using System.Collections;

public class DestoryBox : MonoBehaviour
{
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1);
        RaycastHit2D hi = Physics2D.Raycast(transform.position, -transform.right, 1);
        RaycastHit2D tih = Physics2D.Raycast(transform.position, transform.up, 1);
        RaycastHit2D ti = Physics2D.Raycast(transform.position, -transform.up, 1);

        if (hit.transform?.tag == "wall" || hi.transform?.tag == "wall" || tih.transform?.tag == "wall" || ti.transform?.tag == "wall")
        {
            TestCreat.bn--;
            Destroy(gameObject);
        }
    }
}
