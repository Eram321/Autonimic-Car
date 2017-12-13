using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : MonoBehaviour {

    //Car Fuzzy control logic
    Fuzzy fuzzy;
    CarView carView;

    RaycastHit hit;

    //Car Movement Physic
    [SerializeField] float maxCarSpeed = 50f;
    [SerializeField] float maxRotationSpeed = 100f;

    [SerializeField] float carSpeedValue = 0f;
    [SerializeField] float carTurnValue = 0f;

    Rigidbody rigibody;

    public bool stop = false;

    public void SetRotationSpeed(float h){
        carTurnValue = h;
    }

    public void SetSpeed(float v){
        carSpeedValue = v;
    }

    private void Start(){
        rigibody = GetComponent<Rigidbody> ();
        fuzzy = GetComponentInChildren<Fuzzy> ();
        carView = GetComponent<CarView> ();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        fuzzy.SetDistances (carView.GetLeft(), carView.GetRight (), carView.GetCenter ());

        float h = carTurnValue*2;
        float v = carSpeedValue;

        Move (h, v);
    }

    void Move(float horizontal, float vertical)
    {
        Vector3 rotationVector = new Vector3 (0, horizontal, 0);
        rotationVector *= Time.deltaTime * maxRotationSpeed;

        Quaternion deltaRotation = Quaternion.Euler (rotationVector);
        rigibody.MoveRotation (rigibody.rotation  * deltaRotation);

        Vector3 moveVector = (vertical) * transform.forward;
        moveVector *= Time.deltaTime * maxCarSpeed;

        moveVector += transform.position;
        rigibody.MovePosition (moveVector);
    }
}
