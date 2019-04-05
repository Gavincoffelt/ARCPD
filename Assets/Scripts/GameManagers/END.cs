using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class END : MonoBehaviour {
    private Text jimmy;
    private Text fastasfrick;
    private ButtonManager butt;
    // Use this for initialization
    void Start ()
    {
        jimmy = GameObject.Find("ttime").GetComponent<Text>();
        fastasfrick = GameObject.Find("btime").GetComponent<Text>();
        butt = GameObject.Find("Butooon").GetComponent<ButtonManager>();

        jimmy.text = butt.Jimmy.ToString("F3");
        fastasfrick.text = butt.FastAsFrick.ToString("F3");
	}
	
	
}
