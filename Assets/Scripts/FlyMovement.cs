using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Assets.Scripts;

public class FlyMovement : MonoBehaviour
{

    public SteamVR_Action_Boolean FlyPress;
    public SteamVR_Action_Single SpeedAction;

    public Transform LeftHand;
    public Transform RightHand;
    public Transform Head;
    public float MaxSpeed = 8;

    private float speed = 5f;
    private CharacterController character;
    private bool isFlying = false;
    private BodyGravity bodyGravity;
    private Vector3 direction;
    private Vector3 leftDirection = Vector3.zero;
    private Vector3 rightDirection = Vector3.zero;
    void Start()
    {
        character = GetComponent<CharacterController>();
        bodyGravity = GetComponent<BodyGravity>();
    }

    void FixedUpdate()
    {
        SetDirection();
        SetSpeed();
        HandleCharacterController();
        if (isFlying)
        {
            character.Move = direction * Time.deltaTime * speed;
        }
    }

    private void SetDirection()
    {

        if (FlyPress.GetState(SteamVR_Input_Sources.LeftHand))
        {
            leftDirection = LeftHand.position - Head.position;
        }
        else
        {
            leftDirection /= 1.5f;
        }
        if (FlyPress.GetState(SteamVR_Input_Sources.RightHand))
        {
            rightDirection = RightHand.position - Head.position;
        }
        else
        {
            rightDirection /= 1.5f;
        }

        direction = leftDirection + rightDirection;
    }

    private void SetSpeed()
    {
        if(SpeedAction.axis > 0)
        {
            speed += (SpeedAction.axis / 100 ) * Time.deltaTime ;
        }
        else
        {
            speed -= 9.82f * Time.deltaTime;
        }

        speed = Mathf.Clamp(speed, 0, MaxSpeed);
        isFlying = (speed > 0);
    }

    private void HandleCharacterController()
    {
        bodyGravity.enabled = !isFlying;
        if (isFlying)
        {
            character.CheckIfGrounded = false;
            character.currentGravity = 0;
        }
        else
        {
            character.CheckIfGrounded = true;
        }
    }
}
