using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OriginalAnchorLoader : MonoBehaviour
{
    private OVRSpatialAnchor anchorPrefab;
    private SpatialAnchorManager spatialAnchorManager;
    Action<OVRSpatialAnchor.UnboundAnchor, bool> _onLoadAnchor;

    private void Awake()
    {
        // Get the SpatialAnchorManager component attached to this game object
        spatialAnchorManager = GetComponent<SpatialAnchorManager>();

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
            if (anchors == null) return;

            Debug.Log("Load Anchors Success");

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
                    _onLoadAnchor(anchor, await anchor.LocalizeAsync(20.0));
                }
            }
        }
        catch
        {
            Debug.Log("Could not load Anchors");
        }
    }

    private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        // If the anchor localization is not successful, return
        if (!success) return;

        // Get the pose of the unbound anchor
        var pose = unboundAnchor.Pose;

        string itemId = PlayerPrefs.GetString(unboundAnchor.Uuid.ToString());

        anchorPrefab = ItemDatabase.Instance.GetItemById(itemId).GetComponent<OVRSpatialAnchor>();

        // Instantiate a new spatial anchor at the pose position and rotation
        var spatialAnchor = Instantiate(anchorPrefab, pose.position, pose.rotation);

        // Bind the unbound anchor to the instantiated spatial anchor
        unboundAnchor.BindTo(spatialAnchor);

        // Check if the instantiated spatial anchor has the OVRSpatialAnchor component
        if (spatialAnchor.TryGetComponent<OVRSpatialAnchor>(out var anchor))
        {
            // Get the UUID text and saved status text components
            var uuidText = spatialAnchor.GetComponentInChildren<TextMeshProUGUI>();
            var savedStatusText = spatialAnchor.GetComponentsInChildren<TextMeshProUGUI>()[1];

            // Update the UUID text with the spatial anchor's UUID
            uuidText.text = $"UUID: {spatialAnchor.Uuid.ToString()}";

            // Update the saved status text to indicate that the anchor is loaded from the device
            savedStatusText.text = "Loaded from Device";
        }
    }
}