using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneTrackerTouchObjectSpawn : MonoBehaviour
{

    public ARRaycastManager rayMan;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public GameObject spawnPrefab;
    private GameObject spawnedModel;

    bool spawned;

    private void Start()
    {
        spawned = false;
        rayMan = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.touchCount > 0)
        {
            if (rayMan.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = hits[0].pose;

                if (!spawned)
                {
                    spawnedModel = Instantiate(spawnPrefab,hitPose.position,hitPose.rotation);
                    spawned = true;
                }
                else
                {
                    spawnedModel.transform.position = hitPose.position;
                }
            }
        }
        
        /*if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();

            if(spawnedModel == null)
            {
                spawnedModelOnPlane(touchPos);
            }
            else
            {
                spawnedModelOnPlane(touchPos);
            }
        }*/
    }

    void spawnedModelOnPlane(Vector2 touchPos)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        rayMan.Raycast(touchPos, hits, TrackableType.PlaneWithinPolygon);

        if(hits.Count > 0 )
        {
            Pose hitPose = hits[0].pose;

            if(spawnedModel == null)
            {
                spawnedModel = Instantiate(spawnPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                spawnedModel.transform.position = hitPose.position;
            }
        }
    }

}
