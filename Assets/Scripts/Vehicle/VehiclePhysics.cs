using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePhysics : MonoBehaviour
{
    //Car Movement
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
    //Car Physics
    public float HoverAmount;
    RaycastHit hit;

    Vector3 ForwardVector;
    public bool Grounded;
	void Start ()
    {
        Grounded = false;
	}
	
	void Update ()
    {
        ApplyGravity();
        Grounded = CheckGrounded();
        MoveVehicle();
        DebugRays();
	}

    void ApplyGravity()
    {
        if(!Grounded)
        {
            transform.position += Physics.gravity * Time.deltaTime;
        }
    }
    bool CheckGrounded()
    {
        if(Physics.Raycast(transform.position, -Vector3.up, out hit, HoverAmount))
        {
            if (hit.transform.gameObject.tag.CompareTo("Ground") == 0)
            {
                return true;
            }
        }
        return false;
    }
    Vector3 GetForwardVector()
    {
        if(!Grounded)
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
                transform.Rotate(new Vector3(0, MovementTurn * Time.deltaTime * Mathf.Abs(Direction), 0));
               // transform.rotation = new Quaternion(0, transform.rotation.y, 0, 0);
            }
        }
    }

    void DebugRays()
    {
        Debug.DrawRay(transform.position, GetForwardVector() * 2, Color.blue);
        //Debug.DrawRay(transform.position, Vector3.forward * 2, Color.green);
    }
}
