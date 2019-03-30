using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public int VehicleType;
    public bool SelectionScreen;
    public static GameManager Instance = null;

    void Start ()
    {
        DontDestroyOnLoad(this);

        //Singleton
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != null)
        {
            Destroy(this);
        }

	}
	

	void Update ()
    {

	}
}
