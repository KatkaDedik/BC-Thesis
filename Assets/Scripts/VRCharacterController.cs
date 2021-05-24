using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(CapsuleCollider))]
public class VRCharacterController : MonoBehaviour
{
    public float StepOffset = 0.3f;
    public float gravity = 9.82f;
    public LayerMask WalkableLayer;
    public LayerMask PlayerLayer;
    public bool CheckIfGrounded = true;
    public Transform Head = null;
    public Transform GroundCheck = null;
    [HideInInspector]
    public bool collided = false;
    public PostProcessVolume PP;
    public float ChromaticStrenght = 1f;
    public float currentChromaticStrenght = 0f;

    private Vector3 move;
    private CapsuleCollider capsuleCollider;
    public ChromaticAberration chromatic;
    public bool ScaleCollider = false;


    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        PP.profile.TryGetSettings(out chromatic);
    }

    private void Update()
    {
        HandleHeight();
        MovePlayer(downDirection * currentGravity * Time.deltaTime);
        chromatic.intensity.Override(currentChromaticStrenght * ChromaticStrenght);
        currentChromaticStrenght = 0f;
        transform.position += move;
        CollisionCheck();
        if (CheckIfGrounded)
        {
            grounded = IsGrounded();
        }
        else
        {
            grounded = false;
        }

    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void LateUpdate()
    {
        move = Vector3.zero;
    }

    public void HandleHeight()
    {
        //Get the head in local space
        float headHeight = Mathf.Clamp(Head.localPosition.y, 0.3f, 2.2f);

        capsuleCollider.height = headHeight + 0.1f;

        // Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = capsuleCollider.height / 2 + 0.05f;

        if (ScaleCollider)
        {
            capsuleCollider.height = 0.4f;
            newCenter.y = headHeight + 0.1f;
        }
        else
        {
            newCenter.y = capsuleCollider.height / 2 + 0.05f;
        }

        //Move capsule in local space
        newCenter.x = Head.localPosition.x;
        newCenter.z = Head.localPosition.z;

        //Apply
        capsuleCollider.center = newCenter;

        liftPoint = new Vector3(newCenter.x, newCenter.y + headHeight / 2, newCenter.z);
        GroundCheck.localPosition = new Vector3(newCenter.x, newCenter.y - headHeight / 2 - 0.01f, newCenter.z);
    }

    public void MovePlayer(Vector3 moveBy, bool usechromatic = true)
    {
        if (usechromatic)
        {
            currentChromaticStrenght += moveBy.magnitude;
        }
        move += moveBy;
    }

    #region Gravity methods
    //[HideInInspector]
    public bool grounded;
    public float currentGravity;//{ get; set; }
    private Vector3 liftPoint;
    private RaycastHit groundHit;
    private Ray ray;
    private Vector3 downDirection;

    private void ApplyGravity()
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

    public bool IsGrounded()
    {
        downDirection = GroundCheck.position - transform.TransformPoint(liftPoint);
        ray = new Ray(transform.TransformPoint(liftPoint), downDirection);

        Debug.DrawRay(transform.TransformPoint(liftPoint), downDirection);

        if (Physics.SphereCast(ray, 0.05f, out var tempHit, downDirection.magnitude + 0.05f, WalkableLayer))
        {
            return IsGroundHit(tempHit);
        }
        else
        {
            return false;
        }
    }

    private bool IsGroundHit(RaycastHit tempHit)
    {
        Collider[] col = new Collider[6];
        int num = Physics.OverlapSphereNonAlloc(GroundCheck.position, 0.5f, col, WalkableLayer);

        var ret = false;

        for (int i = 0; i < num; i++)
        {
            if (col[i].transform == tempHit.transform)
            {
                groundHit = tempHit;
                ret = true;

                Vector3 Translate = (downDirection * -1).normalized * (downDirection.magnitude - (groundHit.point - transform.TransformPoint(liftPoint)).magnitude);

                transform.position += Translate;
                break;
            }
        }

        if (num <= 1 && tempHit.distance <= 3f)
        {
            if (col[0] != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 2f, WalkableLayer))
                {
                    if (hit.transform != col[0].transform)
                    {
                        ret = false;
                        return ret;
                    }
                }
            }
        }
        return ret;
    }
    #endregion

    #region Collision Checking
    private void CollisionCheck()
    {
        var offset = StepOffset;
        if (!CheckIfGrounded)
        {
            offset = 0;
        }

        collided = false;
        Collider[] overlaps = new Collider[5];
        int num = Physics.OverlapCapsuleNonAlloc(
            transform.TransformPoint(capsuleCollider.center + (0.1f + capsuleCollider.height / 2) * transform.up),
            transform.TransformPoint(capsuleCollider.center + (capsuleCollider.height / 2 - offset) * -transform.up),
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
                collided = true;
                Vector3 penetrationVector = direction * distance;
                transform.position += penetrationVector;

            }
        }
    }
    #endregion
}
