using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lap_Time : MonoBehaviour {
    public Text laptext;
    public Text time;

    int curlap = 1;
    int totallap = 5;

    float jimmy = 0;
    void Start () {
        time.text = "0";
        laptext.text = "0";
	}
	
	void Update () {
        jimmy += 1 * Time.deltaTime;

        time.text = jimmy.ToString("F3");
        laptext.text = (curlap.ToString() + "/" + totallap.ToString());
    }
}
