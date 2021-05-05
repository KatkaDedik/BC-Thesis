using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGravityAttractor
{
    Quaternion Attract(Transform player, Transform groundCheck);
}
