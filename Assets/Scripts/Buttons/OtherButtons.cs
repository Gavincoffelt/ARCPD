using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OtherButtons : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Next()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void Back()
    {
        SceneManager.LoadScene("TestMenu", LoadSceneMode.Single);
    }
}
