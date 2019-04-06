using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*Forrest McCarthy's Script*/

public class VehicleSelectionPhysics : MonoBehaviour
{
    #region Vehicle Info
    public enum Vehicles { Car1, Car2, Car3 }
    public List<GameObject> VehicleMeshes = new List<GameObject>();
    public GameObject MeshOfVehicle;
    GameObject[] StatDisplayers;
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

    struct VclStat
    {
        public float SetStarterDamage;
        public float SetSpeed;
        public float SetHover;
        public float GravityModifier;

        public VclStat(int StarterDamage, int Speed, float Hover, int GravModifier)
        {
            SetStarterDamage = StarterDamage;
            SetSpeed = Speed;
            SetHover = Hover;
            GravityModifier = GravModifier;
        }
    }
    struct VehicleStats
    {
        List<VclStat> Stats;

        public void Init()
        {
            Stats = new List<VclStat>();
            AddVehicleStats();
        }
        void AddVehicleStats()
        {
            Stats.Add(new VclStat(100, 3, 0.1f, 2));
            Stats.Add(new VclStat(60, 4, 0.15f, 1));
        }
        public VclStat GetCurrentVehicle(int Index)
        {
            return Stats[Index];
        }
    }

    VehicleStats MainVehicleSet;


    public bool EndScreen;
    void Start()
    {
        MyAnim = GetComponent<Animator>();
        Manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Respawn = GameObject.FindGameObjectWithTag("Respawn");

        Animate(ANIMATESTATES.IDLE);
        MainVehicleSet.Init();
        InitVehicleMeshList();
        if(!EndScreen)
            InitStatDisplay();
        RespawnUser();
    }

    void Update()
    {
        if(!EndScreen)
            UpdateStatDisplay();
        ApplyGravity();
        Grounded = CheckGrounded();
        Spin();
        KeyboardControls();
    }

    void InitStatDisplay()
    {
        StatDisplayers = new GameObject[4];
        StatDisplayers[0] = GameObject.Find("DisplayHealth");
        StatDisplayers[1] = GameObject.Find("DisplaySpeed");
        StatDisplayers[2] = GameObject.Find("DisplayHover");
        StatDisplayers[3] = GameObject.Find("DisplayGravity");
    }
    void UpdateStatDisplay()
    {
        VclStat CurrentStats = MainVehicleSet.GetCurrentVehicle(Manager.VehicleType);
        StatDisplayers[0].GetComponent<Text>().text = "Health: " + CurrentStats.SetStarterDamage.ToString();
        StatDisplayers[1].GetComponent<Text>().text = "Speed: " + CurrentStats.SetSpeed.ToString();
        StatDisplayers[2].GetComponent<Text>().text = "Hover: " + CurrentStats.SetHover.ToString();
        StatDisplayers[3].GetComponent<Text>().text = "Gravity: " + CurrentStats.GravityModifier.ToString();
    }

    void KeyboardControls()
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
    }

    void Spin()
    {
        transform.Rotate(new Vector3(0, SpinSpeed * Time.deltaTime, 0));
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
        for (int i = 0; i < VehicleMeshes.Count; i++)
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
            transform.rotation = Quaternion.Euler(0, 0, 0);
            //transform.LookAt(Respawn.transform.forward);
        }
    }
    void Animate(ANIMATESTATES CurrentState)
    {
        MyAnim.SetInteger("AnimateNum", (int)CurrentState);
    }

    public void Play()
    {
        SceneManager.LoadScene("Main");
    }
    public void Right()
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
    public void Left()
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
}