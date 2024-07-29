using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectPlacer : MonoBehaviourPunCallbacks
{
    public bool isInSetupMode;
    public OVRSpatialAnchor worldAnchor;
    public LayerMask paletteMask;
    public LayerMask wallMask;
    private PalletteSpawnItemButton currentSelectedObject;
    private GameObject previewInstance;
    private GameObject spawnedItem;

    public GameObject objectPallette;
    public GameObject saveAnchorPallette;
    public ObjectManager objectManager;

    public float rotationAmount = 15;

    public SpatialAnchorManager anchorManager;
    public LineRenderer line;
    public bool placeObjectRayActive = false;
    public Transform spatialAnchorPrefabSpawnPosition;
    private GameObject previzPrefab;

    SharedAnchor colocationAnchor;



    bool canPlaceWorldAnchor;
    int objectIndex;
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        //objectPallette.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
            canPlaceWorldAnchor = true;
    }

    void Update()
    {
        if(placeObjectRayActive == true)
        {
            //Ray ray = new Ray(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward);
            Ray ray = new Ray(transform.position, transform.rotation * Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //Debug.LogError($"Hit: {hit}");
                line.enabled = true;

                line.SetPosition(0, transform.position);
                line.SetPosition(1, hit.point);
                //spatialAnchorPrefabSpawnPosition.position = hit.point;




            }


            if (previzPrefab)
            {
                previzPrefab.transform.position = hit.point;

                if(OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.RTouch))
                {
                    previzPrefab.transform.Rotate(new Vector3(0, rotationAmount, 0), Space.Self);
                }

                if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.RTouch))
                {
                    previzPrefab.transform.Rotate(new Vector3(0, -rotationAmount, 0), Space.Self);
                }
            }





        }

        if (canPlaceWorldAnchor)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                colocationAnchor = Instantiate(worldAnchor, transform.position, transform.rotation).GetComponent<SharedAnchor>();
                StartCoroutine(SaveAndShareWorldAnchor());

                canPlaceWorldAnchor = false;
            }
            return;
        }

        if (previzPrefab && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            OVRSpatialAnchor anchorPrefab = ObjectDatabase.Instance.items[objectIndex];
            

            OVRSpatialAnchor anchorInstance = Instantiate(anchorPrefab, previzPrefab.transform.position, previzPrefab.transform.rotation);
            colocationAnchor = anchorInstance.GetComponent<SharedAnchor>();            
            StartCoroutine(SaveAndShareAnchor(anchorInstance));

            DestroyPreviewPrefab();


        }
    }

    public void SpawnPreviewPrefab()
    {
        previzPrefab = PhotonNetwork.Instantiate(ObjectDatabase.Instance.previzPrefabs[objectIndex].name.ToString(), transform.position, new Quaternion(0,0,0,0));


    }

    public void DestroyPreviewPrefab()
    {
        if (previzPrefab)
        {
            PhotonNetwork.Destroy(previzPrefab);
        }
    }


    //void Update()
    //{
    //    if(readyToPlaceWorldAnchor)
    //    {
    //        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
    //        {
    //            readyToPlaceWorldAnchor = false;
    //            isWorldAnchor = true;
    //            colocationAnchor = Instantiate(worldAnchor, transform.position, transform.rotation).GetComponent<SharedAnchor>();
    //            StartCoroutine(SaveAndShareAnchor());
    //        }

    //        return;
    //    }


    //    Ray ray = new Ray(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward);

    //    if (Physics.Raycast(ray, out RaycastHit hit, 20, paletteMask))
    //    {
    //        Debug.LogError($"Hit: {hit.transform}");
    //        line.enabled = true;

    //        line.SetPosition(0, transform.position);
    //        line.SetPosition(1, hit.point);

    //        if (previewInstance)
    //        {
    //            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.RTouch))
    //            {
                    
    //                previewInstance.transform.Rotate(new Vector3(0, rotationAmount, 0), Space.Self);
                    
    //            }
    //            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.RTouch))
    //            {
    //                previewInstance.transform.Rotate(new Vector3(0, -rotationAmount, 0), Space.Self);
                    
    //            }

    //            previewInstance.transform.position = hit.point;
                

    //            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
    //            {

    //                OVRSpatialAnchor anchorPrefab = ObjectDatabase.instance.GetItemByID(currentSelectedObject.itemID);

    //                colocationAnchor = Instantiate(anchorPrefab, previewInstance.transform.position, previewInstance.transform.rotation).GetComponent<SharedAnchor>();

    //                StartCoroutine(SaveAndShareAnchor());

    //                //OVRSpatialAnchor anchorPrefab = ObjectDatabase.instance.GetItemByID(currentSelectedObject.itemID);
    //                //spawnedItem = anchorManager.CreateSpatialAnchor(anchorPrefab, previewInstance.transform.position, previewInstance.transform.rotation).gameObject;

    //                currentSelectedObject = null;
    //                Destroy(previewInstance);
    //                //saveAnchorPallette.SetActive(true);


    //            }
    //        }

    //        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
    //        {
    //            PalletteSpawnItemButton palletButton = hit.transform.GetComponent<PalletteSpawnItemButton>();

    //            if (isInSetupMode && palletButton && !currentSelectedObject)
    //            {
    //                currentSelectedObject = hit.transform.GetComponent<PalletteSpawnItemButton>();
    //                //objectPallette.SetActive(false);

    //                GameObject previewPrefab = ObjectDatabase.instance.GetItemPreview(palletButton.itemID);
    //                previewInstance = Instantiate(previewPrefab, hit.point, Quaternion.identity);
    //            }
    //            else if(!palletButton)
    //            {
    //                Button button = hit.transform.GetComponent<Button>();

    //                if(button)
    //                {
    //                    Vector2 screenPosition = Camera.main.WorldToScreenPoint(hit.point);

    //                    var pointerEventData = new PointerEventData(EventSystem.current) { position = screenPosition };
    //                    ExecuteEvents.Execute(button.gameObject, pointerEventData, ExecuteEvents.submitHandler);
    //                }
    //            }
    //        }
            
    //    }
    //    else
    //        line.enabled = false;
    //}

    public void SelectObjectNumber(int objectIndexNumber)
    {
        objectIndex = objectIndexNumber;
        placeObjectRayActive = true;
        SpawnPreviewPrefab();
        
    }



    IEnumerator SaveAndShareWorldAnchor()
    {
        colocationAnchor.OnSaveLocalButtonPressed();

        SampleController.Instance.Log("RSVR: Saving world anchor locally");
        yield return new WaitForSeconds(2);
        
        colocationAnchor.OnShareButtonPressed();
        SampleController.Instance.Log("RSVR: Trying to share world anchor");

        yield return new WaitForSeconds(2);

        colocationAnchor.OnAlignButtonPressed();
        SampleController.Instance.Log("RSVR: Trying to align world anchor");
    }

    IEnumerator SaveAndShareAnchor(OVRSpatialAnchor spatialAnchor)
    {
        Debug.LogError($"Spatial Anchor (Before Save): " + spatialAnchor);
        Debug.LogError($"Spatial Anchor UUID (Before Save): " + spatialAnchor.Uuid);
        colocationAnchor.OnSaveLocalButtonPressed();

        SampleController.Instance.Log("RSVR: Saving anchor locally");
        yield return new WaitForSeconds(2);

        Debug.LogError($"Spatial Anchor (After Save): " + spatialAnchor);
        Debug.LogError($"Spatial Anchor UUID (After Save): " + spatialAnchor.Uuid);

        Debug.LogError($"RSVR: item {ObjectDatabase.Instance.items[objectIndex]}");
        //Debug.LogError($"RSVR: item id {ObjectDatabase.Instance.items[objectIndex].GetComponent<Item>().id}");
        Debug.LogError($"RSVR: OVRSpatialAnchor {spatialAnchor}");
        Debug.LogError($"RSVR: uuid {colocationAnchor.GetComponent<OVRSpatialAnchor>().Uuid.ToString()}");

        string uuid = spatialAnchor.Uuid.ToString();


        NetworkManager.Instance.SendAnchorInfo(uuid, objectIndex);
        yield return new WaitForSeconds(2);

        colocationAnchor.OnShareButtonPressed();
        SampleController.Instance.Log("RSVR: Trying to share anchor");
    }

    public void SaveAnchor()
    {
        anchorManager.SaveLastCreatedAnchor();
        saveAnchorPallette.SetActive(false);
        //objectPallette.SetActive(true);

        currentSelectedObject = null;
        if (previewInstance)
        {
            Destroy(previewInstance);
        }
    }

    public void DeleteAnchor()
    {
        anchorManager.UnsaveLastCreatedAnchor();
       
        Destroy(spawnedItem);

        saveAnchorPallette.SetActive(false);
        objectPallette.SetActive(true);

        currentSelectedObject = null;
        if (previewInstance)
        {
            Destroy(previewInstance);
        }
    }
}
