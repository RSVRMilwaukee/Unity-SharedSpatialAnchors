using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject loadAnchorCanvas;
    public GameObject ObjectPlacerCanvas;

    public ObjectPlacer objectPlacer;

    public SpatialAnchorManager anchorManager;

    public const string NumUuidsPlayerPref = "numUuids";

    void Start()
    {
        if (PlayerPrefs.HasKey(SpatialAnchorManager.NumUuidsPlayerPref))
        {
            var playerUuidCount = PlayerPrefs.GetInt(SpatialAnchorManager.NumUuidsPlayerPref);

            if (playerUuidCount > 0)
            {
                loadAnchorCanvas.SetActive(true);
                ObjectPlacerCanvas.SetActive(false);
            }
            else
            {
                loadAnchorCanvas.SetActive(false);
                ObjectPlacerCanvas.SetActive(true);
                objectPlacer.isInSetupMode = true;
            }
        }
        else
        {
            loadAnchorCanvas.SetActive(false);
            ObjectPlacerCanvas.SetActive(true);
            objectPlacer.isInSetupMode = true;
        }
    }

    public void LoadSavedAnchors()
    {
        Debug.LogError("Try Load Anchors");
        anchorManager.LoadSavedAnchors();

        loadAnchorCanvas.SetActive(false);
        ObjectPlacerCanvas.SetActive(true);
        objectPlacer.isInSetupMode = true;
    }

    public void EraseSavedAnchors()
    {
        anchorManager.UnsaveAllAnchors();
        loadAnchorCanvas.SetActive(false);
        ObjectPlacerCanvas.SetActive(true);
        objectPlacer.isInSetupMode = true;
        Debug.LogError("Clear PlayerPrefs");
    }
}
