using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Assets.Scripts;

[RequireComponent(typeof(VRCharacterController))]
public class FlyMovement : MonoBehaviour
{

    public SteamVR_Action_Boolean FlyPress;
    public SteamVR_Action_Single SpeedAction;

    public Transform LeftHand;
    public Transform RightHand;
    public float MaxSpeed = 10f;
    public float Speed = 10f;
    public float MinDeceleration = 1f;

    private float currentRightPower = 0f;
    private float currentLeftPower = 0f;
    private VRCharacterController controller;
    private bool isFlying = false;
    private BodyGravity bodyGravity;
    private Vector3 direction;
    private Vector3 leftDirection = Vector3.zero;
    private Vector3 rightDirection = Vector3.zero;
    private float deceleration;

    void Start()
    {
        controller = GetComponent<VRCharacterController>();
        bodyGravity = GetComponent<BodyGravity>();
        deceleration = Mathf.Clamp(controller.gravity, MinDeceleration, 20f);
    }

    private void Update()
    {
        SetDirection();
        HandleCharacterController();
        controller.MovePlayer(direction * Time.deltaTime);
    }

    private void SetDirection()
    {
        var shoulders = transform.TransformPoint(controller.Head.localPosition.x, controller.Head.localPosition.y - 0.15f, controller.Head.localPosition.z);
        if (FlyPress.GetState(SteamVR_Input_Sources.LeftHand))
        {
            leftDirection = LeftHand.position - shoulders;
        }
        currentLeftPower = GetPowerInHand(SteamVR_Input_Sources.LeftHand, currentLeftPower);

        if (FlyPress.GetState(SteamVR_Input_Sources.RightHand))
        {
            rightDirection = RightHand.position - shoulders;
        }
        currentRightPower = GetPowerInHand(SteamVR_Input_Sources.RightHand, currentRightPower);

        isFlying = (FlyPress.GetState(SteamVR_Input_Sources.LeftHand) || FlyPress.GetState(SteamVR_Input_Sources.RightHand));

        direction = leftDirection * currentLeftPower + rightDirection * currentRightPower;
    }

    private float GetPowerInHand(SteamVR_Input_Sources source, float power)
    {
        float currentPower = power;
        if(SpeedAction.GetAxis(source) > 0)
        {
            currentPower += SpeedAction.GetAxis(source) * Speed * Time.deltaTime;
        }
        else
        {
            if (controller.grounded)
            {
                currentPower -= deceleration * 5 * Time.deltaTime;
            }
            currentPower -= deceleration * Time.deltaTime;
        }

        return Mathf.Clamp(currentPower, 0, MaxSpeed);
    }

    private void HandleCharacterController()
    {
        bodyGravity.enabled = !isFlying;
        if (isFlying)
        {
            //controller.CheckIfGrounded = false;
            controller.currentGravity = 0;
        }
        else
        {
            //controller.CheckIfGrounded = true;
        }
    }
}
