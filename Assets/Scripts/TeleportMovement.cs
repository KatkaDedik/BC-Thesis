using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Assets.Scripts;

[RequireComponent(typeof(LineRenderer))]
public class TeleportMovement : MonoBehaviour
{
    public GameObject Player;
    public GameObject Pointer;
    public SteamVR_Action_Boolean TeleportAction;
    public Transform GroundCheck;
    public LayerMask IgnoreMask;
    public bool SmoothTeleport = false;
    public Material onHitMaterial;
    public Material onMissMaterial;
    public float Gravity = 9.82f;

    private SteamVR_Behaviour_Pose pose = null;
    private bool pointerHasPosition = false;
    private bool isTeleporting = false;
    private float fadeTime = 0.5f;
    private TeleportArea area;

    private float teleportTimer = 0f;
    private Vector3 endPosition = Vector3.zero;
    private Vector3 startPosition = Vector3.zero;
    private Quaternion startQuaternion;
    private Quaternion endQuaternion;
    private float distance = 1f;
    private float angleDistance;
    private float speed = 0f;
    private LineRenderer line;

    private void Awake()
    {
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        Pointer.SetActive(false);
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        //Pointer
        pointerHasPosition = UpdatePointer();

        if (SmoothTeleport && isTeleporting)
        {
            SmoothTeleportPlayer();
            return;
        }

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
        Vector3 translateVector = Pointer.transform.position - GroundCheck.position;

        //Move
        if (SmoothTeleport)
        {
            distance = translateVector.magnitude;
            SetStartEndTransform();
            Player.GetComponent<BodyGravity>().enabled = false;
            angleDistance = Quaternion.Angle(startQuaternion, endQuaternion);
            SmoothTeleportPlayer();
        }
        else
        {
            StartCoroutine(FleshTeleportPlayer(translateVector));
        }
    }

    private void SetStartEndTransform()
    {
        startPosition = Player.transform.position;
        endPosition = Pointer.transform.position;
        startQuaternion = Player.transform.rotation;
        endQuaternion = area.atractor.Attract(Player.transform, GroundCheck);
        isTeleporting = true;
    }

    private void SmoothTeleportPlayer()
    {
        teleportTimer += Time.deltaTime;
        speed += Gravity * Time.deltaTime;
        speed = Mathf.Clamp(speed, 0, 15);

        if (speed * teleportTimer / distance > 1) 
        {
            speed = 0;
            teleportTimer = 0f;
            isTeleporting = false;
            Player.GetComponent<BodyGravity>().enabled = true;
            Player.GetComponent<VRCharacterController>().currentGravity = 0f;
            area.ChangeAtractor(Player);
            Player.transform.position = endPosition;
            return;
        }

        Player.transform.position = Vector3.Lerp(startPosition, endPosition, speed * teleportTimer / distance);
        Player.transform.rotation = Quaternion.Slerp(startQuaternion, endQuaternion, 200 * teleportTimer / angleDistance - 0.15f);
    }

    private IEnumerator FleshTeleportPlayer(Vector3 translation)
    {
        // Flag
        isTeleporting = true;

        // Fade
        SteamVR_Fade.Start(Color.black, fadeTime, true);

        //Apply translation
        yield return new WaitForSeconds(fadeTime);
        Player.transform.position += translation;
        area.ChangeAtractor(Player);

        // Fade to clear
        SteamVR_Fade.Start(Color.clear, fadeTime, true);

        //De-flag
        isTeleporting = false;
    }


}
