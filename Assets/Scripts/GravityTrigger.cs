using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Switch the IGravityAttractor in Bodygravity by 
/// colliding when the player collides with trigger
/// </summary>
public class GravityTrigger : MonoBehaviour
{
    public GameObject AttractorGameObject;

    private GameObject player;
    private IGravityAttractor attractor;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        attractor = AttractorGameObject.GetComponent<IGravityAttractor>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "GroundCheck")
        {
            player.GetComponent<BodyGravity>().attractor = attractor;
        }
    }
}
