using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyGravity : MonoBehaviour
{

    public GravityAtractor atractor;

    void Update()
    {
        atractor.Attract(transform);
    }
}
