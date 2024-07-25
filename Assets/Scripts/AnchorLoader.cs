using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnchorLoader : MonoBehaviour
{
    private OVRSpatialAnchor anchorPrefab;
    Action<OVRSpatialAnchor.UnboundAnchor, bool> _onLoadAnchor;

    private List<OVRSpatialAnchor> loadedAchors = new List<OVRSpatialAnchor>();

    public ObjectPlacer objectPlacer;
    public GameObject itemPallette;
    public ObjectManager objectManager;
    public SpatialAnchorManager anchorManager;

    private void Awake()
    {

        // Assign the OnLocalized method to the _onLoadAnchor action
        _onLoadAnchor = OnLocalized;
    }

    public void LoadAnchorByUuid()
    {
        // Check if the number of saved anchor UUIDs key exists in PlayerPrefs
        if (!PlayerPrefs.HasKey(SpatialAnchorManager.NumUuidsPlayerPref))
        {
            // Initialize the number of saved anchor UUIDs to 0
            PlayerPrefs.SetInt(SpatialAnchorManager.NumUuidsPlayerPref, 0);
        }

        // Get the current number of saved anchor UUIDs
        var playerUuidCount = PlayerPrefs.GetInt(SpatialAnchorManager.NumUuidsPlayerPref);

        // If there are no saved anchor UUIDs, return
        if (playerUuidCount == 0) return;
        Debug.LogError("Loading Anchors - Anchor Count is : " + playerUuidCount);

        // Create an array to store the UUIDs
        var uuids = new Guid[playerUuidCount];

        // Retrieve each saved anchor UUID from PlayerPrefs and store it in the array
        for (int i = 0; i < playerUuidCount; ++i)
        {
            var uuidKey = "uuid" + i;
            var currentUuid = PlayerPrefs.GetString(uuidKey);
            uuids[i] = new Guid(currentUuid);
        }

        // Load the anchors using the retrieved UUIDs
        Load(new OVRSpatialAnchor.LoadOptions
        {
            Timeout = 0,
            StorageLocation = OVRSpace.StorageLocation.Local,
            Uuids = uuids
        });
    }

    async void Load(OVRSpatialAnchor.LoadOptions options)
    {
        try
        {
            // Load the unbound anchors asynchronously using the provided options
            var anchors = await OVRSpatialAnchor.LoadUnboundAnchorsAsync(options);

            // If no anchors are loaded, return
            if (anchors == null)
            {
                Debug.LogWarning("No anchors loaded.");
                return;
            }

            // Process each loaded anchor
            foreach (var anchor in anchors)
            {
                // If the anchor is already localized, invoke the _onLoadAnchor action
                if (anchor.Localized)
                {
                    _onLoadAnchor(anchor, true);
                }
                // If the anchor is not localizing, attempt to localize it asynchronously
                else if (!anchor.Localizing)
                {
                    try
                    {
                        bool success = await anchor.LocalizeAsync(60.0); // Increased timeout to 60 seconds
                        _onLoadAnchor(anchor, success);
                    }
                    catch (Exception ex)
                    {
                        _onLoadAnchor(anchor, false);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Could not load Anchors: {ex.Message}");
            Debug.LogError($"Exception details: {ex.ToString()}");
        }
    }

    private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        // If the anchor localization is not successful, return
        if (!success) return;

        // Get the pose of the unbound anchor
        var pose = unboundAnchor.Pose;

        string itemId = PlayerPrefs.GetString(unboundAnchor.Uuid.ToString());

        if (ObjectDatabase.Instance == null)
        {
            Debug.LogError("ObjectDatabase.Instance is null");
            return;
        }

        var item = ObjectDatabase.Instance.GetItemByID(itemId);
        if (item == null)
        {
            Debug.LogError($"Item with ID {itemId} not found in ObjectDatabase");
            return;
        }

        anchorPrefab = item.GetComponent<OVRSpatialAnchor>();
        if (anchorPrefab == null)
        {
            Debug.LogError($"OVRSpatialAnchor component not found on item with ID {itemId}");
            return;
        }

        // Instantiate a new spatial anchor at the pose position and rotation
        var spatialAnchor = Instantiate(anchorPrefab, pose.position, pose.rotation);

        if (!loadedAchors.Contains(spatialAnchor))
            loadedAchors.Add(spatialAnchor);

        // Bind the unbound anchor to the instantiated spatial anchor
        unboundAnchor.BindTo(spatialAnchor);

        // Check if the instantiated spatial anchor has the OVRSpatialAnchor component
        
    }

    public void CompleteSetupMode()
    {
        objectPlacer.isInSetupMode = false;
        objectPlacer.line.enabled = false;
        itemPallette.SetActive(false);
    }


    public void DeleteAllOBjectsFromScene()
    {
        if (loadedAchors.Count > 0)
        {
            for (int i = 0; i < loadedAchors.Count; i++)
            {
                Destroy(loadedAchors[i].gameObject);
            }
            loadedAchors.Clear();
        }

        anchorManager.UnsaveAllAnchors();

        objectPlacer.isInSetupMode = true;
        itemPallette.SetActive(true);
    }
}