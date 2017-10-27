using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : MonoBehaviour {

    //Car Fuzzy control logic
    Fuzzy fuzzy;
    public GameObject leftRay;
    public GameObject rightRay;
    public GameObject centerRay;

    float distance = 100;
    float leftDistance;
    float rightDistance;
    float centerDistance;

    RaycastHit hit;

    //Car Movement Physic
    [SerializeField] float maxCarSpeed = 50f;
    [SerializeField] float minCarSpeed = 20f;
    [SerializeField] float maxRotationSpeed = 100f;

    [SerializeField] float carSpeedValue = 10f;
    [SerializeField] float carTurnValue = 1f;

    Rigidbody rigibody;

    public void SetRotationSpeed(float h){
        carTurnValue = h;
    }

    public void SetSpeed(float v){

        if (v < 20f)
            carSpeedValue = minCarSpeed / 150;
        else
            carSpeedValue = v/150;
    }

    private void Start(){
        rigibody = GetComponent<Rigidbody> ();
        fuzzy = GameObject.FindObjectOfType<Fuzzy> ();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetDistances ();

        fuzzy.SetDistances (leftDistance, rightDistance, centerDistance);

        float h = carTurnValue;
        float v = carSpeedValue;

        Move (h, v);
    }

    void Move(float horizontal, float vertical)
    {
        Vector3 rotationVector = new Vector3 (0, horizontal, 0);
        rotationVector *= Time.deltaTime * maxRotationSpeed;

        Quaternion deltaRotation = Quaternion.Euler (rotationVector);
        rigibody.MoveRotation (rigibody.rotation * deltaRotation);

        Vector3 moveVector = (vertical) * transform.forward;
        moveVector *= Time.deltaTime * maxCarSpeed;

        moveVector += transform.position;
        rigibody.MovePosition (moveVector);
    }

    private void SetDistances()
    {
        int layerMask = 1 << 8;
        if (Physics.Raycast (leftRay.transform.position, leftRay.transform.TransformDirection (Vector3.forward), out hit, distance, layerMask))
        {
            leftDistance = hit.distance;
        }
        else
            leftDistance = 100;

        if (Physics.Raycast (rightRay.transform.position, rightRay.transform.TransformDirection (Vector3.forward), out hit, distance, layerMask))
        {
            rightDistance = hit.distance;
        }
        else
            rightDistance = 100;

        if (Physics.Raycast (centerRay.transform.position, centerRay.transform.TransformDirection (Vector3.forward), out hit, distance, layerMask))
        {
            centerDistance = hit.distance;
        }
        else
            centerDistance = 100;
    }
}
