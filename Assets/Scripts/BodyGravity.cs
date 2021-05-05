using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyGravity : MonoBehaviour
{
    public GameObject Attractor;
    public IGravityAttractor attractor;
    public Transform groundCheck;

    private void Start()
    {
        attractor = Attractor.GetComponent<IGravityAttractor>();
    }

    void Update()
    {
        transform.rotation = attractor.Attract(transform, groundCheck);
    }
}
