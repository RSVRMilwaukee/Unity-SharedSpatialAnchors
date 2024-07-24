using Common;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject pallete;

    public OVRSpatialAnchor worldAnchor;
    public AlignPlayer alignPlayer;
    public SampleController controller;
    public ObjectPlacer objectPlacer;

    SharedAnchor colocationAnchor;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        //SetWorldAnchor();

        if (PhotonNetwork.IsMasterClient)
            objectPlacer.readyToPlaceWorldAnchor = true;
            
        pallete.SetActive(true);
      //  pallete.SetActive(true);
    }

    void SetWorldAnchor()
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    colocationAnchor = Instantiate(worldAnchor, transform.position, transform.rotation).GetComponent<SharedAnchor>();

        //    StartCoroutine(controller.WaitingForAnchorLocalization());

        //    //colocationAnchor = Instantiate(worldAnchor, transform.position, transform.rotation).GetComponent<SharedAnchor>();
        //    //StartCoroutine(SavingAndSharing());
        //}
        //else
        //{

        //}


    }

    IEnumerator SavingAndSharing()
    {
        colocationAnchor.OnSaveLocalButtonPressed();

        yield return new WaitForSeconds(2);
        //while(!colocationAnchor.IsSavedLocally)
        //{
        //    Debug.Log("Saving World Anchor Locally..");
        //    yield return null;
        //}

        colocationAnchor.OnShareButtonPressed();


        yield return new WaitForSeconds(2);
        //while(!colocationAnchor.IsSelectedForAlign)
        //{
        //    Debug.Log("Sharing world anchor..");
        //    yield return null;
        //}

        controller.PlaceAnchorAtRoot();
        //StartCoroutine(controller.WaitingForAnchorLocalization());
        //colocationAnchor.OnAlignButtonPressed();
        //while (colocationAnchor.IsSelectedForAlign)
        //{
        //    Debug.Log("Aligning world anchor..");
        //    yield return null;
        //}

        yield return new WaitForSeconds(2);
        pallete.SetActive(true);
    }
}
