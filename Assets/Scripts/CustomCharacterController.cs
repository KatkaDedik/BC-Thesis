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
    public LayerMask PlayerLayer;
    public bool Smooth;
    public float SmoothSpeed = 1f;

    private SphereCollider sphereCollider;
    private CapsuleCollider capsuleCollider;
    private Vector3 velocity;
    private Vector3 move;
    private Vector3 vel;

    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
        Gravity();
        FinalMove();
        GroundChecking();
    }

    public void UpdateCollider()
    {
        capsuleCollider.center = Center;
        capsuleCollider.height = Height;
        capsuleCollider.radius = Radius;
    }

    #region Movement Methods
    public void Move(Vector3 motion)
    {
        move = motion;
    }

    private void FinalMove()
    {
        velocity = new Vector3(move.x, -currentGravity, move.z) + vel;
        velocity = transform.TransformDirection(velocity);
        transform.position += velocity * Time.deltaTime;
        vel = Vector3.zero;
    }
    #endregion

    #region Gravity methods
    private bool grounded;
    private float currentGravity = 0f;
    private Vector3 liftPoint = new Vector3(0, 1.2f, 0);
    private RaycastHit groundHit;
    private Vector3 GroundCheck = new Vector3(0, 0.85f, 0);
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
        Ray ray = new Ray(transform.TransformPoint(liftPoint), Vector3.down);
        RaycastHit tempHit = new RaycastHit();

        if (Physics.SphereCast(ray, 0.17f, out tempHit, 20, ~PlayerLayer))
        {
            ConfirmGround(tempHit);
        }
        else
        {
            grounded = (Physics.SphereCast(ray, 0.17f, out tempHit, 20, ~PlayerLayer);
        }
    }

    private void ConfirmGround(RaycastHit tempHit)
    {
        Collider[] col = new Collider[3];
        int num = Physics.OverlapSphereNonAlloc(transform.TransformPoint(GroundCheck), 0.5f, col, PlayerLayer);

        grounded = false;

        for (int i = 0; i < num; i++)
        {
            if (col[i].transform == tempHit.transform)
            {
                groundHit = tempHit;
                grounded = true;

                Vector3 destination = new Vector3(transform.position.x, (groundHit.point.y + Height / 2f), transform.position.z);

                if (!Smooth)
                {
                    transform.position = destination;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, destination, SmoothSpeed * Time.deltaTime);
                }
                break;
            }
        }

        if(num <= 1 && tempHit.distance <= 3f)
        {
            if (col[0] != null)
            {
                Ray ray = new Ray(transform.TransformPoint(liftPoint), Vector3.down);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 3f, PlayerLayer))
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
            overlaps, PlayerLayer, 
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
