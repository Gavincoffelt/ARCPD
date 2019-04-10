using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*Forrest McCarthy's Script*/
//Note to self, should have made a physics script and a movement script and a collision script all in serperate scripts

public class VehiclePhysics : MonoBehaviour
{
    #region Wrecked Text
    GameObject WreckedText;
    float Timer = 0;
    public bool Wrecked = false;
    #endregion

    #region Car Misc
    GameObject Respawn;
    int Health = 100;
    float MinHitSpeed = 2;
    Image[] Displays = new Image[6];
    #endregion

    #region Car Mechanics
    [Header("Car Mechanics")]
    private float Direction = 0;
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
    private float Turn = 0;
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
    public float Speed = 3;

    int StarterDamage;
    int[] Parts = new int[6];
    public enum Part { FrontLeft, Front, FrontRight, BackLeft, Back, BackRight };
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
    public struct PartSection
    {
        public GameObject This;
        public GameObject[] DamageLevels;

        public PartSection(GameObject MainObject, GameObject perfect, GameObject damaged, GameObject destroyed)
        {
            DamageLevels = new GameObject[3];
            This = MainObject;
            DamageLevels[0] = perfect;
            DamageLevels[1] = damaged;
            DamageLevels[2] = destroyed;
        }
    }
    public List<PartSection> PartSelections = new List<PartSection>();
    #endregion

    #region Car Animations
    [Header("Animation")]
    Animator MyAnim;
    enum ANIMATESTATES { IDLE, LeftTurn, RightTurn };
    #endregion

    #region CameraInfo
    [Header("Camera Info")]
    GameObject MainCamera;
    GameObject CameraLocation;
    #endregion

    #region MultipleVehicleInfo
    public enum Vehicles { Car1, Car2, Car3 }

    [Header("Specific Car")]
    GameManager Manager;
    Vehicles VehicleType;
    public List<GameObject> VehicleMeshes = new List<GameObject>();
    int SetSpeed
    {
        set { Speed = value; }
        get { return 0; }
    }
    int SetStarterDamage
    {
        set { StarterDamage = value; }
        get { return 0; }
    }
    float SetHover
    {
        set { HoverAmount = value; }
        get { return 0; }
    }
    float SetGravityModifier
    {
        set { GravityModifier = value; }
        get { return 0; }
    }
    #endregion


    void Start()
    {
        MainCamera = GameObject.Find("Camera");
        CameraLocation = GameObject.Find("CameraMoveTo");
        Respawn = GameObject.Find("Respawn");
        Manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        WreckedText = GameObject.Find("WreckedText");

        WreckedText.SetActive(false);

        VehicleStatsSetter();
        InitVehicleMeshList();

        #region Display Started
        Displays[0] = GameObject.Find("FL").GetComponent<Image>();
        Displays[1] = GameObject.Find("F").GetComponent<Image>();
        Displays[2] = GameObject.Find("FR").GetComponent<Image>();
        Displays[3] = GameObject.Find("BL").GetComponent<Image>();
        Displays[4] = GameObject.Find("B").GetComponent<Image>();
        Displays[5] = GameObject.Find("BR").GetComponent<Image>();
        #endregion
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
        print("Respawned");
        RespawnUser();
    }

    void Update()
    {
        if (!MeshOfVehicle)
            return;
        ApplyGravity();

        Grounded = CheckGrounded();

        CrashedFromFront = CheckForCrashFromFront();
        CrashedFromBack = CheckForCrashFromBack();
        CheckForCrashFromTop();

        WreckedCheck();
        if(!Wrecked)
            MoveVehicle();
        CorrectMeshAngle();
        TempBringCamera();

        CheckForRespawn();

        CheckForBrokenVehicle();

        KeyboardControls();
        HitCheckpoint();
        DebugRays();
        
    }

