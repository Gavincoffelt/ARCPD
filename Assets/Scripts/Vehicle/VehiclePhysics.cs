using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public class VehiclePhysics : MonoBehaviour
{
    public float GravityModifier = 1;

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
    public float Turn = 0;
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

    [Header("Car Children")]
    public GameObject MeshOfVehicle;

    [Header("Animation")]
    Animator MyAnim;
    enum ANIMATESTATES { IDLE, LeftTurn, RightTurn};

    void Start ()
    {
        Grounded = false;
        MyAnim = GetComponent<Animator>();
        if(!MeshOfVehicle)
        {
            if (!(gameObject.GetComponentInChildren<BoxCollider>().gameObject))
            {
                Debug.LogError("Could not find Mesh for the vehicle");
            }
            else
            {
                MeshOfVehicle = gameObject.GetComponentInChildren<BoxCollider>().gameObject;
            }
        }
	}
	
	void Update ()
    {
        ApplyGravity();
        Grounded = CheckGrounded();
        MoveVehicle();
        CorrectMeshAngle();

        //CheckForCornerTouch();
        DebugRays();
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
        if(Physics.Raycast(transform.position, -Vector3.up, out hit, HoverAmount))
        {
            if (hit.transform.gameObject.tag.CompareTo("Ground") == 0)
            {
                //Correct Hover Distance
                if(Vector3.Distance(transform.position, hit.point) < HoverAmount - 0.05f)
                {
                    transform.position = hit.point + new Vector3(0, HoverAmount, 0);
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
        return (Vector3.Cross(hit.normal, -transform.right));
    }
    void MoveVehicle()
    {
        if (Grounded)
        {
            transform.position += GetForwardVector() * Direction * Time.deltaTime * Speed;
            if(Direction != 0)
            {
                transform.Rotate(new Vector3(0, MovementTurn * Time.deltaTime * Mathf.Abs(Direction) * 2, 0));
                if(MovementTurn > 0)
                {
                    Animate(ANIMATESTATES.RightTurn);
                }
                else if (MovementTurn < 0)
                {
                    Animate(ANIMATESTATES.LeftTurn);
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
            transform.LookAt(MeshOfVehicle.transform.position + GetForwardVector());
        }
    }

    void CheckForCornerTouch()
    {
        Vector3 Right = MeshOfVehicle.transform.right * (transform.localScale.x / 2);
        Vector3 Forward = MeshOfVehicle.transform.forward * transform.localScale.y;
        Vector3 MeshPosition = MeshOfVehicle.transform.position;

        if (Physics.Raycast(MeshPosition + -Right + Forward, -transform.up, out CornersHit[0], HoverAmount))
        {
            if (CornersHit[0].transform.gameObject.tag.CompareTo("Ground") == 0)
            {
                BoolCornerHit[0] = true;
            }
            else
            {
                BoolCornerHit[0] = false;
            }   
        }
        if (Physics.Raycast(MeshPosition + Right + Forward, -transform.up, out CornersHit[0], HoverAmount))
        {
            if (CornersHit[1].transform.gameObject.tag.CompareTo("Ground") == 0)
            {
                BoolCornerHit[1] = true;
            }
            else
            {
                BoolCornerHit[1] = false;
            }
        }
        if (Physics.Raycast(MeshPosition + Right + -Forward, -transform.up, out CornersHit[0], HoverAmount))
        {
            if (CornersHit[2].transform.gameObject.tag.CompareTo("Ground") == 0)
            {
                BoolCornerHit[2] = true;
            }
            else
            {
                BoolCornerHit[2] = false;
            }
        }
        if (Physics.Raycast(MeshPosition + -Right + -Forward, -transform.up, out CornersHit[0], HoverAmount))
        {
            if (CornersHit[3].transform.gameObject.tag.CompareTo("Ground") == 0)
            {
                BoolCornerHit[3] = true;
            }
            else
            {
                BoolCornerHit[3] = false;
            }
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
        Debug.DrawRay(transform.position, Vector3.forward * transform.localScale.z * 2, Color.magenta);//forward
        Debug.DrawRay(transform.position, GetForwardVector() * transform.localScale.z * 2, Color.blue);//Movement Direction
        Debug.DrawRay(transform.position, -transform.up * HoverAmount, Color.red);//Down

        Vector3 Right = MeshOfVehicle.transform.right * (transform.localScale.x / 2);
        Vector3 Forward = MeshOfVehicle.transform.forward * transform.localScale.y;
        Vector3 MeshPosition = MeshOfVehicle.transform.position;

        Debug.DrawRay(MeshPosition + Right + Forward, -transform.up * HoverAmount, Color.green);//   Front Right
        if (BoolCornerHit[0])
        {
            Debug.DrawRay(MeshPosition + Right + Forward, -transform.up * HoverAmount, Color.red);//   Front Right
        }

        Debug.DrawRay(MeshPosition + -Right + Forward, -transform.up * HoverAmount, Color.green);//  Back  Right
        if (BoolCornerHit[1])
        {
            Debug.DrawRay(MeshPosition + -Right + Forward, -transform.up * HoverAmount, Color.red);//   Front Right
        }

        Debug.DrawRay(MeshPosition + Right + -Forward, -transform.up * HoverAmount, Color.green);//  Front Left
        if (BoolCornerHit[2])
        {
            Debug.DrawRay(MeshPosition + Right + -Forward, -transform.up * HoverAmount, Color.red);//   Front Right
        }

        Debug.DrawRay(MeshPosition + -Right + -Forward, -transform.up * HoverAmount, Color.green);// Back  Left
        if (BoolCornerHit[3])
        {
            Debug.DrawRay(MeshPosition + -Right + -Forward, -transform.up * HoverAmount, Color.red);//   Front Right
        }

    }
}
