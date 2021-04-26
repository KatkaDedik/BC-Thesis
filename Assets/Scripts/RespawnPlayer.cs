using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{

    public bool BodyGravityScript = false;
    public GravityAtractor Attractor;
    public bool JoystickMovementScript = false;
    public bool FlyMovementScript = false;
    public bool PushAwayMovementScript = false;
    public bool ZeroGravityMovementScript = false;
    public bool TeleportMovementScript = false;
    public bool DashTeleport = false;
    
    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        player.transform.position = transform.position;

        player.GetComponent<BodyGravity>().enabled = BodyGravityScript;
        player.GetComponent<BodyGravity>().atractor = Attractor;
        player.GetComponent<JoystickMovement>().enabled = JoystickMovementScript;
        player.GetComponent<FlyMovement>().enabled = FlyMovementScript;
        player.GetComponent<PushAwayMovement>().enabled = PushAwayMovementScript;
        player.GetComponent<ZeroGravityMovement>().enabled = ZeroGravityMovementScript;
        player.GetComponentInChildren<TeleportMovement>().enabled = TeleportMovementScript;
        player.GetComponentInChildren<TeleportMovement>().SmoothTeleport = DashTeleport;
    }
}
