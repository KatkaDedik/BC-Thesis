using UnityEngine;
using Valve.VR;

/// <summary>
/// Basic joystic movement that calculates the player's movement accordig to headset rotation and the
/// pressing position on trackpad.
/// </summary>
[RequireComponent(typeof(VRCharacterController))]
public class JoystickMovement : MonoBehaviour
{
    public SteamVR_Action_Boolean JoystickMovePress = null;
    public SteamVR_Action_Vector2 JoystickMoveValue = null;
    public float Sensitivity = 0.1f;
    public float MaxSpeed = 1.0f;

    private float speed = 0.0f;

    private VRCharacterController controller;

    private void Start()
    {
        controller = GetComponent<VRCharacterController>();
    }

    private void Update()
    {
        //Figure out movement orientation
        Vector3 orientationEuler = new Vector3(0.0f, controller.Head.localEulerAngles.y, 0.0f);
        Quaternion orientation = Quaternion.Euler(orientationEuler);

        //stop the movement if the JoystickMovePress is no longer pressed
        if (JoystickMovePress.GetStateUp(SteamVR_Input_Sources.Any))
        {
            speed = 0;
        }

        //Add, clamp
        if (JoystickMovePress.GetState(SteamVR_Input_Sources.Any))
        {
            speed += JoystickMoveValue.axis.y * Sensitivity;
            speed = Mathf.Clamp(speed, -MaxSpeed, MaxSpeed);
        }

        controller.MovePlayer(transform.rotation * orientation * (speed * Vector3.forward) * Time.deltaTime);
    }

}
