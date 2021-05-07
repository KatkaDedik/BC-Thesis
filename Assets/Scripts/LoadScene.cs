using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

    public string nextScene = "";

    private void OnTriggerStay(Collider other)
    {
        SceneManager.LoadScene(nextScene);
    }
}
