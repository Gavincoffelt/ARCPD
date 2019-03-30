using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleSelectionPhysics : MonoBehaviour
{
    #region Vehicle Info
    public enum Vehicles { Car1, Car2, Car3 }
    public List<GameObject> VehicleMeshes = new List<GameObject>();
    public GameObject MeshOfVehicle;
    #endregion

    #region Objects
    GameManager Manager;
    GameObject Respawn;
    #endregion

    #region Physics
    bool Grounded;
    RaycastHit hit;
    float HoverAmount = .1f;
    float SpinSpeed = 10;
    #endregion

    #region Car Animations
    [Header("Animation")]
    Animator MyAnim;
    enum ANIMATESTATES { IDLE };
    #endregion


    void Start ()
    {
        MyAnim = GetComponent<Animator>();
        Manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Respawn = GameObject.FindGameObjectWithTag("Respawn");

        Animate(ANIMATESTATES.IDLE);
        InitVehicleMeshList();
        RespawnUser();
	}
	
	void Update ()
    {
        ApplyGravity();
        Grounded = CheckGrounded();
        Spin();
        Logic();
	}


    void Logic()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (Manager.VehicleType + 1 == VehicleMeshes.Count)
            {
                Manager.VehicleType = 0;
            }
            else
            {
                Manager.VehicleType++;
            }
            FindAndSetMesh((Vehicles)Manager.VehicleType);
            RespawnUser();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (Manager.VehicleType - 1 == -1)
            {
                Manager.VehicleType = VehicleMeshes.Count - 1;
            }
            else
            {
                Manager.VehicleType--;
            }
            FindAndSetMesh((Vehicles)Manager.VehicleType);
            RespawnUser();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
    }
    void Spin()
    {
        transform.Rotate(new Vector3(0,SpinSpeed * Time.deltaTime,0));
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
                    if (!Grounded)
                    {
                        Quaternion NewRot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -hit.transform.eulerAngles.z);
                        transform.rotation = NewRot;
                        //Sets the Vehicle Up right so it does not land sideways
                    }

                    return true;
                }
            }
        }

        return false;
    }


    public void InitVehicleMeshList()
    {
        Transform[] Children = this.GetComponentsInChildren<Transform>();
        for (int i = 0; i < Children.Length; i++)
        {
            if (Children[i].tag.CompareTo("Mesh") == 0)
            {
                VehicleMeshes.Add(Children[i].gameObject);
            }
        }
        FindAndSetMesh((Vehicles)Manager.VehicleType);
    }
    bool FindAndSetMesh(Vehicles VehicleType)
    {
        for(int i = 0; i < VehicleMeshes.Count; i++)
        {
            VehicleMeshes[i].SetActive(false);
        }
        MeshOfVehicle = VehicleMeshes[(int)VehicleType];
        if (MeshOfVehicle)
        {
            MeshOfVehicle.SetActive(true);
            return true;
        }
        return false;//Error if it returns back false
    }
    void ApplyGravity()
    {
        if (!Grounded)//Not on the ground
        {
            //Applys the gravity
            transform.position += (Physics.gravity * Time.deltaTime * (transform.localScale.y));
        }
    }

    public void RespawnUser()
    {
        if (Respawn)
        {
            transform.position = Respawn.transform.position;
            //transform.LookAt(Respawn.transform.forward);
        }
    }
    void Animate(ANIMATESTATES CurrentState)
    {
        MyAnim.SetInteger("AnimateNum", (int)CurrentState);
    }

    void Play()
    {
        SceneManager.LoadScene("Main");
    }
}
