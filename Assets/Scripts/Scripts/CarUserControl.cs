using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{

    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        Fuzzy fuzzy;
        public GameObject leftRay;
        public GameObject rightRay;
        public GameObject centerRay;

        public double turnRate;
        public double speedRate;
        public double breakRate;

        private void Awake()
        {
            fuzzy = GameObject.FindObjectOfType<Fuzzy> ();

            // get the car controller
            m_Car = GetComponent<CarController> ();
        }

        float distance = 100;
        float leftDistance;
        float rightDistance;
        float centerDistance;

        RaycastHit hit;


        private void FixedUpdate()
        {
            int layerMask = 1 << 8;
            if (Physics.Raycast (leftRay.transform.position, leftRay.transform.TransformDirection(Vector3.forward), out hit, distance, layerMask))
            {
                leftDistance = hit.distance;
            }
            else
                leftDistance = 100;

            if (Physics.Raycast (rightRay.transform.position, rightRay.transform.TransformDirection(Vector3.forward), out hit, distance, layerMask))
            {
                rightDistance = hit.distance;
            }
            else
                rightDistance = 100;

            if (Physics.Raycast(centerRay.transform.position, centerRay.transform.TransformDirection(Vector3.forward), out hit, distance, layerMask))
            {
                centerDistance = hit.distance;
            }
            else
                centerDistance = 100;

            fuzzy.SetDistances (leftDistance, rightDistance,centerDistance);
            fuzzy.CarSpeed(m_Car.getSpeed());

            // pass the input to the car!
            float h = (float)turnRate;
            float v = (float)speedRate;
            float b=(float)breakRate;
            float handbrake = CrossPlatformInputManager.GetAxis ("Jump");
            m_Car.Move(h, 1f, 0f, 0f);

        }


        private void OnDrawGizmos()
        {
            float distance = 100;

            Gizmos.DrawRay (rightRay.transform.position, rightRay.transform.TransformDirection(Vector3.forward) * distance);
            Gizmos.DrawRay (leftRay.transform.position, leftRay.transform.TransformDirection(Vector3.forward) * distance);
            Gizmos.DrawRay(centerRay.transform.position, centerRay.transform.TransformDirection(Vector3.forward) * distance);

        }

    }
}
