using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gyro : MonoBehaviour {
    VehiclePhysics PlayerPhysics;
    private GameObject player;
    private GameObject gas;
    private GameObject reverse;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        PlayerPhysics = player.GetComponent<VehiclePhysics>();
        gas = GameObject.Find("Gas");
        reverse = GameObject.Find("Reverse");

        Input.gyro.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
        //player.transform.Rotate(0,Input.acceleration.x, 0);
        PlayerPhysics.MovementTurn = Input.acceleration.x * 100;

    }
}
