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
        public bool isReverse = false;
        public float GravityForce = -9.81f;
        public Vector3 GravityOrientation;
        public bool freezeX = false;
        public bool freezeY = false;
        public bool freezeZ = false;
        public Transform center = null;


        public Quaternion Attract(Transform player, Transform groundCheck)
        {

            Vector3 gravityUp;

            if (isFloor)
            {
                gravityUp = GravityOrientation;
            }
            else
            {   
                gravityUp = -(groundCheck.position -  center.position);

                if (isReverse)
                {
                    gravityUp *= -1;
                }

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
            Vector3 bodyUp = player.up;

            return Quaternion.FromToRotation(bodyUp, gravityUp) * player.rotation;
        }

    }

}