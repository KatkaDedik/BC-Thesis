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

    private CharacterController character = null;
    public Transform CameraRig = null;
    public Transform Head = null;

    private bool isGrounded;
    private Vector3 movement;

    public Transform groundCheck;
    public float groundDistance = 0.01f;
    public LayerMask groundMask;
    public LayerMask undergroundMask;



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
            if (Physics.CheckSphere(groundCheck.position, groundDistance -0.1f , groundMask))
            {
                movement.y = 0.1f;
            }
            else
            {
                movement.y = 0;
            }
            
            
        }

        //Apply
        character.Move(character.transform.rotation * movement * Time.deltaTime);
    }

    private void SnapRotation()
    {
        float snapValue = 0.0f;
        transform.RotateAround(Head.position, Vector3.up, snapValue);
    }
}
