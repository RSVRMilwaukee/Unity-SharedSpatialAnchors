using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public List<OVRSpatialAnchor> items;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetItemById(string id)
    {
        return items.Where(x => x.GetComponent<Item>().id == id).FirstOrDefault().gameObject;
    }
}
