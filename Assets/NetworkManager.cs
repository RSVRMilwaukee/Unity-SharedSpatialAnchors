using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkManager : MonoBehaviourPun
{
    public static NetworkManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SendAnchorInfo(string uuid, int objectIndex)
    {
        photonView.RPC("RpcSendAnchorInfo", RpcTarget.AllBuffered, uuid, objectIndex);
        SampleController.Instance.Log("RSVR: Trying to send anchor info");
    }

    [PunRPC]
    public void RpcSendAnchorInfo(string uuid, int objectIndex)
    {
        Debug.LogError($"Sample Controller: {SampleController.Instance}");

        Debug.LogError($"ItemDatabase: {ObjectDatabase.Instance}");

        Debug.LogError($"GetItemById: {ObjectDatabase.Instance.GetItemByID(objectIndex.ToString())}");

        Debug.LogError($"OVRSpatialAnchor: {ObjectDatabase.Instance.items[objectIndex].GetComponent<OVRSpatialAnchor>()}");
        SampleController.Instance.Log($"recieved anchor info from anchor {uuid} -- id is {objectIndex}");
        SampleController.Instance.anchorPrefab = ObjectDatabase.Instance.items[objectIndex].GetComponent<OVRSpatialAnchor>();
    }
}
