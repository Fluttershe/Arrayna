using UnityEngine;
using System.Collections;

public class black : MonoBehaviour
{
    public GameObject cube3, cube4, cube1;

    public bool kaiguan;

    private void Start()
    {
        kaiguan = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (kaiguan == true)
        {
            if (TestCreat.times >= 7)
            {
                if (collision.tag=="floor")
                {
                    var shang = Physics2D.Raycast(transform.position, transform.up);
                    if (shang.collider.gameObject.tag == "floor")
                    {
                        GameObject go = Instantiate(cube3, transform.position + new Vector3(0, 0.4f, 0), Quaternion.identity) as GameObject;
                    }
                    var xia = Physics2D.Raycast(transform.position, -transform.up);
                    if (xia.collider.gameObject.tag == "floor")
                    {
                        GameObject go = Instantiate(cube4, transform.position, Quaternion.identity) as GameObject;
                    }
                    var zuo = Physics2D.Raycast(transform.position, -transform.right);
                    if (zuo.collider.gameObject.tag == "floor")
                    {
                        GameObject go = Instantiate(cube1, transform.position + new Vector3(-0.4f, 0, 0), Quaternion.identity) as GameObject;
                    }
                    var you = Physics2D.Raycast(transform.position, transform.right);
                    if (you.collider.gameObject.tag == "floor")
                    {
                        GameObject go = Instantiate(cube1, transform.position + new Vector3(0.4f, 0, 0), Quaternion.identity) as GameObject;
                    }
                    if (xia.transform != null)
                    {
                        print(xia.collider.gameObject.tag);
                    }
                    kaiguan = false;
                }
            }
        }
    }
}
