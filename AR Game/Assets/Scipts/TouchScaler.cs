using UnityEngine;
using UnityEngine.InputSystem;

public class TouchScaler : MonoBehaviour
{
    public InputActionReference touchPressReference; // Reference to the TouchPress action
    private Camera arCamera;
    public float scaleFactor = 1.1f; // How much the model will scale with each touch
    public AudioClip sound;
    private AudioSource mySoundSource;

    private void Awake()
    {
        // Ensure the camera is set in Awake to be available if needed before Start.
        arCamera = Camera.main;
        mySoundSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        touchPressReference.action.Enable();
        touchPressReference.action.performed += OnTouchPerformed;
    }

    private void OnDisable()
    {
        touchPressReference.action.performed -= OnTouchPerformed;
        touchPressReference.action.Disable();
    }

    private void OnTouchPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Touch detected");

        if (context.ReadValueAsButton())
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Ray ray = arCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Model"))
                {
                    Transform modelTransform = hit.collider.transform;
                    modelTransform.localScale *= scaleFactor;
                    Debug.Log("Scaled Model");

                    if (modelTransform.localScale.x >= 0.6f)
                    {
                        mySoundSource.clip = sound;
                        mySoundSource.Play();

                        Destroy(modelTransform.gameObject);
                    }

                }
            }
        }
    }

}