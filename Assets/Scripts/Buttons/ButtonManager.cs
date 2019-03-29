using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {
    public GameObject Option1;
    public GameObject Option2;
    public GameObject Option3;


    public GameObject Selection;
    public bool ARTog = false;
    private string Map;
    private Dropdown fun;
    
    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(this);
        fun = GameObject.Find("MapComboBox").GetComponent<Dropdown>();
	}

    public void StartGame()
    {
        ChangeSelected();
        SceneManager.LoadScene(Map, LoadSceneMode.Single);        
    }
    public void ChangeSelected()
    {
        if (ARTog)
        {
            switch (fun.value)
            {
                case 0:
                    Selection = Option1;
                    break;
                case 1:
                    Selection = Option2;
                    break;
                case 2:
                    Selection = Option3;
                    break;
                default:
                    Selection = Option1;
                    break;
            }
            Map = "ARcore rewrite";
        }
        else
        {
            switch (fun.value)
            {
                case 0:
                    Map = "Main";
                    break;
                case 1:
                    Map = "GavinTestScene";
                    break;
                case 2:
                    Map = "ARcore rewrite";
                    break;
                default:
                    Map = "Main";
                    break;
            }
        }
    }

    public void KILLLLLLLLLLLLLL()
    {
        Application.Quit();
    }

    public void AR()
    {
        if (!ARTog)
        {
            ARTog = true;
        }
        else
        {
            ARTog = false;
        }
    }
}
