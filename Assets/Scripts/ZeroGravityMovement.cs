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
    
    private Vector3 direction;
    private VRCharacterController controller;
    private float speed = 0f;
    private float energy;
    private void Start()
    {
        controller = GetComponent<VRCharacterController>();
        energy = MaxEnergy;
    }

    private void Update()
    {
        //Figure out movement orientation
        Vector3 orientationEuler = new Vector3(0.0f, controller.Head.localEulerAngles.y, 0.0f);
        Quaternion orientation = Quaternion.Euler(orientationEuler);

        if (RocketMovePress.GetState(SteamVR_Input_Sources.Any) && energy > 0)
        {
            speed += RocketMoveValue.axis.y * Sensitivity;
            energy -= Time.deltaTime * RocketMoveValue.axis.y;
            RotatePlayer();
            direction = transform.rotation * orientation * Vector3.forward * Time.deltaTime;
        }
        else
        {
            speed -= Deceleration * Time.deltaTime;
            energy += Time.deltaTime;
        }

        speed = Mathf.Clamp(speed, 0, MaxSpeed);

        controller.MovePlayer(direction * speed);
    }

    private void RotatePlayer()
    {
        var rotatedForward = controller.Head.localRotation * Vector3.forward;
        rotatedForward.x = 0;
        rotatedForward.Normalize();
        var cosRotation = Vector3.Dot(rotatedForward, Vector3.forward);

        cosRotation = Mathf.Clamp(cosRotation, 0, 1);
        cosRotation = 1 - cosRotation;

        transform.localRotation *= Quaternion.AngleAxis(cosRotation * RotationSpeed * Time.deltaTime, Vector3.right);
    }
}
