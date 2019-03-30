using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{
    #region PlayerData
    GameObject Player;
    VehiclePhysics PlayerPhysics;
    #endregion

    #region NotNeeded
    public GameObject Stick;
    #endregion

    #region Movement Checks
    bool GasPressed;
    bool ReversePressed;
    #endregion

    #region Modifiers
    int Speed = 1;
    #endregion

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerPhysics = Player.GetComponent<VehiclePhysics>();
        Stick = GameObject.FindGameObjectWithTag("JoyStick");
    }

    void Update()
    {
        if(!PlayerPhysics)
        {
            Debug.LogError("The Cube returns!");
        }


        if (PlayerPhysics.Grounded && ((GasPressed && !PlayerPhysics.CrashedFromFront) || (ReversePressed && !PlayerPhysics.CrashedFromBack)))
        {
            PlayerPhysics.MovementDirection += Speed * Time.deltaTime;
        }
        else if(PlayerPhysics.MovementDirection != 0 && PlayerPhysics.Grounded)
        {
            PlayerPhysics.SlowVehicleDown();
        }
    }

    public void GasButton()
    {
        GasPressed = true;
        Speed = 1;
    }
    public void ReverseButton()
    {
        ReversePressed = true;
        Speed = -1;
    }
    public void NonGasButton()
    {
        GasPressed = false;
    }
    public void NonReverseButton()
    {
        ReversePressed = false;
    }


    //JoyStick
    public void MoveJoystick(BaseEventData e)
    {
        PointerEventData Data = e as PointerEventData;
        Stick.transform.position = Data.position;
        PlayerPhysics.MovementTurn = Stick.transform.localPosition.x - 100;
    }
    public void ResetJoystick(BaseEventData e)
    {
        PlayerPhysics.MovementTurn = 0;
        float temp = Stick.transform.parent.GetComponent<RectTransform>().rect.width / 2;
        Stick.transform.localPosition = new Vector3(temp,temp,temp);
    }

}
