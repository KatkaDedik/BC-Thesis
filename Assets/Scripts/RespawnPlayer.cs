using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{
    private GameObject player;
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        player.transform.position = transform.position;
    }
}
