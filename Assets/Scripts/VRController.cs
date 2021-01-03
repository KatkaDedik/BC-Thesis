using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class VRController : MonoBehaviour
{
    public float gravity = 30.0f;
    public float Sensitivity = 0.1f;
    public float MaxSpeed = 1.0f;
    public float rotateIncrement = 90;

    public SteamVR_Action_Boolean RotatePress = null;
    public SteamVR_Action_Boolean MovePress = null;
    public SteamVR_Action_Vector2 MoveValue = null;

    private float speed = 0.0f;

    private CharacterController character = null;
    public Transform CameraRig = null;
    public Transform Head = null;

    private bool isGrounded;
    private Vector3 movement;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;


    private void Awake()
    {
        character = GetComponent<CharacterController>();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        HandleHeight();
        CalculateMovement();
        SnapRotation();
    }

    private void HandleHeight()
    {
        //Get the head in local space
        float headHeight = Mathf.Clamp(Head.localPosition.y, 0.3f, 2.2f);
        character.height = headHeight;

        // Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = character.height / 2;

        //Move capsule in local space
        newCenter.x = Head.localPosition.x;
        newCenter.z = Head.localPosition.z;

        //Apply
        character.center = newCenter;
    }
    private void CalculateMovement()
    {
        //Figure out movement orientation
        Vector3 orientationEuler = new Vector3(0, Head.eulerAngles.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);

        //if not moving
        if (MovePress.GetStateUp(SteamVR_Input_Sources.Any))
        {
            speed = 0;
        }

        movement.x = 0;
        movement.z = 0;

        //if button pressed
        if (MovePress.state)
        {
            //Add, clamp
            speed += MoveValue.axis.y * Sensitivity;
            speed = Mathf.Clamp(speed, -MaxSpeed, MaxSpeed);

            //Orientation
            movement += orientation * (speed * Vector3.forward);
        }
        
        if (!isGrounded)
        {
            // Grativy
            movement.y += gravity * Time.deltaTime;
        }
        else
        {
            movement.y = 0;
        }
        

        //Apply
        character.Move(movement * Time.deltaTime);
    }

    private void SnapRotation()
    {
        float snapValue = 0.0f;
        transform.RotateAround(Head.position, Vector3.up, snapValue);

    }
}
