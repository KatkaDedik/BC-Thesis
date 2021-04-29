using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTrigger : MonoBehaviour
{

    public GravityAtractor atractor;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "GroundCheck")
        {
            player.GetComponent<BodyGravity>().atractor = atractor;
        }
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GroundCheck")
        {
            player.GetComponent<BodyGravity>().atractor = atractor;
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
