using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [System.NonSerialized]
    public int VehicleType;
    public static GameManager Instance = null;

    private void Awake()
    {
        VehicleType = 0;
    }

    void Start ()
    {
        #region Singleton
        DontDestroyOnLoad(this);
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != null)
        {
            Destroy(gameObject);
        }
        #endregion
    }

    public GameObject GetChildWithTag(GameObject Obj, string tag)
    {
        if (Obj && tag.Length > 0)
        {
            for (int i = 0; i < Obj.transform.childCount; i++)
            {
                if (Obj.transform.GetChild(i).tag.CompareTo(tag) == 0)
                {
                    return Obj.transform.GetChild(i).gameObject;
                }
            }
        }
        return null;
    }
    public List<GameObject> GetChildrenWithTag(GameObject Obj, string tag)
    {
        List<GameObject> Children = new List<GameObject>();
        if (Obj && tag.Length > 0)
        {
            for (int i = 0; i < Obj.transform.childCount; i++)
            {
                if (Obj.transform.GetChild(i).tag.CompareTo(tag) == 0)
                {
                    Children.Add(Obj.transform.GetChild(i).gameObject);
                }
            }
        }
        return Children;
    }

    public GameObject GetChildWithName(GameObject Obj, string Name)
    {
        if (Obj && Name.Length > 0)
        {
            for (int i = 0; i < Obj.transform.childCount; i++)
            {
                if (Obj.transform.GetChild(i).name.CompareTo(Name) == 0)
                {
                    return Obj.transform.GetChild(i).gameObject;
                }
            }
        }
        return null;
    }
    public List<GameObject> GetChildrenWithName(GameObject Obj, string Name)
    {
        List<GameObject> Children = new List<GameObject>();
        if (Obj && Name.Length > 0)
        {
            for (int i = 0; i < Obj.transform.childCount; i++)
            {
                if (Obj.transform.GetChild(i).name.CompareTo(Name) == 0)
                {
                    Children.Add(Obj.transform.GetChild(i).gameObject);
                }
            }
        }
        return Children;
    }
}
