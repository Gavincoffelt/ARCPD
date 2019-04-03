using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lap_Time : MonoBehaviour {
    // Textboxes For Laps and Time.
    public Text laptext;
    public Text time;

    VehiclePhysics Playerphysics;
    // Number Of Total Laps Playable.
    private int curlap;
    private int lastcurlap;
    private int totallap = 3;
   private float[] laptimes;
    // Timer Float.
    private float jimmy = 0;
    private GameObject player;
    void Start () {
        laptimes = new float[totallap];

        curlap = 1;
        lastcurlap = 1;

    }
    // Updates Time and Laps and Displays on Screen.
    void Update () {
       jimmy += 1 * Time.deltaTime;
            if (lastcurlap < curlap)
            {
            laptimes[lastcurlap] = jimmy;
                jimmy = 0f;
                Debug.Log(laptimes[lastcurlap]);
                lastcurlap = curlap;
            }
        Lapcount();
        time.text = jimmy.ToString("F3");
        
    }
    void Lapcount()
    {
        if (gameObject.tag == "Player")
        {
            player = GameObject.FindGameObjectWithTag("Player");
            Playerphysics = GetComponent<VehiclePhysics>() as VehiclePhysics;
        }


        if (gameObject.tag == "Player")
        {
             //curlap = Playerphysics.curlap;
            laptext.text = curlap.ToString() + "/" + totallap.ToString();
        }
    }
}
