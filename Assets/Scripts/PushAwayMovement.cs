using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

/// <summary>
/// Script for push away movement
/// </summary>
[RequireComponent(typeof(VRCharacterController))]
public class PushAwayMovement : MonoBehaviour
{
    /// <summary>
    /// Posibilities of holding
    /// </summary>
    enum HandHoldingStatus { Left, None, Right};

    public GameObject RightHand;
    public GameObject LeftHand;
    public SteamVR_Action_Boolean HoldAction;
    /// <summary>
    /// radius of a sphere around hand for detecting a collisions with objects that the player could hold
    /// </summary>
    public float DistanceToHold = 0.15f;
    public LayerMask HoldMask;
    public int PositionBufferSize = 24;
    public float Deceleration = 0.2f;

    private VRCharacterController controller;
    /// <summary>
    /// Which hand is currently holding onto something
    /// </summary>
    private HandHoldingStatus status;
    private Vector3 holdingPosition;
    private Transform holdingHandTransform;
    private Vector3[] positionsBuffer;
    private int bufferIndex = 0;
    private Vector3 velocityDirection = Vector3.zero;
    private float speed;

    private void Start()
    {
        controller = GetComponent<VRCharacterController>();
        status = HandHoldingStatus.None;
        positionsBuffer = new Vector3[PositionBufferSize];
    }

    private void Update()
    {
        status = IsAnchored();
        if (status != HandHoldingStatus.None)
        {
            AnchoredMovement();
        }
        else
        {
            Movement();
        }
    }
    
    private HandHoldingStatus IsAnchored()
    { 
        if (status == HandHoldingStatus.None)
        {
            //check if right hand closed from last frame
            if (StartedHolding(SteamVR_Input_Sources.RightHand, RightHand.transform))
            {
                return HandHoldingStatus.Right;
            }

            //check if left hand closed from last frame
            if (StartedHolding(SteamVR_Input_Sources.LeftHand, LeftHand.transform))
            {
                return HandHoldingStatus.Left;
            }
        }
        
        return StillHolding(status);
    }

    /// <summary>
    /// Check if the player is still holding something
    /// </summary>
    /// <param name="status">which hand was holding something last frame</param>
    /// <returns>hand that currently holds onto something</returns>
    private HandHoldingStatus StillHolding(HandHoldingStatus status)
    {

        if(!GetCompleteStatus(status, out var holdingHand, out var freeHand, out var otherHandStatus, out var otherHandTransform))
        {
            return HandHoldingStatus.None;
        }
        
        //if player still pressing action, do not change anything
        if (!HoldAction.GetStateUp(holdingHand))
        {
            return status;
        }
        //player stopped holding with this hand

        //check if free hand isn't trying to hold onto something
        if (HoldAction.GetState(freeHand))
        {
            if (LookForObjectToHold(otherHandTransform))
            {
                holdingHandTransform = otherHandTransform;
                holdingPosition = otherHandTransform.position;
                return otherHandStatus;
            }
        }
        PushAway();
        return HandHoldingStatus.None;
    }

    /// <summary>
    /// Set the out parameters accoding to currectStatus
    /// </summary>
    /// <returns>false if setting out parameters went wrong</returns>
    private bool GetCompleteStatus(HandHoldingStatus currentStatus, 
        out SteamVR_Input_Sources holdingHand, 
        out SteamVR_Input_Sources freeHand, 
        out HandHoldingStatus otherHand, 
        out Transform otherHandTransform)
    {
        switch (status)
        {
            case HandHoldingStatus.Right:
                holdingHand = SteamVR_Input_Sources.RightHand;
                freeHand = SteamVR_Input_Sources.LeftHand;
                otherHand = HandHoldingStatus.Left;
                otherHandTransform = LeftHand.transform;
                break;
            case HandHoldingStatus.Left:
                holdingHand = SteamVR_Input_Sources.LeftHand;
                freeHand = SteamVR_Input_Sources.RightHand;
                otherHand = HandHoldingStatus.Right;
                otherHandTransform = RightHand.transform;
                break;
            default:
                holdingHand = SteamVR_Input_Sources.Any;
                freeHand = SteamVR_Input_Sources.Any;
                otherHand = HandHoldingStatus.None;
                otherHandTransform = null;
                return false;
        }
        return true;
    }

    /// <summary>
    /// check if specific started holding onto something
    /// </summary>
    /// <param name="inputSource">input of specific hand</param>
    /// <param name="hand">trasform of specific hand</param>
    /// <returns>true if the inputSource is pressed and hand collided with HoldMask layer</returns>
    private bool StartedHolding(SteamVR_Input_Sources inputSource, Transform hand)
    {
        if (HoldAction.GetStateDown(inputSource))
        {
            if (LookForObjectToHold(hand))
            {
                holdingPosition = hand.position;
                holdingHandTransform = hand;
                return true;
            }
        }
        //the HoldAction on inputSorce was not pressed or the hand was not colliding with enithing
        return false;
    } 
    
    private bool LookForObjectToHold(Transform handTransform)
    {
        Collider[] colliders = Physics.OverlapSphere(handTransform.position, DistanceToHold, HoldMask);
        return colliders.Length > 0;
    }

    /// <summary>
    /// move the player according to holding hand displacement
    /// </summary>
    private void AnchoredMovement()
    {
        positionsBuffer[bufferIndex % PositionBufferSize] = transform.position;
        bufferIndex++;
        var handDeltaPosition =  holdingPosition - holdingHandTransform.position;
        controller.MovePlayer(handDeltaPosition, false);

        if (bufferIndex > 6 * PositionBufferSize)
        {
            bufferIndex -= 3 * PositionBufferSize;
        }
    }
    
    private void PushAway()
    {
        Vector3 currentPosition = positionsBuffer[(bufferIndex - 1) % PositionBufferSize ];
        Vector3 oldestPosition;
        if(bufferIndex < PositionBufferSize)
        {
            oldestPosition = positionsBuffer[0];
        }
        else
        {
            oldestPosition = positionsBuffer[bufferIndex % PositionBufferSize];
        }
        
        velocityDirection = currentPosition - oldestPosition;
        speed = velocityDirection.magnitude * 10;
        velocityDirection.Normalize();
        bufferIndex = 0;
    }

    private void Movement()
    {
        if (controller.collided)
        {
            speed = 0;
        }
        speed = Mathf.Clamp(speed - Time.deltaTime * Deceleration, 0, 50);
        var finalMove = velocityDirection * Time.deltaTime * speed;
        controller.MovePlayer(finalMove, true );
    }
}
