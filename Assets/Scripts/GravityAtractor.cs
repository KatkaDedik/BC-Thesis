using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{

    public class GravityAtractor : MonoBehaviour
    {
        public bool isFloor = true;
        public float GravityForce = -9.81f;
        public Vector3 GravityOrientation;
        public bool freezeX = false;
        public bool freezeY = false;
        public bool freezeZ = false;
        public Transform center = null;


        public void Attract(Transform cameraRig, Transform groundCheck)
        {

            Vector3 gravityUp;
            Vector3 moveCameraRig = groundCheck.localPosition;

            if (isFloor)
            {
                gravityUp = GravityOrientation;
            }
            else
            {
                gravityUp = -(groundCheck.position -  center.position);
                if (freezeX)
                {
                    gravityUp.x = 0;
                }
                if (freezeY)
                {
                    gravityUp.y = 0;
                }
                if (freezeZ)
                {
                    gravityUp.z = 0;
                }
            }

            gravityUp.Normalize();
            Vector3 bodyUp = cameraRig.up;

            //Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
            //body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50 * Time.deltaTime);
            cameraRig.rotation = Quaternion.FromToRotation(bodyUp, gravityUp) * cameraRig.rotation;
            if (!isFloor)
            {
                //cameraRig.position += moveCameraRig;

                //cameraRig.position -= Quaternion.FromToRotation(bodyUp, gravityUp) * moveCameraRig;
            }
            
        }

    }

}