using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kolizja : MonoBehaviour {


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
            Time.timeScale = 0;
    }
}
