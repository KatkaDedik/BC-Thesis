using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TeleportArea : MonoBehaviour
{
    public GameObject Attractor;
    public IGravityAttractor attractor;
    public bool PointingAt = false;
    private MeshRenderer mesh;

    private void Start()
    {
        attractor = Attractor.GetComponent<IGravityAttractor>();
        mesh = GetComponent<MeshRenderer>();
        ChangeMaterial(0);
    }

    public void ChangeAtractor(GameObject player)
    {
        player.GetComponent<BodyGravity>().attractor = attractor;
    }

    public void ChangeMaterial(int active)
    {
        mesh.material.SetInt("IsActive", active);
    }
}
