using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Node of a pseudo linked list. First checkpoit has IsActive set to true, and all others has Isactive set to false.
/// When player collides with checkpoint tak has IsActive set to true, the checkpoint is destroyed, and set IsActive
/// for NextCheckpoint to true.
/// The last checkpoint has IsLast set to true. When the player collides with them, they finished the level and send 
/// to another scene.
/// </summary>
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
        else if(IsActive)
        {
            NextCheckpoint.Activate();
            Destroy(this.gameObject, 0.1f);
        }
    }

    private void Activate()
    {
        IsActive = true;
        GetComponent<Renderer>().material = ActiveMaterial;
    }

}
