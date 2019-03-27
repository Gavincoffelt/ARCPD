using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSpiral : MonoBehaviour
{
    public GameObject SpiralPart;
    public int Location;
	void Start ()
    {
        if (SpiralPart)
            switch (Location)
            {
                case 0:
                    for (int i = 0; i < 92; i++)
                    {
                        GameObject temp = Instantiate(SpiralPart);
                        temp.transform.parent = transform;
                        temp.name = "Clone:" + i;

                        temp.transform.localPosition = new Vector3(-1.834f, -2.587f, i * temp.transform.localScale.z + 48.845f);
                        temp.transform.Rotate(new Vector3(0, 0, i * 4));
                    }
                    break;
                case 1:
                    for (int i = 0; i < 92; i++)
                    {
                        GameObject temp = Instantiate(SpiralPart);
                        temp.transform.parent = transform;
                        temp.name = "Clone:" + i;

                        temp.transform.localPosition = new Vector3(-1.818f, 0.45f, i * -temp.transform.localScale.z + 87.6188f);
                        temp.transform.eulerAngles = new Vector3(179, temp.transform.eulerAngles.y, temp.transform.eulerAngles.z);
                        temp.transform.Rotate(new Vector3(0, 0, i * 2));
                    }
                    break;
            }
            
    }
	
	// Update is called once per frame
	void Update ()
    {
        //transform.Rotate(new Vector3(0,0,Time.deltaTime * 2));
	}
}
