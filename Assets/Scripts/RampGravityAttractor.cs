using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampGravityAttractor : MonoBehaviour, IGravityAttractor
{
    public enum Axis { X, Y, Z };
    public Axis DoNotMoveInAxis;

    public bool IsConvex = false;
    public Transform GravityHelper = null;

    private Vector3 center;

    private void Start()
    {
        center = GravityHelper.localPosition;
    }

    public Quaternion Attract(Transform player, Transform groundCheck)
    {
        GravityHelper.position = groundCheck.position;
        Vector3 gravityUp = GetGravityUp(GravityHelper.localPosition);

        //preventing sliding to sides
        switch (DoNotMoveInAxis)
        {
            case Axis.X:
                gravityUp.x = 0;
                break;
            case Axis.Y:
                gravityUp.y = 0;
                break;
            case Axis.Z:
                gravityUp.z = 0;
                break;
            default:
                break;
        }

        gravityUp.Normalize();
        return Quaternion.FromToRotation(player.up, gravityUp) * player.rotation;
    }

    /// <summary> This method calculates the gravity direction acording to contact point
    /// contact point is clamped if it leaves the ramp 
    /// </summary>
    /// <param name="contactPoint"> local position of point where the player touches the ramp</param>
    /// <returns></returns>
    private Vector3 GetGravityUp(Vector3 contactPoint)
    {
        Vector3 gravityUp;

        if (IsConvex)
        {
            contactPoint.y = Mathf.Clamp(contactPoint.y, -Mathf.Infinity, center.y);
            contactPoint.x = Mathf.Clamp(contactPoint.x, center.x, Mathf.Infinity);
            GravityHelper.localPosition = contactPoint;
            gravityUp = (GravityHelper.position - transform.TransformPoint(center));
        }
        else
        {
            contactPoint.y = Mathf.Clamp(contactPoint.y, center.y, Mathf.Infinity);
            contactPoint.x = Mathf.Clamp(contactPoint.x, -Mathf.Infinity, center.x);
            GravityHelper.localPosition = contactPoint;
            gravityUp = -(GravityHelper.position - transform.TransformPoint(center));
        }

        return gravityUp;
    }
}

