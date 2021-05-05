using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(VRCharacterController))]
public class ZeroGravityMovement : MonoBehaviour
{
    public SteamVR_Action_Boolean RocketMovePress = null;
    public SteamVR_Action_Vector2 RocketMoveValue = null;
    public float MaxEnergy = 1f;
    public float MaxSpeed = 3;
    public float Deceleration = 0.2f;
    public float RotationSpeed = 0.1f;
    public float Sensitivity = 1;
    public Transform RotationHelper = null;
    
    private Vector3 direction;
    private VRCharacterController controller;
    private float speed = 0f;
    private float energy;

    private LineRenderer line;
    private Vector3[] positions;

    private void Start()
    {
        controller = GetComponent<VRCharacterController>();
        energy = MaxEnergy;
        line = GetComponent<LineRenderer>();
        positions = new Vector3[2];
    }

    private void Update()
    {
        //Figure out movement orientation
        Vector3 orientationEuler = new Vector3(controller.Head.localEulerAngles.x, controller.Head.localEulerAngles.y, 0.0f);
        Quaternion orientation = Quaternion.Euler(orientationEuler);

        var rotatedForward = controller.Head.localRotation * Vector3.forward;
        rotatedForward.x = 0;
        rotatedForward.Normalize();
        //var cos = Vector3.Dot(rotatedForward, Vector3.forward);

        var angle = Quaternion.Angle(controller.Head.localRotation, Quaternion.Euler(rotatedForward));

        //Move RotationHelper
        RotationHelper.position = controller.groundCheck.position;
        var rotation = Mathf.Clamp(angle * RotationSpeed, -RotationSpeed, RotationSpeed);
        RotationHelper.localPosition = new Vector3(RotationHelper.localPosition.x, RotationHelper.localPosition.y, RotationHelper.localPosition.z - rotation);

        positions[0] = controller.Head.position;
        positions[1] = RotationHelper.position;
        line.SetPositions(positions);

        if (RocketMovePress.GetState(SteamVR_Input_Sources.Any) && energy > 0)
        {
            

            var axis = Mathf.Clamp(RocketMoveValue.axis.y, 0, 1);
            speed += axis * Sensitivity;
            energy -= Time.deltaTime * RocketMoveValue.axis.y;
            RotatePlayer();
            direction = transform.rotation * orientation * Vector3.forward * Time.deltaTime;
        }
        else
        {
            speed -= Deceleration * Time.deltaTime;
            energy += Time.deltaTime;
            
        }
        if (controller.collided)
        {
            speed = 0;
        }
        speed = Mathf.Clamp(speed, 0, MaxSpeed);

        controller.MovePlayer(direction * speed);
    }

    private void RotatePlayer()
    {
        var directionUp = controller.Head.position - RotationHelper.position;
        transform.rotation = Quaternion.FromToRotation(transform.up, directionUp) * transform.rotation;
    }
}
