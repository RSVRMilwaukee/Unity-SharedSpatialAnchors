using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectDatabase : MonoBehaviour
{
    public List<OVRSpatialAnchor> items;
    public List<Item> previzPrefabs;

    public static ObjectDatabase instance;

    public void Awake()
    {
        instance = this;
    }

    public GameObject GetItemPreview(string id)
    {
        return previzPrefabs.Where(x => x.GetComponent<Item>().id == id).FirstOrDefault().gameObject;
    }

    public OVRSpatialAnchor GetItemByID(string id)
    {
        return items.Where(x => x.GetComponent<Item>().id == id).FirstOrDefault();
    }
}
