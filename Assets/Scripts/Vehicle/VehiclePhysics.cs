using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public class VehiclePhysics : MonoBehaviour
{
    #region Car Misc
    GameObject Respawn;
    int Health = 100;
    float MinHitSpeed = 2;
    #endregion

    #region Car Mechanics
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
    #endregion

    #region Car Physics
    [Header("Car Physics")]
    RaycastHit hit;
    RaycastHit[] CornersHit = new RaycastHit[4];
    bool[] BoolCornerHit = new bool[4];

    public float GravityModifier = 1;

    Vector3 ForwardVector;

    public float HoverAmount;
    public bool Grounded;
    public bool Rolling;

    public bool CrashedFromFront;
    public bool CrashedFromBack;
    #endregion

    #region Car Children
    [Header("Car Children")]
    public GameObject MeshOfVehicle;
    #endregion

    #region Car Animations
    [Header("Animation")]
    Animator MyAnim;
    enum ANIMATESTATES { IDLE, LeftTurn, RightTurn};
    #endregion

    #region CameraInfo
    [Header("Camera Info")]
    GameObject MainCamera;
    GameObject CameraLocation;
    #endregion

    void Start ()
    {
        MainCamera = GameObject.Find("Camera");
        CameraLocation = GameObject.Find("CameraMoveTo");
        Respawn = GameObject.Find("Respawn");

        Rolling = false;
        Grounded = false;
        CrashedFromFront = false;
        CrashedFromBack = false;
        MyAnim = GetComponent<Animator>();
        if (!MeshOfVehicle)
        {
            Debug.LogError("Could not find Mesh for the vehicle");
        }
        Speed *= transform.parent.transform.localScale.x;

        RespawnUser();
	}
	
	void Update ()
    {
        if (!MeshOfVehicle)
            return;
        ApplyGravity();

        Grounded = CheckGrounded();

        CrashedFromFront = CheckForCrashFromFront();
        CrashedFromBack = CheckForCrashFromBack();
        CheckForCrashFromTop();

        MoveVehicle();
        CorrectMeshAngle();

        TempBringCamera();
        CheckForRespawn();

        if (HitCheckpoint())
            print("Hit Checkpoint");

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
        if(!Grounded)//Not on the ground
        {
            if (Direction < 0)//not in motion
            {
                GravityModifier = 1 - Mathf.Abs(Direction);//How fast it will fall
            }
            else//In Motion
            {
                GravityModifier = 1 - Mathf.Abs(Direction);//Falls slower
            }
            //Applys the gravity
            transform.position += ((Physics.gravity - (Physics.gravity * Mathf.Abs(Direction)))) * GravityModifier * Time.deltaTime * (transform.localScale.y);
        }
    }
    bool CheckGrounded()
    {
        if (MeshOfVehicle)
        {
            if (Physics.Raycast(MeshOfVehicle.transform.position, -MeshOfVehicle.transform.up, out hit, HoverAmount * 1.5f))
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
        transform.position += GetForwardVector() * Direction * Time.deltaTime * Speed;
        if (Grounded)
        {
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
            if(Vector3.Angle(MeshOfVehicle.transform.forward, Vector3.up) < 20)
            {
                Direction -= Time.deltaTime;
            }
        }
        else
        {
            SlowVehicleDown();
        }
    }
    void CorrectMeshAngle()
    {
        if(MeshOfVehicle)
        {
            transform.LookAt(MeshOfVehicle.transform.position + GetForwardVector(), MeshOfVehicle.transform.up);
            if (Grounded)
            {
                if (hit.transform != null)
                {
                    Quaternion NewRot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -hit.transform.eulerAngles.z);
                    transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, Time.deltaTime * 10);
                }
                else
                {
                    Grounded = true;
                }
            }
        }
        if(!Grounded)
        {
            //transform.LookAt(MeshOfVehicle.transform.position + GetForwardVector(), transform.up);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }
    }
    bool CheckForCrashFromFront()
    {
        if(!MeshOfVehicle)
            return false;

        RaycastHit hit;
        string CheckPoint = "Checkpoint";
        Vector3 StartRay = MeshOfVehicle.transform.position + new Vector3(0,.5f,0);
        return Physics.Raycast(StartRay, GetForwardVector(), out hit, .1f) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0 &&
            Physics.Raycast(StartRay, GetForwardVector() + MeshOfVehicle.transform.right, out hit, .1f) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0 &&
            Physics.Raycast(StartRay, GetForwardVector() + -MeshOfVehicle.transform.right, out hit, .1f) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0;
    }
    bool CheckForCrashFromBack()
    {
        if (!MeshOfVehicle)
            return false;

        RaycastHit hit;
        string CheckPoint = "Checkpoint";
        Vector3 StartRay = MeshOfVehicle.transform.position + new Vector3(0, .5f, 0);
        return Physics.Raycast(StartRay, -GetForwardVector(), out hit, 1) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0 &&
            Physics.Raycast(StartRay, -GetForwardVector() + MeshOfVehicle.transform.right, out hit, 1) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0 &&
            Physics.Raycast(StartRay, -GetForwardVector() + -MeshOfVehicle.transform.right, out hit, 1) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0;
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
                CrashedFromFront = true;
                if (Direction > 0)
                {
                    goto JumpPos;//Jumping to the same data as below
                }
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
                    goto JumpPos;//Jumping to the same data as Above
                }
                CrashedFromBack = true;
            }
        }
        return false;
        
        JumpPos:
        if(hit.transform.gameObject.tag.CompareTo("CheckPoint") == 0)
        {
            return false;
        }
        if (Mathf.Abs(Direction * Speed) > MinHitSpeed)
        {
            int DamageMultiplier = 5;
            Health -= 1 + (int)((Mathf.Abs(Direction * Speed) - MinHitSpeed) * DamageMultiplier);
            print("Damaged: " + ((int)((Mathf.Abs(Direction * Speed) - MinHitSpeed) * DamageMultiplier) + 1));
            print("Health: " + Health);
        }

        Direction = 0;//Where it sets the speed after collision
        return false;
    }

    public GameObject HitCheckpoint()
    {
        string CheckPoint = "CheckPoint";
        if ((Physics.Raycast(MeshOfVehicle.transform.position, MeshOfVehicle.transform.up + MeshOfVehicle.transform.forward, out hit, .1f) &&
            hit.transform.tag.CompareTo(CheckPoint) == 0) ||
            Physics.Raycast(MeshOfVehicle.transform.position, MeshOfVehicle.transform.up + -MeshOfVehicle.transform.forward, out hit, .1f) &&
            hit.transform.tag.CompareTo(CheckPoint) == 0)
        {
            return hit.transform.gameObject;
        }
        return null;
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
            if (hit.transform && hit.transform.gameObject.tag.CompareTo("Respawn") == 0)
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
            //transform.LookAt(Respawn.transform.forward);
        }
    }

    public void SlowVehicleDown()
    {
        if (Grounded)
        {
            MovementDirection -= Mathf.Lerp(MovementDirection, 0, 0.01f) * Time.deltaTime;
        }
        else
        {
            MovementDirection -= Mathf.Lerp(MovementDirection, 0, 0.01f) * Time.deltaTime * 0.5f;
        }
        if (Mathf.Abs(MovementDirection) < 0.01f)
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
        Vector3 StartRay = MeshOfVehicle.transform.position + new Vector3(0, .5f, 0) * transform.localScale.z;
        Debug.DrawRay(StartRay, GetForwardVector() * .5f * transform.localScale.z * 2, Color.white);//Front Detection
        Debug.DrawRay(StartRay, ((GetForwardVector() + MeshOfVehicle.transform.right * .5f) * .5f) * transform.localScale.z * 2, Color.white);//Front Right Detection
        Debug.DrawRay(StartRay, ((GetForwardVector() + -MeshOfVehicle.transform.right * .5f) * .5f) * transform.localScale.z * 2, Color.white);//Front Left Detection
        #endregion
        #region Back Detection
        Debug.DrawRay(StartRay, -GetForwardVector() * .5f * transform.localScale.z * 2, Color.white);//Back Detection
        Debug.DrawRay(StartRay, (-GetForwardVector() + MeshOfVehicle.transform.right * .5f) * .5f * transform.localScale.z * 2, Color.white);//Back Right Detection
        Debug.DrawRay(StartRay, (-GetForwardVector() + -MeshOfVehicle.transform.right * .5f) * .5f * transform.localScale.z * 2, Color.white);//Back Left Detection
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
