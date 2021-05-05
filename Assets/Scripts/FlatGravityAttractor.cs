using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatGravityAttractor : MonoBehaviour, IGravityAttractor
{
    public Vector3 GravityDirection;

    public Quaternion Attract(Transform player, Transform groundCheck)
    {
        return Quaternion.FromToRotation(player.up, -GravityDirection) * player.rotation;
    }
}
