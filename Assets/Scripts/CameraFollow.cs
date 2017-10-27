using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
	public class CameraFollow : MonoBehaviour {

		[SerializeField] bool followPlayer = false; // may I follow player ?

        private GameObject player; // Reference to player

		void Start(){
			player = GameObject.FindGameObjectWithTag ("Player"); // Find player
		}

		void Update(){

            if (followPlayer)
				this.transform.position = player.transform.position;

		}
	}

}