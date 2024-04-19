using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaneTrackerTouchObjectSpawn : MonoBehaviour
{
    [Header("Touch Input")]
    public ARRaycastManager rayMan;
    private ARPlaneManager planeMan;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    [Header("Prefabs")]
    public GameObject spawnPrefab;
    public GameObject destroyedPrefab;
    private GameObject spawnedModel;

    [Header("Sound")]
    public AudioClip PlaceSound, DestroySound;
    private AudioSource audioSrc;

    [Header("Variables")]
    private bool spawned;

    [SerializeField] private float scaleF = 1.1f;

    private void Awake()
    {
        spawned = false;
        rayMan = GetComponent<ARRaycastManager>();
        audioSrc = GetComponent<AudioSource>();
    }

    
    // Update is called once per frame
    void Update()
    {
        
        /*if(Input.touchCount > 0)
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
        }*/
        
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

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
        EnhancedTouch.Touch.onFingerDown += TouchScale;
    }
     private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        EnhancedTouch.Touch.onFingerDown -= TouchScale;
    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;

        if (rayMan.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon) && !spawned)
        {
                        
            foreach(ARRaycastHit hit in hits)
            {
                
                Pose pose = hit.pose;
                GameObject obj = Instantiate(spawnPrefab, pose.position, pose.rotation);
                audioSrc.PlayOneShot(PlaceSound);

                spawned = true;
                break;

            }

        }

    }

    private void TouchScale(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;

        Debug.Log("Scale Touch");

        Vector2 touchPos = finger.currentTouch.screenPosition;
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Model"))
            {
                Debug.Log("Model hit");

                Transform modelTransform = hit.collider.transform;
                modelTransform.localScale *= scaleF;
                Debug.Log("Model scaled");

                if (modelTransform.localScale.x >= 6f)
                {
                    audioSrc.PlayOneShot(DestroySound);

                    GameObject desPrefab = Instantiate(destroyedPrefab, modelTransform.position, modelTransform.rotation);
                    Destroy(modelTransform.gameObject);

                    spawned = false;

                    Debug.Log("Model Destroyed");
                }
            }

        }
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
