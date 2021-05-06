using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(VRCharacterController))]
public class PushAwayMovement : MonoBehaviour
{
    enum HandHoldingStatus { Left, None, Right};

    public GameObject RightHand;
    public GameObject LeftHand;
    public SteamVR_Action_Boolean HoldAction;
    public float DistanceToHold = 0.15f;
    public LayerMask HoldMask;
    public int PositionBufferSize = 24;
    public float Deceleration = 0.2f;

    private VRCharacterController controller;
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
        return false;
    } 

    private bool LookForObjectToHold(Transform handTransform)
    {
        Collider[] colliders = Physics.OverlapSphere(handTransform.position, DistanceToHold, HoldMask);
        return colliders.Length > 0;
    }

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
