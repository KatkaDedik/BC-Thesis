using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles player's rotation according to attractor
/// </summary>
public class BodyGravity : MonoBehaviour
{
    public GameObject Attractor;
    public IGravityAttractor attractor;
    public Transform groundCheck;

    private void Start()
    {
        attractor = Attractor.GetComponent<IGravityAttractor>();
    }

    private void Update()
    {
        transform.rotation = attractor.Attract(transform, groundCheck);
    }
}
