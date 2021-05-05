using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class TeleportArea : MonoBehaviour
{
    public GameObject Attractor;
    public IGravityAttractor attractor;

    private void Start()
    {
        attractor = Attractor.GetComponent<IGravityAttractor>();
    }

    public void ChangeAtractor(GameObject player)
    {
        player.GetComponent<BodyGravity>().attractor = attractor;
    }
}
