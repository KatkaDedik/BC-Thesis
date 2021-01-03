using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyGravity : MonoBehaviour
{

    public GravityAtractor atractor;
    void Start()
    {

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
    }

    void Update()
    {
        atractor.Attract(transform);
    }
}
