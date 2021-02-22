using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TeleportTrigger : MonoBehaviour
{

    public List<TeleportPoint> activate;
    public Material activeMaterial;
    public Material inactiveMaterial;
    public GameObject teleportSysytem;
    public GameObject ring;

    private MeshRenderer mesh;

    private void Start()
    {
        mesh = ring.GetComponent<MeshRenderer>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "GroundCheck")
        {
            teleportSysytem.SetActive(true);
            for(int i = 0; i < activate.Capacity; i++)
            {
                activate[i].locked = false;
            }
            mesh.material = activeMaterial;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "GroundCheck")
        {
            teleportSysytem.SetActive(false);
            for (int i = 0; i < activate.Capacity; i++)
            {
                activate[i].locked = true;
            }
            mesh.material = inactiveMaterial;
        }
    }
}
