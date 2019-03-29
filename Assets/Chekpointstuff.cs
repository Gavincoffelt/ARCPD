using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chekpointstuff : MonoBehaviour {
    GameObject Checkpoint1;
    GameObject Checkpoint2;
    float billy;

	void Start () {
        Checkpoint1 = GameObject.Find("Check1");
        Checkpoint2 = GameObject.Find("Check2");
        billy = 0.0f;
        Checkpoint1.SetActive(false);
    }

    void Update () {
        if (Checkpoint1.activeInHierarchy == false && Checkpoint2.activeInHierarchy == false)
        {
            CollisionCheck();
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
        }
    }
}
