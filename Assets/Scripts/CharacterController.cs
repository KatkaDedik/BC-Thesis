using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(JoystickMovement))]
public class CharacterController : MonoBehaviour
{
    public float StepOffset = 0.3f;
    public float SkinWidth = 0.08f;
    public float gravity = 9.82f;
    public LayerMask WalkableLayer;
    public LayerMask PlayerLayer;
    public bool CheckIfGrounded = true;
    public Transform Head = null;
    public Transform groundCheck = null;
    public Vector3 Move { get; set; }

    private CapsuleCollider capsuleCollider;
    private FlyMovement flying;

    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        flying = GetComponent<FlyMovement>();
    }

    private void Update()
    {
        HandleHeight();
        MovePlayer();
    }

    private void FixedUpdate()
    {
        if (CheckIfGrounded)
        {
            GroundChecking();
        }
        else
        {
            grounded = false;
        }
        Gravity();
        transform.position += Move * Time.fixedDeltaTime;
        
        CollisionCheck();
    }

    private void HandleHeight()
    {
        //Get the head in local space
        float headHeight = Mathf.Clamp(Head.localPosition.y, 0.3f, 2.2f);
        capsuleCollider.height = headHeight;

        // Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = headHeight / 2;

        //Move capsule in local space
        newCenter.x = Head.localPosition.x;
        newCenter.z = Head.localPosition.z;

        //Apply
        capsuleCollider.center = newCenter;

        liftPoint = new Vector3(newCenter.x, newCenter.y + headHeight / 2, newCenter.z);
        GroundCheck = new Vector3(newCenter.x, newCenter.y - headHeight / 2, newCenter.z);

        newCenter.y = groundCheck.localPosition.y;
        groundCheck.localPosition = newCenter;
    }

    #region Movement Methods


    public SteamVR_Action_Boolean JoystickMovePress = null;
    public SteamVR_Action_Vector2 JoystickMoveValue = null;

    public float Sensitivity = 0.1f;
    public float MaxSpeed = 1.0f;
    private float speed = 0.0f;

    private void MovePlayer()
    {
        Move = Vector3.zero;

        //Handle Joystick movement
        if (JoystickMovePress.state)
        {
            Move = CalculateJoystickMovement();
        }
        else if (JoystickMovePress.GetStateUp(SteamVR_Input_Sources.Any))
        {
            speed = 0;
        }

        Move += downDirection.normalized * currentGravity;
    }

    public Vector3 CalculateJoystickMovement()
    {
        //Figure out movement orientation
        Vector3 orientationEuler = new Vector3(0.0f, Head.localEulerAngles.y, 0.0f);
        Quaternion orientation = Quaternion.Euler(orientationEuler);

        var movement = Vector3.zero;

        //Add, clamp
        speed += JoystickMoveValue.axis.y * Sensitivity;
        speed = Mathf.Clamp(speed, -MaxSpeed, MaxSpeed);

        //Orientation
        movement = orientation * (speed * Vector3.forward);

        return transform.rotation * movement;
    }

    #endregion

    #region Gravity methods
    private bool grounded;
    public float currentGravity { get; set; }
    private Vector3 liftPoint;
    private RaycastHit groundHit;
    private Vector3 GroundCheck;
    private Ray ray;
    private Vector3 downDirection;

    private void Gravity()
    {
        if (!grounded)
        {
            currentGravity += gravity * Time.deltaTime;
        }
        else
        {
            currentGravity = 0;
        }
    }

    public void GroundChecking()
    {
        downDirection = transform.TransformPoint(GroundCheck) - transform.TransformPoint(liftPoint);
        ray = new Ray(transform.TransformPoint(liftPoint), downDirection);

        if (Physics.SphereCast(ray, 0.05f, out var tempHit, downDirection.magnitude + 0.05f, WalkableLayer))
        {
            ConfirmGround(tempHit);
        }
        else
        {
            grounded = false;
        }
    }

    private void ConfirmGround(RaycastHit tempHit)
    {
        Collider[] col = new Collider[6];
        int num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(GroundCheck), 0.5f, col, WalkableLayer);

        grounded = false;

        for (int i = 0; i < num; i++)
        {
            if (col[i].transform == tempHit.transform)
            {
                groundHit = tempHit;
                grounded = true;

                Vector3 Translate =  (downDirection * -1).normalized * (downDirection.magnitude - (groundHit.point - transform.TransformPoint(liftPoint)).magnitude);

                transform.position += Translate;
                break;
            }
        }

        if (num <= 1 && tempHit.distance <= 3f)
        {
            if (col[0] != null)
            {
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 2f, WalkableLayer))
                { 
                    if (hit.transform != col[0].transform)
                    {
                        grounded = false;
                        return;
                    }
                }
            }
        }
    }
    #endregion

    #region Collision Checking
    private void CollisionCheck()
    {
        Collider[] overlaps = new Collider[5];
        int num = Physics.OverlapCapsuleNonAlloc(
            transform.TransformPoint(capsuleCollider.center + (capsuleCollider.height / 2) * transform.up),
            transform.TransformPoint(capsuleCollider.center + (capsuleCollider.height / 2 - StepOffset) * -transform.up),
            capsuleCollider.radius,
            overlaps,
            ~PlayerLayer,
            QueryTriggerInteraction.UseGlobal);
        
        for (int i = 0; i < num; i++)
        {
            if (overlaps[i].isTrigger)
            {
                continue;
            }
            Transform t = overlaps[i].transform;
            Vector3 direction;
            float distance;

            if (Physics.ComputePenetration(capsuleCollider, transform.position, transform.rotation, 
                overlaps[i], t.position, t.rotation, out direction, out distance))
            {
                Vector3 penetrationVector = direction * distance;
                transform.position += penetrationVector;
            }
        }
    }
    #endregion
}
