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

    private Dropdown fun;
    
    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(this);
        fun = GameObject.Find("MapComboBox").GetComponent<Dropdown>();
	}

    public void StartGame()
    {
        SceneManager.LoadScene("ARcore rewrite", LoadSceneMode.Single);        
    }
    public void ChangeSelected()
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
    }




}
