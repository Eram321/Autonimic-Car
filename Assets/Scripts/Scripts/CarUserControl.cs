using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{

    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use

        private CarView carView;

        Fuzzy fuzzy;

        [SerializeField] float carSpeedValue = 0f;
        [SerializeField] float carTurnValue = 0f;


        public void SetRotationSpeed(float h)
        {
            carTurnValue = h;
        }

        public void SetSpeed(float v)
        {
            carSpeedValue = (v / 150) + 0.2f;
        }


        private void Awake()
        {
            fuzzy = GameObject.FindObjectOfType<Fuzzy> ();

            // get the car controller
            m_Car = GetComponent<CarController> ();
            carView = GetComponent<CarView> ();
        }

        private void FixedUpdate()
        {

            fuzzy.SetDistances (carView.GetLeft (), carView.GetRight (), carView.GetCenter ());
            
            // pass the input to the car!
            float h = (float)carTurnValue;
            float v = (float)carSpeedValue;
            m_Car.Move(h, v, 0f, 0f);

        }
    }
}
