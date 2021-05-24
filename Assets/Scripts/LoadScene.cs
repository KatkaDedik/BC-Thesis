using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that loads the next scene when the player enters the trigger
/// </summary>
public class LoadScene : MonoBehaviour
{
    public string NextScene = "";

    private void OnTriggerStay(Collider other)
    {
        SceneManager.LoadScene(NextScene);
    }
}
