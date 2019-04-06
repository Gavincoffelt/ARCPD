using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class OtherButtons : MonoBehaviour {
    private ButtonManager butt;
    private Dropdown fun;
    private bool funisfun = false;
    // Use this for initialization
    void Start () {
        butt = GameObject.Find("Butooon").GetComponent<ButtonManager>();
        if (GameObject.Find("LapComboBox") == null)
        {
            funisfun = false;
        }
        else
        {
            fun = GameObject.Find("LapComboBox").GetComponent<Dropdown>();
            funisfun = true;
        }
    }
	
    public void Next()
    {
        if (funisfun)
        {
        butt.laps = (fun.value + 1);
        }
        SceneManager.LoadScene("VehicleSelection", LoadSceneMode.Single);
    }
    
    public void Back()
    {
        ButtonManager.Instance = null;
        Destroy(GameObject.Find("Butooon"));
        SceneManager.LoadScene("TestMenu", LoadSceneMode.Single);
    }

    public void Select()
    {
        if (butt.ARTog)
        {
            SceneManager.LoadScene("ARcore rewrite", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(butt.MapName, LoadSceneMode.Single);
        }
    }
}