    void WreckedCheck()
    {
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
        }
        if (Timer <= 0 && WreckedText.activeInHierarchy)
        {
            WreckedText.SetActive(false);
            Wrecked = false;
            RespawnUser();
        }
    }

    public void InitVehicleMeshList()
    {
        List<GameObject> Children = Manager.GetChildrenWithTag(gameObject, "Mesh");
        for (int i = 0; i < Children.Count; i++)
        {
            VehicleMeshes.Add(Children[i].gameObject);
            Children[i].gameObject.SetActive(false);
        }
        FindAndSetMesh(VehicleType);
        GatherSectionsOfCar();
    }

    void VehicleStatsSetter()
    {
        VehicleType = (Vehicles)Manager.VehicleType;
        switch (VehicleType)
        {
            default:
            case Vehicles.Car1:
                SetStarterDamage = 100;
                SetSpeed = 3;
                SetHover = 0.1f;
                GravityModifier = 2;
                break;
            case Vehicles.Car2:
                SetStarterDamage = 60;
                SetSpeed = 4;
                SetHover = 0.15f;
                GravityModifier = 1;
                break;
            case Vehicles.Car3:
                SetStarterDamage = 100;
                SetSpeed = 3;
                break;
        }

    }
    bool FindAndSetMesh(Vehicles VehicleType)
    {
        MeshOfVehicle = VehicleMeshes[(int)VehicleType];
        if (MeshOfVehicle)
        {
            MeshOfVehicle.SetActive(true);
            return true;
        }
        return false;//Error if it returns back false
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
        if (!Grounded)//Not on the ground
        {
            if (Direction < 0)//not in motion
            {
                GravityModifier = 1 - Mathf.Abs(Direction);//How fast it will fall
            }
            else//In Motion
            {
                GravityModifier = 2 - Mathf.Abs(Direction);//Falls slower
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
            if (Direction > 0)
            {
                transform.Rotate(new Vector3(0, MovementTurn * Time.deltaTime * Mathf.Abs(Direction) * 2, 0));
            }
            else if (Direction < 0)
            {
                transform.Rotate(new Vector3(0, -MovementTurn * Time.deltaTime * Mathf.Abs(Direction) * 2, 0));
            }
            else
            {
                Animate(ANIMATESTATES.IDLE);
            }
        }
        else
        {
            Vector3 RotateAngle = new Vector3((Mathf.Abs(GetForwardVector().z) + Mathf.Abs(GetForwardVector().x)) * 0.4f, 0, 0);
            RotateAngle *= Direction < 0 ? -1 : 1;
            transform.Rotate(RotateAngle, Space.Self);
            SlowVehicleDown();
        }
    }
    void CorrectMeshAngle()
    {
        if (MeshOfVehicle)
        {
            //--------------------------------------------------
            //TODO: 
            //--------------------------------------------------
            transform.LookAt(transform.position + GetForwardVector(), MeshOfVehicle.transform.up);
            if (Grounded)
            {
                Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.position - new Vector3(0, 1, 0), out hit, 2);
                if (hit.transform != null)
                {
                    Quaternion NewRot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
                    transform.rotation = Quaternion.Lerp(transform.rotation, NewRot, Time.deltaTime * 10);
                    // Rotate the vehicle to the ground below it
                }
                else
                {
                    Grounded = true;
                }
            }
        }
    }
    bool CheckForCrashFromFront()
    {
        if (!MeshOfVehicle)
            return false;
        bool[] threeHits = new bool[3];

        RaycastHit hit;
        string CheckPoint = "CheckPoint";
        Vector3 StartRay = MeshOfVehicle.transform.position + MeshOfVehicle.transform.up * transform.localScale.z;

        threeHits[1] = Physics.Raycast(StartRay, GetForwardVector(), out hit, .1f) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0;
        threeHits[2] = Physics.Raycast(StartRay, GetForwardVector() + MeshOfVehicle.transform.right * .75f, out hit, .1f) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0;
        threeHits[0] = Physics.Raycast(StartRay, GetForwardVector() + -MeshOfVehicle.transform.right * .75f, out hit, .1f) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0;

        if (threeHits[0] || threeHits[1] || threeHits[2])
        {
            if (Mathf.Abs(Direction * Speed) > MinHitSpeed || (!Grounded && Mathf.Abs(Direction * Speed) > MinHitSpeed / 4))
            {
                for (int i = 0; i < 3; i++)
                {
                    if (threeHits[i])
                    {
                        int DamageMultiplier = 15;
                        if (Grounded)
                        {
                            DamagePart((Part)i, (int)((Mathf.Abs(Direction * Speed) - MinHitSpeed) * DamageMultiplier));
                        }
                        else
                        {
                            DamagePart((Part)i, (int)((Mathf.Abs(Direction * Speed) - MinHitSpeed / 4) * DamageMultiplier));
                        }
                        if (CheckPart((Part)i) > 75)
                            print("(GREEN)" + (Part)i + ": " + CheckPart((Part)i));
                        if (CheckPart((Part)i) < 75 && CheckPart((Part)i) > 50)
                            print("(YELLOW)" + (Part)i + ": " + CheckPart((Part)i));
                        if (CheckPart((Part)i) < 50)
                            print("(RED)" + (Part)i + ": " + CheckPart((Part)i));
                    }
                }
            }
            if (Direction > 0)
                Direction = 0;
        }




        return threeHits[0] || threeHits[1] || threeHits[2];
    }
    bool CheckForCrashFromBack()
    {
        if (!MeshOfVehicle)
            return false;
        bool[] threeHits = new bool[3];

        RaycastHit hit;
        string CheckPoint = "CheckPoint";
        Vector3 StartRay = MeshOfVehicle.transform.position + MeshOfVehicle.transform.up * transform.localScale.z;

        threeHits[1] = Physics.Raycast(StartRay, -GetForwardVector(), out hit, .1f) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0;
        threeHits[2] = Physics.Raycast(StartRay, -GetForwardVector() + MeshOfVehicle.transform.right * .75f, out hit, .1f) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0;
        threeHits[0] = Physics.Raycast(StartRay, -GetForwardVector() + -MeshOfVehicle.transform.right * .75f, out hit, .1f) && hit.transform.gameObject.tag.CompareTo(CheckPoint) != 0;

        if (threeHits[0] || threeHits[1] || threeHits[2])
        {
            if (Mathf.Abs(Direction * Speed) > MinHitSpeed || (!Grounded && Mathf.Abs(Direction * Speed) > MinHitSpeed / 4))
            {
                for (int i = 0; i < 3; i++)
                {
                    int j = i + 3;
                    if (threeHits[i])
                    {
                        int DamageMultiplier = 15;
                        if (Grounded)
                        {
                            DamagePart((Part)j, (int)((Mathf.Abs(Direction * Speed) - MinHitSpeed) * DamageMultiplier));
                        }
                        else
                        {
                            DamagePart((Part)j, (int)((Mathf.Abs(Direction * Speed) - MinHitSpeed / 4) * DamageMultiplier));
                        }
                        if (CheckPart((Part)j) > 75)
                            print("(GREEN)" + (Part)j + ": " + CheckPart((Part)j));
                        if (CheckPart((Part)j) < 75 && CheckPart((Part)i) > 50)
                            print("(YELLOW)" + (Part)j + ": " + CheckPart((Part)j));
                        if (CheckPart((Part)j) < 50)
                            print("(RED)" + (Part)j + ": " + CheckPart((Part)j));
                    }
                }
            }
            if (Direction < 0)
                Direction = 0;
        }




        return threeHits[0] || threeHits[1] || threeHits[2];
    }
    bool CheckForCrashFromTop()
    {
        RaycastHit hit;

        //Checks the up of the player
        if (Physics.Raycast(MeshOfVehicle.transform.position, MeshOfVehicle.transform.up, out hit, .1f))
        {
            if (Mathf.Abs(transform.rotation.x) > 0.1f || Mathf.Abs(transform.rotation.z) > 0.1f)
            {
                Grounded = true;
                RespawnUser(transform.position + new Vector3(0, 1, 0));
                //Flip Vehicle back over
            }
        }
        //Checks the up and forward of the player
        if (Physics.Raycast(MeshOfVehicle.transform.position, MeshOfVehicle.transform.up + MeshOfVehicle.transform.forward, out hit, .1f))
        {
            if (Mathf.Abs(transform.rotation.x) > 0.1f || Mathf.Abs(transform.rotation.z) > 0.1f)
            {
                Grounded = true;
                RespawnUser(transform.position + new Vector3(0, 1, 0));
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
        //Checks the up and backward of the player
        if (Physics.Raycast(MeshOfVehicle.transform.position, MeshOfVehicle.transform.up + -MeshOfVehicle.transform.forward, out hit, .1f))
        {
            if (Mathf.Abs(transform.rotation.x) > 0.1f || Mathf.Abs(transform.rotation.z) > 0.1f)
            {
                Grounded = true;
                RespawnUser(transform.position + new Vector3(0, 1, 0));
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
        if (hit.transform.gameObject.tag.CompareTo("CheckPoint") == 0)
        {
            return false;
        }
        if (Mathf.Abs(Direction * Speed) > MinHitSpeed)
        {
            int DamageMultiplier = 5;
            Health -= 1 + (int)((Mathf.Abs(Direction * Speed) - MinHitSpeed) * DamageMultiplier);
            //print("Damaged: " + ((int)((Mathf.Abs(Direction * Speed) - MinHitSpeed) * DamageMultiplier) + 1));
            //print("Health: " + Health);
        }

        Direction = 0;//Where it sets the speed after collision
        return false;
    }

    //Job of caller to check the GameObject is the correct checkpoint
    public GameObject HitCheckpoint()//Returns which checkpoint it hit
    {
        string CheckPoint = "CheckPoint";
        if ((Physics.Raycast(MeshOfVehicle.transform.position, MeshOfVehicle.transform.up + MeshOfVehicle.transform.forward, out hit, .1f) &&
            hit.transform.tag.CompareTo(CheckPoint) == 0) ||
            Physics.Raycast(MeshOfVehicle.transform.position, MeshOfVehicle.transform.up + -MeshOfVehicle.transform.forward, out hit, .1f) &&
            hit.transform.tag.CompareTo(CheckPoint) == 0)
        {
            print("Hit CheckPoint");
            return hit.transform.gameObject;
        }
        return null;
    }

    void CheckForCornerTouch()
    {
        Vector3 Right = MeshOfVehicle.transform.right * (transform.localScale.x / 2);
        Vector3 Forward = MeshOfVehicle.transform.forward * transform.localScale.y;
        Vector3 MeshPosition = MeshOfVehicle.transform.position;

        BoolCornerHit[0] = Physics.Raycast(MeshPosition + Right + Forward, -transform.up, out CornersHit[0], HoverAmount * 2)
            ? (CornersHit[0].transform.gameObject.tag.CompareTo("Ground") == 0) : false;

        BoolCornerHit[1] = Physics.Raycast(MeshPosition + -Right + Forward, -transform.up, out CornersHit[1], HoverAmount * 2)
            ? (CornersHit[1].transform.gameObject.tag.CompareTo("Ground") == 0) : false;

        BoolCornerHit[2] = Physics.Raycast(MeshPosition + Right + -Forward, -transform.up, out CornersHit[2], HoverAmount * 2)
            ? (CornersHit[2].transform.gameObject.tag.CompareTo("Ground") == 0) : false;

        BoolCornerHit[3] = Physics.Raycast(MeshPosition + -Right + -Forward, -transform.up, out CornersHit[3], HoverAmount * 2)
            ? (CornersHit[3].transform.gameObject.tag.CompareTo("Ground") == 0) : false;
    }

    void CheckForRespawn()
    {
        RaycastHit TempHit;
        if (Physics.Raycast(transform.position, -Vector3.up, out TempHit, MeshOfVehicle.transform.localScale.y))
        {
            if ((hit.transform && hit.transform.gameObject.tag.CompareTo("Respawn") == 0))
            {
                RespawnUser();
            }
        }
        Debug.DrawRay(transform.position, -Vector3.up * 1 * MeshOfVehicle.transform.localScale.y, Color.red);
    }
    public void RespawnUser()
    {
        if (Respawn)//GameObject to set the location to
        {
            transform.position = Respawn.transform.position;
            transform.LookAt(transform.position + Respawn.transform.forward, MeshOfVehicle.transform.up);
            ResetParts();
            UpdatePartDisplay();
        }
    }
    public void RespawnUser(Vector3 NewPosition)
    {
        transform.position = NewPosition;
        transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
    }
    public void RespawnButton()
    {
        transform.position = Respawn.transform.position;
        transform.LookAt(transform.position + Respawn.transform.forward, MeshOfVehicle.transform.up);
    }

    //Parts Functions
    public int CheckPart(Part PartyType)
    {
        return Parts[(int)PartyType];
    }
    int ReportDamageLevel(Part PartType)
    {
        if (CheckPart(PartType) >= StarterDamage * .75f)
        {
            return 1;
        }
        else if (CheckPart(PartType) < StarterDamage * .75f && CheckPart(PartType) > StarterDamage * .50f)
        {
            return 2;
        }
        else if (CheckPart(PartType) <= StarterDamage * .50f)
        {
            return 3;
        }
        return 1;
    }
    void DamagePart(Part PartType, int Damage)
    {
        Parts[(int)PartType] -= Damage;
        UpdatePartDisplay();
    }
    void ResetParts()
    {
        for (int i = 0; i < Parts.Length; i++)
        {
            Parts[i] = StarterDamage;
        }
    }

    void UpdatePartDisplay()
    {
        for (int i = 0; i < Parts.Length; i++)
        {
            int DamageLevel = ReportDamageLevel((Part)i);
            if (DamageLevel == 1)
            {
                Displays[i].color = Color.green;
            }
            else if (DamageLevel == 2)
            {
                Displays[i].color = Color.yellow;
            }
            else if (DamageLevel == 3)
            {
                Displays[i].color = Color.red;
            }
            DisplayDamagedParts((Part)i);
        }
    }

    void CheckForBrokenVehicle()
    {
        for (int i = 0; i < Parts.Length; i++)
        {
            if (CheckPart((Part)i) <= 0 && Timer <= 0)
            {
                WreckedText.SetActive(true);
                Wrecked = true;
                Timer = 3;
            }
        }
    }

    void GatherSectionsOfCar()
    {
        string[] DifferentParts = { "FrontLeft", "Front", "FrontRight", "BackLeft", "Back", "BackRight" };
        List<GameObject> ChildrenOfMesh = new List<GameObject>();
        for(int i = 0; i < DifferentParts.Length; i++)
        {
            ChildrenOfMesh.Add(Manager.GetChildWithName(MeshOfVehicle, DifferentParts[i]));
        }
        for(int i = 0; i < ChildrenOfMesh.Count; i++)
        {
            GameObject Perfect = Manager.GetChildWithName(ChildrenOfMesh[i],    "Perfect" );
            GameObject Damaged = Manager.GetChildWithName(ChildrenOfMesh[i],    "Damaged" );
            GameObject Destroyed = Manager.GetChildWithName(ChildrenOfMesh[i], "Destroyed");
            PartSelections.Add(new PartSection(ChildrenOfMesh[i], Perfect, Damaged, Destroyed));
        }
    }
    void DisplayDamagedParts(Part part)
    {
        int PartToBeShown = ReportDamageLevel(part) - 1;
        for (int i = 0; i < PartSelections[(int)part].DamageLevels.Length; i++)
        {
            PartSelections[(int)part].DamageLevels[i].SetActive(false);
        }
        PartSelections[(int)part].DamageLevels[PartToBeShown].SetActive(true);
    }
    //End Part functions


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
        Vector3 StartRay = MeshOfVehicle.transform.position + MeshOfVehicle.transform.up * transform.localScale.z;
        Debug.DrawRay(StartRay, GetForwardVector() * .5f * transform.localScale.z * 2, Color.blue);//Front Detection
        Debug.DrawRay(StartRay, ((GetForwardVector() + MeshOfVehicle.transform.right * .75f * .5f) * .5f) * transform.localScale.z * 2, Color.blue);//Front Right Detection
        Debug.DrawRay(StartRay, ((GetForwardVector() + -MeshOfVehicle.transform.right * .75f * .5f) * .5f) * transform.localScale.z * 2, Color.blue);//Front Left Detection
        #endregion
        #region Back Detection
        Debug.DrawRay(StartRay, -GetForwardVector() * .5f * transform.localScale.z * 2, Color.blue);//Back Detection
        Debug.DrawRay(StartRay, (-GetForwardVector() + MeshOfVehicle.transform.right * .5f) * .5f * transform.localScale.z * 2, Color.blue);//Back Right Detection
        Debug.DrawRay(StartRay, (-GetForwardVector() + -MeshOfVehicle.transform.right * .5f) * .5f * transform.localScale.z * 2, Color.blue);//Back Left Detection
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

    void KeyboardControls()
    {
        if (Grounded)
        {
            if (Input.GetKey(KeyCode.W))
            {
                MovementDirection += Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                MovementDirection -= Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                MovementTurn -= Time.deltaTime * 100;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                MovementTurn += Time.deltaTime * 100;
            }
            else if(Input.GetKey(KeyCode.F))
            {
                MovementTurn = 0;
            }
        }
    }

}