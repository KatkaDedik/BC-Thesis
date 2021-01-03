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
        

        public void Attract(Transform body)
        {
            
            Vector3 gravityUp = -(body.position - transform.position);

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

            gravityUp.Normalize();
            if (isFloor)
            {
                gravityUp = GravityOrientation;
            }
            Vector3 bodyUp = body.up;

            Rigidbody rb = body.GetComponent<Rigidbody>();

            rb.AddForce(gravityUp * GravityForce);

            Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
            body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50 * Time.deltaTime);
        }

    }

}