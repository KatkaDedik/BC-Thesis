using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class VRController : MonoBehaviour
{
    public float gravity = 9.81f;
    public float Sensitivity = 0.1f;
    public float MaxSpeed = 1.0f;
    public float rotateIncrement = 90;

    public SteamVR_Action_Boolean RotatePress = null;
    public SteamVR_Action_Boolean MovePress = null;
    public SteamVR_Action_Vector2 MoveValue = null;

    private float speed = 0.0f;

    private CustomCharacterController character = null;
    public Transform CameraRig = null;
    public Transform Head = null;

    private Vector3 movement;

    public Transform groundCheck;



    private void Awake()
    {
        character = GetComponent<CustomCharacterController>();
    }

    private void Update()
    {
        HandleHeight();
        CalculateMovement();
    }

    private void HandleHeight()
    {
        //Get the head in local space
        float headHeight = Mathf.Clamp(Head.localPosition.y, 0.3f, 2.2f);
        character.Height = headHeight;

        // Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = character.Height / 2;

        //Move capsule in local space
        newCenter.x = Head.localPosition.x;
        newCenter.z = Head.localPosition.z;

        //Apply
        character.Center = newCenter;
        character.UpdateCollider();

        newCenter.y = groundCheck.localPosition.y;
        groundCheck.localPosition = newCenter;
    }
    private void CalculateMovement()
    {
        //Figure out movement orientation
        Vector3 orientationEuler = new Vector3(0.0f, Head.localEulerAngles.y, 0.0f);
        Quaternion orientation = Quaternion.Euler(orientationEuler);

        //if not moving
        if (MovePress.GetStateUp(SteamVR_Input_Sources.Any))
        {
            speed = 0;
        }

        movement = Vector3.zero;

        //if button pressed
        if (MovePress.state)
        {
            //Add, clamp
            speed += MoveValue.axis.y * Sensitivity;
            speed = Mathf.Clamp(speed, -MaxSpeed, MaxSpeed);

            //Orientation
            movement = orientation * (speed * Vector3.forward);
        }

        var debug = movement * Time.deltaTime;

        //Apply
        character.Move = transform.rotation * debug;
    }
}
