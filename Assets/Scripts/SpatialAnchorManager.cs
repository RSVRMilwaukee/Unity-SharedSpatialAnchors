using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpatialAnchorManager : MonoBehaviour
{
    public const string NumUuidsPlayerPref = "numUuids";

    private Canvas canvas;
    

    private List<OVRSpatialAnchor> anchors = new List<OVRSpatialAnchor>();
    private OVRSpatialAnchor lastCreatedAnchor;
    private AnchorLoader anchorLoader;

    private void Awake()
    {
        // Get the AnchorLoader component attached to this game object
        anchorLoader = GetComponent<AnchorLoader>();
    }

    private void Start()
    {
        // Load previously saved anchors on startup
        //LoadSavedAnchors();
    }

    //private void Update()
    //{
    //    // Create a new spatial anchor when the right touch controller's primary index trigger is pressed
    //    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
    //    {
    //        CreateSpatialAnchor();
    //    }

    //    // Save the last created anchor when the right touch controller's button one is pressed
    //    if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
    //    {
    //        SaveLastCreatedAnchor();
    //    }

    //    // Unsave the last created anchor when the right touch controller's button two is pressed
    //    if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
    //    {
    //        UnsaveLastCreatedAnchor();
    //    }

    //    // Unsave all anchors when the right touch controller's primary hand trigger is pressed
    //    if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
    //    {
    //        UnsaveAllAnchors();
    //    }

    //    // Load saved anchors when the right touch controller's primary thumbstick down is pressed
    //    if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown, OVRInput.Controller.RTouch))
    //    {
    //        LoadSavedAnchors();
    //    }
    //}

    public void LoadSavedAnchors()
    {
        // Load anchors by their UUIDs using the AnchorLoader component
        anchorLoader.LoadAnchorByUuid();
    }

    public void UnsaveAllAnchors()
    {
        // Unsave all anchors in the list
        foreach (var anchor in anchors)
        {
            UnsaveAnchor(anchor);
        }
        anchors.Clear();

        // Clear all saved anchor IDs from PlayerPrefs
        ClearAllIdsFromPlayerPrefs();
    }

    void ClearAllIdsFromPlayerPrefs()
    {
        // Check if there are any saved anchor IDs in PlayerPrefs
        if (PlayerPrefs.HasKey(NumUuidsPlayerPref))
        {
            int playerNumUuids = PlayerPrefs.GetInt(NumUuidsPlayerPref);

            // Delete each saved anchor ID from PlayerPrefs
            for (int i = 0; i < playerNumUuids; i++)
            {
                PlayerPrefs.DeleteKey("uuid" + i);
            }

            // Delete the key storing the number of saved anchor IDs
            PlayerPrefs.DeleteKey(NumUuidsPlayerPref);
            PlayerPrefs.Save();
        }
    }

    async void UnsaveAnchor(OVRSpatialAnchor anchor)
    {
        try
        {
            // Erase the anchor asynchronously
            await anchor.EraseAsync();
            Console.WriteLine("Erase was successful.");

            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    void SaveIdToPlayerPrefs(OVRSpatialAnchor anchor)
    {
        // Check if the number of saved anchor IDs key exists in PlayerPrefs
        if (!PlayerPrefs.HasKey(NumUuidsPlayerPref))
        {
            // Initialize the number of saved anchor IDs to 0
            PlayerPrefs.SetInt(NumUuidsPlayerPref, 0);
        }

        // Get the current number of saved anchor IDs
        int playerNumUuids = PlayerPrefs.GetInt(NumUuidsPlayerPref);

        // Save the new anchor ID with a unique key
        PlayerPrefs.SetString("uuid" + playerNumUuids, anchor.Uuid.ToString());

        Item item = anchor.GetComponent<Item>();

        PlayerPrefs.SetString(anchor.Uuid.ToString(), item.id);

        // Increment the number of saved anchor IDs and update PlayerPrefs
        PlayerPrefs.SetInt(NumUuidsPlayerPref, ++playerNumUuids);
    }

    public async void SaveLastCreatedAnchor()
    {
        try
        {
            // Save the last created anchor asynchronously
            await lastCreatedAnchor.SaveAsync();
            Console.WriteLine("Save was successful.");

            // Update the anchor's status text to indicate it's saved
            //savedStatusText.text = "Saved";

            // Save the anchor's ID to PlayerPrefs
            SaveIdToPlayerPrefs(lastCreatedAnchor);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public async void UnsaveLastCreatedAnchor()
    {
        try
        {
            // Erase the last created anchor asynchronously
            await lastCreatedAnchor.EraseAsync();
            Console.WriteLine("Erase was successful.");

            // Update the anchor's status text to indicate it's not saved
            //savedStatusText.text = "Not Saved";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public OVRSpatialAnchor CreateSpatialAnchor(OVRSpatialAnchor anchorPrefab, Vector3 position, Quaternion rotation)
    {
        // Instantiate a new spatial anchor at the position and rotation of the right touch controller

        OVRSpatialAnchor workingAnchor = Instantiate(anchorPrefab, position, rotation);

        // Get the Canvas component and its child text components
        canvas = workingAnchor.gameObject.GetComponentInChildren<Canvas>();
        //uuidText = canvas.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        //savedStatusText = canvas.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        // Start the coroutine to handle the anchor creation process
        StartCoroutine(AnchorCreated(workingAnchor));
        return workingAnchor;
    }

    IEnumerator AnchorCreated(OVRSpatialAnchor workingAnchor)
    {
        // Wait until the anchor is created and localized
        yield return new WaitUntil(() => workingAnchor.Created && workingAnchor.Localized);

        // Get the anchor's UUID
        Guid anchorGuid = workingAnchor.Uuid;

        // Add the anchor to the list and set it as the last created anchor
        anchors.Add(workingAnchor);
        lastCreatedAnchor = workingAnchor;

        // Update the anchor's UUID and status text
        //uuidText.text = $"UUID: {anchorGuid}";
        //savedStatusText.text = "Not Saved";
    }
}