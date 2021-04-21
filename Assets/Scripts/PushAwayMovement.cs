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
    public float distanceToHold = 0.15f;
    public LayerMask HoldMask;

    private VRCharacterController controller;
    private HandHoldingStatus status;
    private Vector3 holdingPosition;
    private Transform holdingHandTransform;

    private void Start()
    {
        controller = GetComponent<VRCharacterController>();
        status = HandHoldingStatus.None;
    }

    private void Update()
    {
        status = IsAnchored();
        if (status != HandHoldingStatus.None)
        {
            AnchoredMovement();
        }
    }

    private HandHoldingStatus IsAnchored()
    {

        if (status == HandHoldingStatus.None)
        {
            //check if right hand closed from last frame
            if (HoldAction.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                if (LookForObjectToHold(RightHand.transform))
                {
                    holdingPosition = RightHand.transform.position;
                    holdingHandTransform = RightHand.transform;
                    return HandHoldingStatus.Right;
                }
            }

            //check if left hand closed from last frame
            if (HoldAction.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                if (LookForObjectToHold(LeftHand.transform))
                {
                    holdingHandTransform = LeftHand.transform;
                    holdingPosition = LeftHand.transform.position;
                    return HandHoldingStatus.Left;
                }
            }
        }

        if(status == HandHoldingStatus.Left)
        {
            //if player still pressing action, do not change anything
            if (!HoldAction.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                return HandHoldingStatus.Left;
            }

            //check if right hand isn't trying to hold something
            if (HoldAction.GetState(SteamVR_Input_Sources.RightHand))
            {
                if (LookForObjectToHold(RightHand.transform))
                {
                    holdingHandTransform = RightHand.transform;
                    holdingPosition = RightHand.transform.position;
                    return HandHoldingStatus.Right;
                }
            }

            return HandHoldingStatus.None;
        }

        if (status == HandHoldingStatus.Right)
        {
            //if player still pressing action, do not change anything
            if (!HoldAction.GetStateUp(SteamVR_Input_Sources.RightHand))
            {
                return HandHoldingStatus.Right;
            }

            //check if left hand isn't trying to hold something
            if (HoldAction.GetState(SteamVR_Input_Sources.LeftHand))
            {
                if (LookForObjectToHold(LeftHand.transform))
                {
                    holdingHandTransform = LeftHand.transform;
                    holdingPosition = LeftHand.transform.position;
                    return HandHoldingStatus.Left;
                }
            }

            return HandHoldingStatus.None;
        }

        return status;
    }

    private bool LookForObjectToHold(Transform handTransform)
    {
        Collider[] colliders = Physics.OverlapSphere(handTransform.position, distanceToHold, HoldMask);
        return colliders.Length > 0;
    }

    private void AnchoredMovement()
    {
        var handDeltaPosition =  holdingPosition - holdingHandTransform.position;
        controller.MovePlayer(handDeltaPosition);
    }
}
