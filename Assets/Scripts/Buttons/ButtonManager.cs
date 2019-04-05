using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour {
    public static ButtonManager Instance = null;

    public GameObject Option1;
    public GameObject Option2;
    public GameObject Option3;
    public GameObject Option4;

    public GameObject Selection;

    public string MapName;
    
    public bool ARTog = false;
    public bool GYTog = false;

   
    private Toggle Tog;

    // Use this for initialization
    private void Awake()
    {
         #region Singleton
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
        #endregion
    }
   
    public void StartGame()
    {
        SceneManager.LoadScene("MapSelection", LoadSceneMode.Single);        
    }    

    public void KILLLLLLLLLLLLLL()
    {
        Application.Quit();
    }

    public void AR()
    {
        ARTog = !ARTog;

        if (ARTog)
        {
            Tog = GameObject.Find("GyroToggle").GetComponent<Toggle>();
            Tog.isOn = false;
        }
    }
    public void GY()
    {
        GYTog = !GYTog;

        if (GYTog)
        {
            Tog = GameObject.Find("ARToggle").GetComponent<Toggle>();
            Tog.isOn = false;
        }
    }
}
