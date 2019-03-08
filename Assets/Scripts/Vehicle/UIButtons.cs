using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtons : MonoBehaviour
{
    GameObject Player;
    VehiclePhysics PlayerPhysics;

    public GameObject Stick;

    bool GasPressed;
    bool ReversePressed;
    int Speed = 1;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerPhysics = Player.GetComponent<VehiclePhysics>();
        Stick = GameObject.FindGameObjectWithTag("JoyStick");
    }

    void Update()
    {
        if (GasPressed || ReversePressed)
        {
            PlayerPhysics.MovementDirection += Speed * Time.deltaTime;
        }
        else if(PlayerPhysics.MovementDirection != 0)
        {
            PlayerPhysics.MovementDirection -= Mathf.Lerp(PlayerPhysics.MovementDirection, 0, 0.01f) * Time.deltaTime;
            if (Mathf.Abs(PlayerPhysics.MovementDirection) < 0.1f)
            {
                PlayerPhysics.MovementDirection = 0;
            }
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
        PlayerPhysics.MovementTurn = Stick.transform.localPosition.x - 50;
    }
    public void ResetJoystick(BaseEventData e)
    {
        PointerEventData Data = e as PointerEventData;
        PlayerPhysics.MovementTurn = 0;
        Stick.transform.localPosition = Vector3.zero;
    }

}
