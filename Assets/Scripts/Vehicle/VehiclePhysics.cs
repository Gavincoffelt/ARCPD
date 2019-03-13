using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public class VehiclePhysics : MonoBehaviour
{
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

    Vector3 ForwardVector;
    public bool Grounded;

    [Header("Car Children")]
    public GameObject MeshOfVehicle;

    void Start ()
    {
        Grounded = false;
        if(!MeshOfVehicle)
        {
            if (!(MeshOfVehicle = gameObject.GetComponentInChildren<Transform>().gameObject))
            {
                Debug.LogError("Could not find Mesh for the vehicle");
            }
        }
	}
	
	void Update ()
    {
        ApplyGravity();
        Grounded = CheckGrounded();
        MoveVehicle();
        CorrectMeshAngle();
        DebugRays();
	}

    void ApplyGravity()
    {
        if(!Grounded)
        {
            transform.position += Physics.gravity * Time.deltaTime * (transform.localScale.y);
        }
    }
    bool CheckGrounded()
    {
        Debug.DrawRay(transform.position, -transform.up * HoverAmount, Color.red);
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
                transform.Rotate(new Vector3(0, MovementTurn * Time.deltaTime * Mathf.Abs(Direction) * 10, 0));
               // transform.rotation = new Quaternion(0, transform.rotation.y, 0, 0);
            }
        }
    }

    void CorrectMeshAngle()
    {
        if(MeshOfVehicle)
        {
            MeshOfVehicle.transform.LookAt(MeshOfVehicle.transform.position + GetForwardVector());
        }
    }

    void DebugRays()
    {
        Debug.DrawRay(transform.position, GetForwardVector() * 2, Color.blue);//Should move
        Debug.DrawRay(transform.position, Vector3.forward * 2, Color.green);//Its forward
    }
}
