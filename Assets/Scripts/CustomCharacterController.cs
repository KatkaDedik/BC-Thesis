using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(SphereCollider))]
public class CustomCharacterController : MonoBehaviour
{
    public float SlopeLimit = 45f;
    public float StepOffset = 0.3f;
    public float SkinWidth = 0.08f;
    public float MinMoveDistance = 0.001f;
    public Vector3 Center = Vector3.zero;
    public float Radius = 0.2f;
    public float Height = 2f;
    public float gravity = 2.5f;
    public LayerMask WalkableLayer;
    public LayerMask PlayerLayer;
    public bool Smooth;
    public float SmoothSpeed = 1f;
    public Vector3 Move { get; set; }

    public bool debugRay = false;

    private SphereCollider sphereCollider;
    private CapsuleCollider capsuleCollider;
    private Vector3 velocity;

    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void FixedUpdate()
    {
        Gravity();
        FinalMove();
        GroundChecking();

        if (debugRay)
        {
            Debug.DrawRay(transform.TransformPoint(liftPoint), downDirection, Color.magenta);
        }
    }


    public void UpdateCollider()
    {
        capsuleCollider.center = Center;
        capsuleCollider.height = Height;
        capsuleCollider.radius = Radius;
        liftPoint = new Vector3(Center.x, Center.y + Height/2 , Center.z);
        GroundCheck = new Vector3(Center.x, Center.y - Height / 2, Center.z);
    }

    #region Movement Methods

    private void FinalMove()
    {
        //velocity = new Vector3(Move.x, -currentGravity, Move.z);
        //velocity = transform.TransformDirection(velocity);

        Move += downDirection.normalized * currentGravity * Time.fixedDeltaTime;

        transform.position += Move;
    }
    #endregion



    #region Gravity methods
    private bool grounded;
    public float currentGravity = 0f;
    private Vector3 liftPoint;
    private RaycastHit groundHit;
    private Vector3 GroundCheck;
    private Ray ray;
    private Vector3 downDirection;

    private void Gravity()
    {
        if (!grounded)
        {
            currentGravity = gravity;
        }
        else
        {
            currentGravity = 0;
        }
    }

    private void GroundChecking()
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

                //Vector3 destination = new Vector3(transform.position.x, (groundHit.point.y + Height / 2f), transform.position.z);
                Vector3 Translate =  (downDirection * -1).normalized * (downDirection.magnitude - (groundHit.point - transform.TransformPoint(liftPoint)).magnitude);

                if (!Smooth)
                {
                    transform.position += Translate;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, Translate, SmoothSpeed * Time.deltaTime);
                }
                break;
            }
        }

        if (num <= 1 && tempHit.distance <= 3f)
        {
            if (col[0] != null)
            {
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 2f, ~PlayerLayer))
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
        int num = Physics.OverlapSphereNonAlloc
            (
            transform.TransformPoint(sphereCollider.center), 
            sphereCollider.radius, 
            overlaps, ~PlayerLayer, 
            QueryTriggerInteraction.UseGlobal
            );
        
        for (int i = 0; i < num; i++)
        {
            Transform t = overlaps[i].transform;
            Vector3 direction;
            float distance;

            if (Physics.ComputePenetration(sphereCollider, transform.position, transform.rotation, 
                overlaps[i], t.position, t.rotation, out direction, out distance))
            {
                Vector3 penetrationVector = direction * distance;
                //Vector3 velocityProjected = Vector3.Project(velocity, -direction);
                transform.position += penetrationVector;
                //vel -= velocityProjected;
            }
        }
    }
    #endregion
}
