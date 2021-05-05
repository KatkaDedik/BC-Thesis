using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GroundCheck")
        {
            player.GetComponent<BodyGravity>().attractor = attractor;
            Debug.Log($"enter {this.gameObject.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "GroundCheck")
        {
            Debug.Log($"left {this.gameObject.name}");
        }
    }
}
