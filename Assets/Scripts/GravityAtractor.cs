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
        public bool isConvex = false;
        public float GravityForce = -9.81f;
        public Vector3 GravityOrientation;
        public bool freezeX = false;
        public bool freezeY = false;
        public bool freezeZ = false;
        public Transform GravityHelper = null;
        public Vector3 Center = Vector3.zero;


        public Quaternion Attract(Transform player, Transform groundCheck)
        {

            Vector3 gravityUp;

            if (isFloor)
            {
                gravityUp = GravityOrientation;
            }
            else
            {
                GravityHelper.position = groundCheck.position;
                var tmp = GravityHelper.localPosition;
                
                if (isConvex)
                {
                    tmp.y = Mathf.Clamp(tmp.y, -Mathf.Infinity, Center.y);
                    tmp.x = Mathf.Clamp(tmp.x, Center.x, Mathf.Infinity);
                    GravityHelper.localPosition = tmp;
                    gravityUp = (GravityHelper.position - transform.TransformPoint(Center));
                }
                else
                {
                    tmp.y = Mathf.Clamp(tmp.y, Center.y, Mathf.Infinity);
                    tmp.x = Mathf.Clamp(tmp.x, -Mathf.Infinity, Center.x);
                    GravityHelper.localPosition = tmp;
                    gravityUp = -(GravityHelper.position - transform.TransformPoint(Center));
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
            return Quaternion.FromToRotation(player.up, gravityUp) * player.rotation;
        }

    }

    

}