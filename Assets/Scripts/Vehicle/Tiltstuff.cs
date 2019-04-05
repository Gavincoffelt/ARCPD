using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiltstuff : MonoBehaviour {
    private ButtonManager butt;
    // Updates Vehicle Rotation to allow tilt controls.
    private void Start()
    {
        butt = GameObject.Find("Butooon").GetComponent<ButtonManager>();
    }
    void Update ()
    {
        if (butt.GYTog)
        {
            transform.Rotate(0, Input.acceleration.x * 2, 0);
        }
    }
}
