using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class logo : MonoBehaviour
{
    void Awake()
    {
        Invoke("QieHuanChangJing",5);
    }

    void QieHuanChangJing()
    {
        SceneManager.LoadScene("Title");
    }
}
