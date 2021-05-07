using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public CheckPoint NextCheckpoint;
    public Material ActiveMaterial;

    public bool IsActive = false;
    public bool IsLast = false;
    public LoadScene Load;

    private void Start()
    {
        if (IsActive)
        {
            GetComponent<Renderer>().material = ActiveMaterial;
        }

        if (Load != null)
        {
            Load.enabled = false;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (IsLast)
        {
            Load.enabled = true;
        }
        else
        {
            NextCheckpoint.Activate();
        }

        Destroy(this.gameObject, 0.1f);
    }

    private void Activate()
    {
        IsActive = true;
        GetComponent<Renderer>().material = ActiveMaterial;
    }

}
