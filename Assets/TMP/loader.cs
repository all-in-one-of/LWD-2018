using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class loader : MonoBehaviour
{

    public void load1()
    {
        SceneManager.LoadScene(1);
    }

    public void load2()
    {
        SceneManager.LoadScene(2);
    }
}
