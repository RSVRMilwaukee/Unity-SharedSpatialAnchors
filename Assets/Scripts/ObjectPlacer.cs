using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectPlacer : MonoBehaviour
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
    public SampleController controller;
    SharedAnchor colocationAnchor;

    public bool readyToPlaceWorldAnchor = false;
    bool isWorldAnchor = false;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (readyToPlaceWorldAnchor)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                readyToPlaceWorldAnchor = false;
                isWorldAnchor = true;
                colocationAnchor = Instantiate(worldAnchor, transform.position, transform.rotation).GetComponent<SharedAnchor>();
                StartCoroutine(SaveAndShareAnchor());
            }

            return;
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            OVRSpatialAnchor anchorPrefab = ObjectDatabase.instance.items[0];

            colocationAnchor = Instantiate(anchorPrefab, transform.position, transform.rotation).GetComponent<SharedAnchor>();

            StartCoroutine(SaveAndShareAnchor());

            //OVRSpatialAnchor anchorPrefab = ObjectDatabase.instance.GetItemByID(currentSelectedObject.itemID);
            //spawnedItem = anchorManager.CreateSpatialAnchor(anchorPrefab, previewInstance.transform.position, previewInstance.transform.rotation).gameObject;

            //currentSelectedObject = null;
            //Destroy(previewInstance);
            //saveAnchorPallette.SetActive(true);
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


    IEnumerator SaveAndShareAnchor()
    {
        colocationAnchor.OnSaveLocalButtonPressed();
        yield return new WaitForSeconds(2);
        colocationAnchor.OnShareButtonPressed();
        yield return new WaitForSeconds(2);
        //StartCoroutine(controller.WaitingForAnchorLocalization());

        if(isWorldAnchor)
        {
            colocationAnchor.OnAlignButtonPressed();
        }
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
