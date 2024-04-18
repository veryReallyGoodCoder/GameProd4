using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageTracking : MonoBehaviour
{

    [SerializeField]
    private ARTrackedImageManager _imageMan;

    public GameObject[] RefPrefabs;

    private Dictionary<string, GameObject> _prefabDict = new Dictionary<string, GameObject>();

    private GameObject _currSpawnedObjects = null;

    private void Awake()
    {
        
        foreach(var prefab in RefPrefabs)
        {
            _prefabDict[prefab.name] = prefab;
        }

        if(_imageMan == null)
        {
            Debug.LogError("No Images");
        }

    }

    private void OnEnable()
    {
        _imageMan.trackedImagesChanged += HandleTrackImagesChanged;
    }

    void OnDisbale()
    {
        _imageMan.trackedImagesChanged -= HandleTrackImagesChanged;
    }

    void HandleTrackImagesChanged(ARTrackedImagesChangedEventArgs arg)
    {
        foreach(var addedImage in arg.added)
        {
            SpawnOrReplaceObjectForImage(addedImage);
        }

        foreach(var updatedImage in arg.added)
        {
            if(updatedImage.trackingState == TrackingState.Tracking)
            {
                UpdateARObjectTracking(updatedImage);
            }
        }

        foreach(ARTrackedImage image in arg.removed)
        {
            RemoveObjectForImage(image);
        }

    }

    void SpawnOrReplaceObjectForImage(ARTrackedImage trackedImage)
    {
        if(_currSpawnedObjects != null)
        {
            Destroy(_currSpawnedObjects);
        }

        string imageName = trackedImage.referenceImage.name;
        if(_prefabDict.TryGetValue(imageName, out GameObject prefab))
        {
            _currSpawnedObjects = Instantiate(prefab, trackedImage.transform.position, Quaternion.identity);
            Debug.Log($"Spawned of Replaced Prefab for Image: {imageName}");
        }
        else
        {
            Debug.LogWarning($"No Prefab Found for Image: {imageName}");
        }
    }

    void UpdateARObjectTracking(ARTrackedImage trackedImage)
    {
        if(_currSpawnedObjects != null)
        {
            _currSpawnedObjects.transform.position = trackedImage.transform.position;
            Debug.Log($"Updated Position of Current Object");
        }
    }

    void RemoveObjectForImage(ARTrackedImage trackedImage)
    {
        if(_currSpawnedObjects != null)
        {
            Destroy(_currSpawnedObjects);
            _currSpawnedObjects = null;
            Debug.Log($"Removed Prefab for Image: {trackedImage.referenceImage.name}");
        }
    }

}
