using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Assets.Scripts;

public class Flying : MonoBehaviour
{

    public SteamVR_Action_Boolean FlyAction;

    public Transform LeftHand;
    public Transform RightHand;
    public Transform Head;
    public float speed = 5f;

    private CharacterController character;
    private bool isFlying = false;
    private BodyGravity bodyGravity;
    private VRController controller;
    void Start()
    {
        character = GetComponent<CharacterController>();
        bodyGravity = GetComponent<BodyGravity>();
        controller = GetComponent<VRController>();
    }

    // Update is called once per frame
    void Update()
    {
        isFlying = false;
        Vector3 leftDirection = Vector3.zero;
        Vector3 rightDirection = Vector3.zero;

        if (FlyAction.GetState(SteamVR_Input_Sources.LeftHand)){
            leftDirection = Head.position - LeftHand.position;
            isFlying = true;
        }
        if (FlyAction.GetState(SteamVR_Input_Sources.RightHand)){
            rightDirection = Head.position - RightHand.position;
            isFlying = true;
        }

        bodyGravity.enabled = !isFlying;
        controller.enabled = !isFlying;

        Vector3 direction = leftDirection + rightDirection;
        character.Move(direction * Time.deltaTime * speed);
    }
}
