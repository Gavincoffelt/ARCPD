using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiltstuff : MonoBehaviour {    
	// Updates Vehicle Rotation to allow tilt controls.
	void Update () {
        transform.Rotate(0,Input.acceleration.x * 2, 0);
    }
}
