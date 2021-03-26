using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class TeleportArea : MonoBehaviour
{
    public GravityAtractor atractor;

    public void ChangeAtractor(GameObject player)
    {
        player.GetComponent<BodyGravity>().atractor = atractor;
    }
}
