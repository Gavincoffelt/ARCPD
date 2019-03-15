﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public class VehiclePhysics : MonoBehaviour
{
    public float GravityModifier = 1;
    GameObject Respawn;

    [Header("Car Mechanics")]
    float Direction = 0;
    public float MovementDirection
    {
        set
        {
            Direction = Mathf.Clamp(value, -1, 1);
        }
        get
        {
            return Direction;
        }
    }
    float Turn = 0;
    public float MovementTurn
    {
        set
        {
            Turn = Mathf.Clamp(value, -100, 100);
        }
        get
        {
            return Turn;
        }
    }
    public float Speed;

    [Header("Car Physics")]
    public float HoverAmount;
    RaycastHit hit;
    RaycastHit[] CornersHit = new RaycastHit[4];
    bool[] BoolCornerHit = new bool[4];
    Vector3 ForwardVector;
    public bool Grounded;

    public bool CrashedFromFront;
    public bool CrashedFromBack;

    [Header("Car Children")]
    public GameObject MeshOfVehicle;

    [Header("Animation")]
    Animator MyAnim;
    enum ANIMATESTATES { IDLE, LeftTurn, RightTurn};


    [Header("Temp")]
    GameObject MainCamera;
    GameObject CameraLocation;

    void Start ()
    {
        MainCamera = GameObject.Find("Camera");
        CameraLocation = GameObject.Find("CameraMoveTo");
        Respawn = GameObject.Find("Respawn");


        Grounded = false;
        CrashedFromFront = false;
        CrashedFromBack = false;
        MyAnim = GetComponent<Animator>();
        if(!MeshOfVehicle)
        {
            if (!(gameObject.GetComponentInChildren<Transform>().gameObject))
            {
                Debug.LogError("Could not find Mesh for the vehicle");
            }
            else
            {
                MeshOfVehicle = gameObject.GetComponentInChildren<BoxCollider>().gameObject;
            }
        }


        RespawnUser();
	}
	
	void Update ()
    {
        ApplyGravity();

        Grounded = CheckGrounded();

        CrashedFromFront = CheckForCrashFromFront();
        CrashedFromBack = CheckForCrashFromBack();
        CheckForCrashFromTop();

        MoveVehicle();
        CorrectMeshAngle();

        TempBringCamera();
        CheckForRespawn();

        DebugRays();
	}

    void TempBringCamera()
    {
        if (MainCamera && CameraLocation)
        {
            MainCamera.gameObject.transform.position = Vector3.Lerp(MainCamera.transform.position, CameraLocation.transform.position, Speed * Time.deltaTime);
            MainCamera.GetComponentInChildren<Transform>().transform.LookAt(MeshOfVehicle.transform.position);
        }
    }

    void ApplyGravity()
    {
        if(!Grounded)
        {
            GravityModifier = 1 - Mathf.Abs(Direction);
            transform.position += ((Physics.gravity - (Physics.gravity * Mathf.Abs(Direction)))) * GravityModifier * Time.deltaTime * (transform.localScale.y);
        }
    }
    bool CheckGrounded()
    {
        if(Physics.Raycast(MeshOfVehicle.transform.position, -MeshOfVehicle.transform.up, out hit, HoverAmount))
        {
            if (hit.transform.gameObject.tag.CompareTo("Ground") == 0)
            {
                //Correct Hover Distance
                if (Vector3.Distance(transform.position, hit.point) < 0.1f)
                {
                    transform.position = hit.point + transform.up * HoverAmount;
                }
                return true;
            }
        }
        return false;
    }
    Vector3 GetForwardVector()
    {
        if (!Grounded)
        {
            return transform.forward;
        }
        return (Vector3.Cross(hit.normal, -MeshOfVehicle.transform.right));
    }
    void MoveVehicle()
    {
        if (Grounded)
        {
            transform.position += GetForwardVector() * Direction * Time.deltaTime * Speed;
            if (Direction != 0)
            {
                transform.Rotate(new Vector3(0, MovementTurn * Time.deltaTime * Mathf.Abs(Direction) * 2, 0));
                if (MovementTurn > 0)
                {
                    //Animate(ANIMATESTATES.RightTurn);
                }
                else if (MovementTurn < 0)
                {
                   // Animate(ANIMATESTATES.LeftTurn);
                }
            }
            else
            {
                Animate(ANIMATESTATES.IDLE);
            }
        }
        else
        {
            transform.position += GetForwardVector() * Direction * Time.deltaTime * Speed;
            SlowVehicleDown();
        }
    }
    void CorrectMeshAngle()
    {
        if(MeshOfVehicle)
        {
            transform.LookAt(MeshOfVehicle.transform.position + GetForwardVector(), MeshOfVehicle.transform.up);
            //transform.eulerAngles = new Vector3(-hit.transform.eulerAngles.x, transform.eulerAngles.y, -hit.transform.eulerAngles.z);
            //Work in progress to do a screw kind of driving
        }
        if(!Grounded)
        {
            transform.LookAt(MeshOfVehicle.transform.position + GetForwardVector());
        }
    }
    bool CheckForCrashFromFront()
    {
        RaycastHit hit;
        return Physics.Raycast(MeshOfVehicle.transform.position, GetForwardVector(), out hit, .1f) &&
            Physics.Raycast(MeshOfVehicle.transform.position, GetForwardVector() + MeshOfVehicle.transform.right, out hit, .1f) &&
            Physics.Raycast(MeshOfVehicle.transform.position, GetForwardVector() + -MeshOfVehicle.transform.right, out hit, .1f);
    }
    bool CheckForCrashFromBack()
    {
        RaycastHit hit;
        return Physics.Raycast(MeshOfVehicle.transform.position, -GetForwardVector(), out hit, 1) &&
            Physics.Raycast(MeshOfVehicle.transform.position, -GetForwardVector() + MeshOfVehicle.transform.right, out hit, 1) &&
            Physics.Raycast(MeshOfVehicle.transform.position, -GetForwardVector() + -MeshOfVehicle.transform.right, out hit, 1);
    }
    bool CheckForCrashFromTop()
    {
        RaycastHit hit;

        if(Physics.Raycast(MeshOfVehicle.transform.position, MeshOfVehicle.transform.up, out hit, .1f))
        {
            if(Mathf.Abs(transform.rotation.x) > 0.1f || Mathf.Abs(transform.rotation.z) > 0.1f)
            {
                Grounded = true;
                //Flip Vehicle back over
            }
        }
        if (Physics.Raycast(MeshOfVehicle.transform.position, MeshOfVehicle.transform.up + MeshOfVehicle.transform.forward, out hit, .1f))
        {
            if (Mathf.Abs(transform.rotation.x) > 0.1f || Mathf.Abs(transform.rotation.z) > 0.1f)
            {
                Grounded = true;
                //Flip Vehicle back over
            }
            else
            {
                if (Direction > 0)
                {
                    Direction = 0;
                }
                CrashedFromFront = true;
            }
        }
        if (Physics.Raycast(MeshOfVehicle.transform.position, MeshOfVehicle.transform.up + -MeshOfVehicle.transform.forward, out hit, .1f))
        {
            if (Mathf.Abs(transform.rotation.x) > 0.1f || Mathf.Abs(transform.rotation.z) > 0.1f)
            {
                Grounded = true;
                //Flip Vehicle back over
            }
            else
            {
                if (Direction < 0)
                {
                    Direction = 0;
                }
                CrashedFromBack = true;
            }
        }
        return false;
    }
    void CheckForCornerTouch()
    {
        Vector3 Right = MeshOfVehicle.transform.right * (transform.localScale.x / 2);
        Vector3 Forward = MeshOfVehicle.transform.forward * transform.localScale.y;
        Vector3 MeshPosition = MeshOfVehicle.transform.position;

        if (Physics.Raycast(MeshPosition + Right + Forward, -transform.up, out CornersHit[0], HoverAmount * 2))
        {
            BoolCornerHit[0] = (CornersHit[0].transform.gameObject.tag.CompareTo("Ground") == 0);
        }
        else
        {
            BoolCornerHit[0] = false;
        }
        if (Physics.Raycast(MeshPosition + -Right + Forward, -transform.up, out CornersHit[1], HoverAmount * 2))
        {
            BoolCornerHit[1] = (CornersHit[1].transform.gameObject.tag.CompareTo("Ground") == 0);
        }
        else
        {
            BoolCornerHit[1] = false;
        }
        if (Physics.Raycast(MeshPosition + Right + -Forward, -transform.up, out CornersHit[2], HoverAmount * 2))
        {
            BoolCornerHit[2] = (CornersHit[2].transform.gameObject.tag.CompareTo("Ground") == 0);
        }
        else
        {
            BoolCornerHit[2] = false;
        }
        if (Physics.Raycast(MeshPosition + -Right + -Forward, -transform.up, out CornersHit[3], HoverAmount * 2))
        {
            BoolCornerHit[3] = (CornersHit[3].transform.gameObject.tag.CompareTo("Ground") == 0);
        }
        else
        {
            BoolCornerHit[3] = false;
        }
    }

    void CheckForRespawn()
    {
        RaycastHit TempHit;
        if(Physics.Raycast(transform.position, -Vector3.up, out TempHit, MeshOfVehicle.transform.localScale.y))
        {
            if (hit.transform.gameObject.tag.CompareTo("Respawn") == 0)
            {
                RespawnUser();
            }
        }
        Debug.DrawRay(transform.position, -Vector3.up * 1 * MeshOfVehicle.transform.localScale.y, Color.red);
    }
    void RespawnUser()
    {
        if(Respawn)
        {
            transform.position = Respawn.transform.position;
        }
    }

    public void SlowVehicleDown()
    {
        MovementDirection -= Mathf.Lerp(MovementDirection, 0, 0.01f) * Time.deltaTime;
        if (Mathf.Abs(MovementDirection) < 0.1f)
        {
            MovementDirection = 0;
        }
    }
    void Animate(ANIMATESTATES CurrentState)
    {
        MyAnim.SetInteger("AnimateNum", (int)CurrentState);
    }
    void DebugRays()
    {
        Debug.DrawRay(transform.position, GetForwardVector() * transform.localScale.z * 2, Color.blue);//Movement Direction
        Debug.DrawRay(MeshOfVehicle.transform.position, -MeshOfVehicle.transform.up * HoverAmount, Color.yellow);//Down
        Debug.DrawRay(transform.position, -transform.up * HoverAmount, Color.red);//Down



        #region Collision Detection
        //Collision Detection
        #region Front Detection
        Debug.DrawRay(transform.position, GetForwardVector() * .5f * transform.localScale.z * 2, Color.white);//Front Detection
        Debug.DrawRay(transform.position, ((GetForwardVector() + MeshOfVehicle.transform.right * .5f) * .5f) * transform.localScale.z * 2, Color.white);//Front Right Detection
        Debug.DrawRay(transform.position, ((GetForwardVector() + -MeshOfVehicle.transform.right * .5f) * .5f) * transform.localScale.z * 2, Color.white);//Front Left Detection
        #endregion
        #region Back Detection
        Debug.DrawRay(transform.position, -GetForwardVector() * .5f * transform.localScale.z * 2, Color.white);//Back Detection
        Debug.DrawRay(transform.position, (-GetForwardVector() + MeshOfVehicle.transform.right * .5f) * .5f * transform.localScale.z * 2, Color.white);//Back Right Detection
        Debug.DrawRay(transform.position, (-GetForwardVector() + -MeshOfVehicle.transform.right * .5f) * .5f * transform.localScale.z * 2, Color.white);//Back Left Detection
        #endregion
        #region Roof Detection
        Debug.DrawRay(transform.position, (MeshOfVehicle.transform.up + MeshOfVehicle.transform.forward) * .1f, Color.red);//Up + Front
        Debug.DrawRay(transform.position, MeshOfVehicle.transform.up * .1f, Color.red);//Up
        Debug.DrawRay(transform.position, (MeshOfVehicle.transform.up + -MeshOfVehicle.transform.forward) * .1f, Color.red);//Up + back
        #endregion
        #endregion
        #region Edge Detection
        //Vector3 Right = MeshOfVehicle.transform.right * (transform.localScale.x / 2);
        //Vector3 Forward = MeshOfVehicle.transform.forward * transform.localScale.y;
        //Vector3 MeshPosition = MeshOfVehicle.transform.position;
        //Debug.DrawRay(MeshPosition + Right + Forward, -transform.up * HoverAmount * 2, Color.green);//   Front Right
        //if (!BoolCornerHit[0])
        //{
        //    Debug.DrawRay(MeshPosition + Right + Forward, -transform.up * HoverAmount * 2, Color.red);//   Front Right
        //}

        //Debug.DrawRay(MeshPosition + -Right + Forward, -transform.up * HoverAmount * 2, Color.green);//  Back  Right
        //if (!BoolCornerHit[1])
        //{
        //    Debug.DrawRay(MeshPosition + -Right + Forward, -transform.up * HoverAmount * 2, Color.red);//   Front Right
        //}

        //Debug.DrawRay(MeshPosition + Right + -Forward, -transform.up * HoverAmount * 2, Color.green);//  Front Left
        //if (!BoolCornerHit[2])
        //{
        //    Debug.DrawRay(MeshPosition + Right + -Forward, -transform.up * HoverAmount * 2, Color.red);//   Front Right
        //}

        //Debug.DrawRay(MeshPosition + -Right + -Forward, -transform.up * HoverAmount * 2, Color.green);// Back  Left
        //if (!BoolCornerHit[3])
        //{
        //    Debug.DrawRay(MeshPosition + -Right + -Forward, -transform.up * HoverAmount * 2, Color.red);//   Front Right
        //}
        #endregion
    }
}
