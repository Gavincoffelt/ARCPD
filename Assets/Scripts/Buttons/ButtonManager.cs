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
    public GameObject Option5;
    public GameObject Selection;
    public string MapName;

    //private Image bob;
    //public Sprite Img1;
    //public Sprite Img2;
    //public Sprite Img3;
    //public Sprite Img4;
    //public Sprite Img5;




    public bool ARTog = false;
    public bool GYTog = false;
    private string Map;
    private Dropdown fun;
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
    void Start ()
    {
       
        
        fun = GameObject.Find("MapComboBox").GetComponent<Dropdown>();
        //bob = GameObject.Find("MapImage").GetComponent<Image>();
        //bob.sprite = Img1;
    }

    public void StartGame()
    {
       // ChangeSelected();
        SceneManager.LoadScene("MapSelection", LoadSceneMode.Single);        
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
                case 3:
                    Selection = Option4;
                    break;
                case 4:
                    Selection = Option5;
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
                    Map = "PlayGround";

                    break;
                case 1:
                    Map = "Basic";
                    break;
                case 2:
                    Map = "Circut";
                    break;
                case 3:
                    Map = "Jumps";
                    break;
                case 4:
                    Map = "Figure8";
                    break;
                default:
                    Map = "PlayGround";
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
