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
    public GameObject soldier;
    private GameObject spawnedGuy;

    [Header("Sound")]

    public AudioClip PlaceSound, DestroySound, TrumpetSound;
    private AudioSource audioSrc;

    [Header("Variables")]
    private bool spawned, solSpawned;

    public float moveSpeed = 5f;


    [SerializeField] private float scaleF = 1.1f;

    private void Awake()
    {
        spawned = false;
        solSpawned = false;
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

        if (rayMan.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon) && !spawned && !solSpawned)
        {
                        
            foreach(ARRaycastHit hit in hits)
            {
                
                Pose pose = hit.pose;
                spawnedGuy = Instantiate(soldier, pose.position, pose.rotation);
                audioSrc.PlayOneShot(TrumpetSound);

                solSpawned = true;
                break;

            }

        }
        else if(rayMan.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon) && !spawned && solSpawned)
        {
            foreach (ARRaycastHit hit in hits)
            {

                Pose pose = hit.pose;
                GameObject obj = Instantiate(soldier, pose.position, pose.rotation);
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
                Debug.Log($"model scale " + modelTransform.localScale.x);

                Vector3 direction = spawnedGuy.transform.position - modelTransform.position;
                float distance = direction.magnitude;

                if (distance <= 0.1) return;

                direction.Normalize();

                float currSpeed = Mathf.Lerp(0, moveSpeed, Mathf.Clamp01(distance / 10f));

                while(distance >= 0.1)
                {
                    spawnedGuy.transform.position += direction * currSpeed * Time.deltaTime;
                }


                /*modelTransform.localScale *= scaleF;
                Debug.Log("Model scaled" + modelTransform.localScale);

                if (modelTransform.localScale.x >= 6f)
                {
                    audioSrc.PlayOneShot(DestroySound);

                    GameObject desPrefab = Instantiate(destroyedPrefab, modelTransform.position, modelTransform.rotation);
                    desPrefab.transform.localScale = modelTransform.localScale;
                    Destroy(modelTransform.gameObject);

                    spawned = false;

                    Debug.Log("Model Destroyed");
                }*/
            }

        }
    }

}
