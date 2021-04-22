using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(VRCharacterController))]
public class ZeroGravityMovement : MonoBehaviour
{
    public SteamVR_Action_Boolean JoystickMovePress = null;
    public SteamVR_Action_Vector2 JoystickMoveValue = null;
    public float Energy = 5f;
    public float MaxSpeed = 3;
    public float Deceleration = 0.2f;
    public float RotationSpeed = 0.5f;

    private VRCharacterController controller;
    private float speed = 0f;

    private void Start()
    {
        controller = GetComponent<VRCharacterController>();
    }

    private void Update()
    {
        RotatePlayer();
    }

    private void RotatePlayer()
    {
        //Vector3 orientationEuler = new Vector3(
        //    controller.Head.localEulerAngles.x,
        //    0f,
        //    controller.Head.localEulerAngles.z);

        //var headOrientation = controller.Head.localRotation;

        transform.rotation = Quaternion.FromToRotation(transform.up, controller.Head.localEulerAngles) * transform.rotation;
    }
}
