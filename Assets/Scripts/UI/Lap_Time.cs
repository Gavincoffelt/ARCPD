using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lap_Time : MonoBehaviour
{
    // Textboxes For Laps and Time.
    public Text laptext;
    public Text time;
    public Text prevlaps;
    // Number Of Total Laps Playable.
    int curlap = 0;
    int lastcurlap;
    public int totallap;
    private float[] laptimes;
    // Timer Float.
    private float jimmy = 0;
    private GameObject player;
    float fastasfrick;
    private ButtonManager butt;
    void Start()
    {
        butt = GameObject.Find("Butooon").GetComponent<ButtonManager>();
        totallap = butt.laps;
        laptimes = new float[totallap + 2];
        curlap = 0;
        lastcurlap = 0;
    }
    // Updates Time and Laps and Displays on Screen.
    void Update()
    {
        if (curlap <= totallap)
        {
            Lapcount();
            
        }
        else
        {
            butt.Jimmy = jimmy;
            butt.FastAsFrick = fastasfrick; 
            SceneManager.LoadScene("EndScene", LoadSceneMode.Single);
        }
    }
    void Lapcount()
    {
        player = GameObject.Find("Checkpointmanager");
        curlap = player.GetComponent<Chekpointstuff>().curlap;
        laptext.text = curlap.ToString() + "/" + totallap.ToString();
        time.text = "Total:" + jimmy.ToString("F3");
        prevlaps.text = "Current " + laptimes[curlap].ToString("F3");
    
        if(curlap > 1)
        {
            prevlaps.text = "Current " + laptimes[curlap].ToString("F3") + " \n" + " Fastest " + fastasfrick.ToString("F3");
        }
        if (curlap > 0)
        {
                jimmy += 1 * Time.deltaTime;
                laptimes[curlap] += 1 * Time.deltaTime;
        }

            if (lastcurlap < curlap)
            {
                laptimes[lastcurlap] = laptimes[curlap - 1];
                Debug.Log(laptimes[lastcurlap]);
                lastcurlap = curlap;
                CheckForFastLap();
            }
    }
    public void CheckForFastLap()
    {
        fastasfrick = Mathf.Max(laptimes);
        for (int i = 1; i <= totallap; i++)

        {
            if (laptimes[i] < fastasfrick && laptimes[i] > 1)
            {
                fastasfrick = laptimes[i];
            }
        }
    }
}