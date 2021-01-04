using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityTrigger : MonoBehaviour
{

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "GroundCheck")
        {
            player.GetComponent<BodyGravity>().atractor = GetComponent<GravityAtractor>();
        }
    }
}
