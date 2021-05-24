using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;


/// <summary>
/// Return player to Start scene when both menu buttons are pressed
/// </summary>
public class BackToStartScene : MonoBehaviour
{
    public SteamVR_Action_Boolean ResetAction;

    void Update()
    {
        if (ResetAction.GetState(SteamVR_Input_Sources.RightHand)
            && ResetAction.GetState(SteamVR_Input_Sources.LeftHand))
        {
            SceneManager.LoadScene("StartRoom");
        }
    }
}
