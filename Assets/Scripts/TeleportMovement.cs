using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Assets.Scripts;

[RequireComponent(typeof(LineRenderer))]
public class TeleportMovement : MonoBehaviour
{
    public enum Type { Dash, Fade};
    public Type TeleportType = Type.Dash;

    public GameObject Player;
    public GameObject Pointer;
    public SteamVR_Action_Boolean TeleportAction;
    public Transform GroundCheck;
    public LayerMask IgnoreMask;
    public Material onHitMaterial;
    public Material onMissMaterial;
    public float Gravity = 9.82f;

    private bool isTeleporting = false;
    private SteamVR_Behaviour_Pose pose = null;
    private bool pointerHasPosition = false;
    private LineRenderer line;
    private VRCharacterController controller;
    private Vector3 GroundCheckOffset = Vector3.zero;

    // Fade teleport
    private float fadeTime = 0.5f;
    private TeleportArea area;
    private Vector3 destination;

    //Dash teleport
    private float teleportTimer = 0f;
    private Vector3 endPosition = Vector3.zero;
    private Vector3 startPosition = Vector3.zero;
    private Quaternion startQuaternion;
    private Quaternion endQuaternion;
    private float distance = 1f;
    private float angleDistance;
    private float speed = 0f;

    private void Awake()
    {
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        Pointer.SetActive(false);
        line = GetComponent<LineRenderer>();
        controller = Player.GetComponent<VRCharacterController>();
    }

    private void Update()
    {
        //Pointer
        pointerHasPosition = UpdatePointer();

        //Teleport
        if (TeleportAction.GetStateUp(pose.inputSource))
        {
            line.enabled = false;
            TryTeleport();
        }
    }

    private bool UpdatePointer()
    {
        if (!TeleportAction.GetState(pose.inputSource) && !TeleportAction.GetStateUp(pose.inputSource))
        {
            Pointer.SetActive(false);
            return false;
        }

        // Ray from the controller
        Ray ray = new Ray(transform.position, transform.forward);
        // if it is a hit
        if (Physics.Raycast(ray,out var hit, 42, ~IgnoreMask))
        {
            line.enabled = true;
            line.positionCount = 2;
            line.transform.position = transform.position;
            line.SetPositions(new Vector3[2] { transform.position, hit.point });

            Pointer.SetActive(true);
            Pointer.transform.position = hit.point;
            area = hit.collider.GetComponent<TeleportArea>();

            if (area == null)
            {
                Pointer.GetComponent<MeshRenderer>().material = onMissMaterial;
                line.material.SetInt("Hit", 0);
                return false;
            }
            line.material.SetInt("Hit", 1);
            Pointer.GetComponent<MeshRenderer>().material = onHitMaterial;
            return true;
        }

        //if not a hit
        Pointer.SetActive(false);
        return false;
    }

    private void TryTeleport()
    {
        //Check for valid position, and if already teleporting;
        if(!pointerHasPosition || isTeleporting)
        {
            return;
        }

        //Figure out transition
        destination = Pointer.transform.position;
        Vector3 translateVector = GetDestination();
        

        //Move
        switch (TeleportType)
        {
            case Type.Dash:
                distance = translateVector.magnitude;
                SetStartEndTransform();
                Player.GetComponent<BodyGravity>().enabled = false;
                angleDistance = Quaternion.Angle(startQuaternion, endQuaternion);
                StartCoroutine(DashTeleportPlayer());
                break;
            case Type.Fade:
                StartCoroutine(FleshTeleport(translateVector));
                break;
            default:
                break;
        }
        
    }

    private void SetStartEndTransform()
    {
        startPosition = Player.transform.position;
        endPosition = Pointer.transform.position + GroundCheckOffset;
        startQuaternion = Player.transform.rotation;
        endQuaternion = area.attractor.Attract(Player.transform, GroundCheck);
        isTeleporting = true;
    }

    private Vector3 GetDestination()
    {
        //rotate the player to find the right Translate vector
        Player.transform.rotation = area.attractor.Attract(Player.transform, GroundCheck);
        controller.HandleHeight();
        
        // get the translate Vector 
        Vector3 translateVector = Pointer.transform.position - GroundCheck.position;

        //set ground check offset with this rotation
        GroundCheckOffset = Player.transform.position - GroundCheck.transform.position;

        //rotate the player back
        Player.transform.rotation = Player.GetComponent<BodyGravity>().attractor.Attract(Player.transform, GroundCheck);
        controller.HandleHeight();

        return translateVector;
    }

    private IEnumerator DashTeleportPlayer()
    {
        // Flag
        isTeleporting = true;

        while (speed * teleportTimer / distance <= 1)
        {
            teleportTimer += Time.deltaTime;
            speed += Gravity * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0, 15);

            Player.transform.position = Vector3.Lerp(startPosition, endPosition, speed * teleportTimer / distance);
            Player.transform.rotation = Quaternion.Slerp(startQuaternion, endQuaternion, 200 * teleportTimer / angleDistance - 0.15f);

            yield return null;
        }

        speed = 0;
        teleportTimer = 0f;
        isTeleporting = false;
        Player.GetComponent<BodyGravity>().enabled = true;
        Player.GetComponent<VRCharacterController>().currentGravity = 0f;
        area.ChangeAtractor(Player);
        Player.transform.position = endPosition;

        //De-flag
        isTeleporting = false;
    }

    private IEnumerator FleshTeleport(Vector3 translation)
    {
        // Flag
        isTeleporting = true;

        // Fade
        SteamVR_Fade.View(Color.black, fadeTime);

        //Apply translation
        yield return new WaitForSeconds(fadeTime);
        Player.transform.position += translation;
        area.ChangeAtractor(Player);
        yield return new WaitForSeconds(fadeTime);
        translation = destination - GroundCheck.position;
        Player.transform.position += translation;

        // Fade to clear
        SteamVR_Fade.View(Color.clear, fadeTime);

        //De-flag
        isTeleporting = false;
    }


}
