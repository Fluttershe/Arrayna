using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class title : MonoBehaviour
{
    public Animator zhuanchang;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            zhuanchang.SetBool("zhuanchang",true);
            Invoke("QieHuanChangJing",2);
        }
    }

    void QieHuanChangJing()
    {
        SceneManager.LoadScene("Menu");
    }
}
