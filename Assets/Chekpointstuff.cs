using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chekpointstuff : MonoBehaviour {
    GameObject Checkpoint1;
    GameObject Checkpoint2;
    VehiclePhysics hitcheck;
    int Lapcounter;
    bool Lapchange = true;
    float billy;
    public int curlap = 0;
    void Start ()
    {
        curlap = 0;
        billy = 0.0f;

        Checkpoint1 = GameObject.Find("Start");
        Checkpoint2 = GameObject.Find("Check2");
        hitcheck = GameObject.FindGameObjectWithTag("Player").GetComponent<VehiclePhysics>();
        Lapcounter = GameObject.FindGameObjectWithTag("Player").GetComponent<Lap_Time>().totallap;
    }

    void Update ()
    {
        if (hitcheck.HitCheckpoint() == Checkpoint1)
        {
            Checkpoint1.SetActive(false);
        }
        if (hitcheck.HitCheckpoint() == Checkpoint2)
        {
            Checkpoint2.SetActive(false);
        }
        if (curlap < Lapcounter +1)
        {
            if (Checkpoint1.activeInHierarchy == false && Checkpoint2.activeInHierarchy == false)
            {
                CollisionCheck();

            }
            if (Checkpoint1.activeInHierarchy == false && Checkpoint2.activeInHierarchy == true)
            {

                if (Lapchange == true) { curlap = (curlap + 1); Lapchange = false; }
            }

        } 
    }
    void CollisionCheck()
    {
        billy += Time.deltaTime;

        if (billy >= 5)
        {
            Checkpoint1.SetActive(true);
            Checkpoint2.SetActive(true);
            Debug.Log("Checkpoints Reset");
            Lapchange = true;
            billy = 0.0f;
        }
    }
}
