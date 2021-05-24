using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/// <summary>
/// Script for ``Superman'' flying
/// </summary>
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
    
    private VRCharacterController controller;
    private BodyGravity bodyGravity;
    private float currentRightPower = 0f;
    private float currentLeftPower = 0f;
    private Vector3 leftDirection = Vector3.zero;
    private Vector3 rightDirection = Vector3.zero;
    private bool isFlying = false;
    private Vector3 direction;
    private float deceleration;

    void Start()
    {
        controller = GetComponent<VRCharacterController>();
        bodyGravity = GetComponent<BodyGravity>();
        deceleration = Mathf.Clamp(controller.gravity, MinDeceleration, 20f);
    }

    /// <summary>
    /// Each frame, set the direction of movement, and move the player
    /// </summary>
    private void Update()
    {
        SetDirection();
        HandleCharacterController();
        if((direction * Time.deltaTime) != Vector3.zero)
        {
            controller.MovePlayer(direction * Time.deltaTime);
        }
    }

    /// <summary>
    /// Calculate the direction from positions of left and right hands
    /// </summary>
    private void SetDirection()
    {
        // Get the pseudo position of the playe's shoulders
        var shoulders = transform.TransformPoint(controller.Head.localPosition.x, controller.Head.localPosition.y - 0.15f, controller.Head.localPosition.z);

        // Get the direction from shoulders to left hand if the trigger on the left controller is pressed
        if (FlyPress.GetState(SteamVR_Input_Sources.LeftHand))
        {
            leftDirection = LeftHand.position - shoulders;
        }
        currentLeftPower = GetPowerInHand(SteamVR_Input_Sources.LeftHand, currentLeftPower);


        // Get the direction from shoulders to right hand if the trigger on the right controller is pressed
        if (FlyPress.GetState(SteamVR_Input_Sources.RightHand))
        {
            rightDirection = RightHand.position - shoulders;
        }
        currentRightPower = GetPowerInHand(SteamVR_Input_Sources.RightHand, currentRightPower);

        //the player is flying if they press the trigger at least on one controller
        isFlying = (FlyPress.GetState(SteamVR_Input_Sources.LeftHand) || FlyPress.GetState(SteamVR_Input_Sources.RightHand));

        //calculates the final direction
        direction = leftDirection * currentLeftPower + rightDirection * currentRightPower;
    }

    /// <summary>
    /// calculates the power from in hand. 
    /// </summary>
    /// <param name="source">which hand</param>
    /// <param name="power">waht was the last power in hand</param>
    /// <returns>current power in hand</returns>
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

    /// <summary>
    /// Turn on and off gravity in VRCharactercontroller if IsFlying = true
    /// </summary>
    private void HandleCharacterController()
    {
        bodyGravity.enabled = !isFlying;
        if (isFlying)
        {
            controller.CheckIfGrounded = false;
            controller.currentGravity = 0;
        }
        else
        {
            controller.CheckIfGrounded = true;
        }
    }
}
