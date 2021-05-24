using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type of IGravityAttractor for constant gravity direciton
/// </summary>
public class FlatGravityAttractor : MonoBehaviour, IGravityAttractor
{
    /// <summary>
    /// The player's rotation is set as -GravityDirection
    /// </summary>
    public Vector3 GravityDirection;

    public Quaternion Attract(Transform player, Transform groundCheck)
    {
        return Quaternion.FromToRotation(player.up, -GravityDirection) * player.rotation;
    }
}
