using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public GameObject door;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            this.enabled = false;
            Destroy(door);
        }
    }
}
