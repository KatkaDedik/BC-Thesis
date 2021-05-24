using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// interface for RampGravityAttractor and FlatGravityAttractor
/// </summary>
public interface IGravityAttractor
{
    Quaternion Attract(Transform player, Transform groundCheck);
}
