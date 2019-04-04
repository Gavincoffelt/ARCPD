using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapRotation : MonoBehaviour {

    public GameObject Option1;
    public GameObject Option2;
    public GameObject Option3;
    public GameObject Option4;
    //public GameObject Option5;
    private GameObject Current;

    private Dropdown fun;
    // Use this for initialization
    void Start ()
    {
        fun = GameObject.Find("MapComboBox").GetComponent<Dropdown>();
        Current = Option1;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Current.transform.Rotate(0,1,0);
	}

   public void Changebots()
    {
        Current.SetActive(false);
        Debug.Log(Current.activeSelf);
        switch (fun.value)
        {
            case 0:
                Current = Option1;
                break;
            case 1:
                Current = Option2;
                break;
            case 2:
                Current = Option3;
                break;
            case 3:
                Current = Option4;
                break;
            //case 4:
            //    Current = Option5;
            //    break;
            default:
                Current = Option1;
                break;
        }
        Current.SetActive(true);
    }
}
