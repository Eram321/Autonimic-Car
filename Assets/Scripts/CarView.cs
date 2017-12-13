using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarView : MonoBehaviour {

    public Transform leftRaycast;
    public Transform centerRaycast;
    public Transform rightRaycast;


    public List<float> leftRayDistances;
    public List<float> rightRaysDistances;
    public List<float> centerRaysDistances;

    public float distance = 30f;
    public float raycastsGap = 1f;

    Car car;
    RaycastHit hit;
    int layerMask;
    public int raycastAmount;

    float startLeftAngle = 300f;
    float startCenterAngle = 340f;
    float startRightAngle = 380f;

    float minLeft;
    float minRight;
    float minCenter;

    public void Start()
    {
        layerMask = 1 << 8;
        raycastAmount = (int)(40 * (1/raycastsGap));

        startLeftAngle = leftRaycast.rotation.eulerAngles.y;
        startCenterAngle = centerRaycast.rotation.eulerAngles.y;
        startRightAngle = rightRaycast.rotation.eulerAngles.y;


        car = GetComponent<Car> ();
    }

    public float GetLeft()
    {
        return minLeft;
    }

    public float GetRight()
    {
        return minRight;
    }

    public float GetCenter()
    {
        return minCenter;
    }

    public void Update()
    {
        SetupRaycastDistances ();

        minLeft = 25f; // for fuzzy logic 100 as max value
        if(leftRayDistances.Count > 0)
           minLeft = leftRayDistances.Average ();
            

        minCenter = 25f; // for fuzzy logic 100 as max value
        if (centerRaysDistances.Count > 0)
           minCenter = centerRaysDistances.Average ();

            

        minRight = 25f; // for fuzzy logic 100 as max value
        if (rightRaysDistances.Count > 0)
           minRight = rightRaysDistances.Average ();

    }

    private void SetupRaycastDistances()
    {
        leftRayDistances.Clear ();
        centerRaysDistances.Clear ();
        rightRaysDistances.Clear ();
        for (int i = 0; i < raycastAmount; i++)
        {
            leftRaycast.localRotation = Quaternion.Euler (new Vector3 (0, startLeftAngle + raycastsGap * i, 0));
            if (Physics.Raycast (leftRaycast.position, leftRaycast.forward, out hit, distance, layerMask))
            {
                leftRayDistances.Add (hit.distance);
            }

            centerRaycast.localRotation = Quaternion.Euler (new Vector3 (0, startCenterAngle + raycastsGap * i, 0));
            if (Physics.Raycast (centerRaycast.position, centerRaycast.forward, out hit, distance, layerMask))
            {
                centerRaysDistances.Add (hit.distance);
            }

            rightRaycast.localRotation = Quaternion.Euler (new Vector3 (0, startRightAngle + raycastsGap * i, 0));
            if (Physics.Raycast (rightRaycast.position, rightRaycast.forward, out hit, distance, layerMask))
            {
                rightRaysDistances.Add (hit.distance);
            }
        }
    }

    //------------ DEBUG -------------//
    public Transform DebugLeftRay;
    public Transform DebugCenterRay;
    public Transform DebugRightRay;
    private void OnDrawGizmos()
    {
        raycastAmount = (int)(40 * (1 / raycastsGap));
        for (int i = 0; i < raycastAmount; i++)
        {
            DebugLeftRay.localRotation = Quaternion.Euler (new Vector3 (0, startLeftAngle + raycastsGap * i, 0));
            DebugCenterRay.localRotation = Quaternion.Euler (new Vector3 (0, startCenterAngle + raycastsGap * i, 0));
            DebugRightRay.localRotation = Quaternion.Euler (new Vector3 (0, startRightAngle + raycastsGap * i, 0));

            Gizmos.color = Color.red;
            Gizmos.DrawRay (DebugLeftRay.position, DebugLeftRay.forward * distance);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay (DebugCenterRay.position, DebugCenterRay.forward * distance);

            Gizmos.color = Color.green;
            Gizmos.DrawRay (DebugRightRay.position, DebugRightRay.forward * distance);

        }

    }



}
