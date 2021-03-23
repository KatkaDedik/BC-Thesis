using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Teleporter : MonoBehaviour
{
    public GameObject Player;
    public GameObject Pointer;
    public SteamVR_Action_Boolean TeleportAction;
    public Transform GroundCheck;
    public LayerMask TeleportMask;

    private SteamVR_Behaviour_Pose pose = null;
    private bool hasPosition = false;
    private bool isTeleporting = false;
    private float fadeTime = 0.5f;


    private void Awake()
    {
        pose = GetComponent<SteamVR_Behaviour_Pose>();
    }


    void Update()
    {
        //Pointer
        hasPosition = UpdatePointer();
        Pointer.SetActive(hasPosition);

        //Teleport
        if (TeleportAction.GetStateUp(pose.inputSource))
        {
            TryTeleport();
        }

    }

    private void TryTeleport()
    {
        //Check for valid positio, and if already teleporting;
        if(!hasPosition || isTeleporting)
        {
            return;
        }

        //Figure out transition
        Vector3 groundPosition = GroundCheck.position;
        Vector3 translateVector = Pointer.transform.position - groundPosition;

        //Move
        StartCoroutine(MovePlayer(translateVector));
    }

    private IEnumerator MovePlayer(Vector3 translation)
    {
        // Flag
        isTeleporting = true;

        // Fade
        SteamVR_Fade.Start(Color.black, fadeTime, true);

        //Apply translation
        yield return new WaitForSeconds(fadeTime);
        Player.transform.position += translation;

        // Fade to clear
        SteamVR_Fade.Start(Color.clear, fadeTime, true);

        //De-flag
        isTeleporting = false;
    }

    private bool UpdatePointer()
    {
        // Ray from the controller
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // if it is a hit
        if (Physics.Raycast(ray, out hit, TeleportMask))
        {
            Pointer.transform.position = hit.point;
            return true;
        }

        //if not a hit
        return false;
    }
}
