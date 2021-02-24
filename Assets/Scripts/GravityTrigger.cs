using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTrigger : MonoBehaviour
{

    public GravityAtractor atractor;
    private GameObject player;

    void OnTriggerStay(Collider other)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (other.tag == "GroundCheck")
        {
            player.GetComponent<BodyGravity>().atractor = atractor;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (other.tag == "GroundCheck")
        {
            player.GetComponent<BodyGravity>().atractor = atractor;
        }
    }
}
