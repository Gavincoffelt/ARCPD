using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OtherButtons : MonoBehaviour {
    private ButtonManager butt;
    // Use this for initialization
    void Start () {
        butt = GameObject.Find("Butooon").GetComponent<ButtonManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Next()
    {
        SceneManager.LoadScene("VehicleSelection", LoadSceneMode.Single);
    }

    public void Back()
    {
        SceneManager.LoadScene("TestMenu", LoadSceneMode.Single);
    }

    public void Select()
    {
        SceneManager.LoadScene(butt.MapName, LoadSceneMode.Single);
    }
}
