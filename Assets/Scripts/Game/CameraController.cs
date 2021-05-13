using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // COMPONENTS
    private Camera cam;
    
    // VARIABLES
    [SerializeField] private List<Transform> targets;
    [SerializeField] private float verticalLimitUp;
    [SerializeField] private float verticalLimitDown;
    [SerializeField] private float delayToCenter;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private float zoomLimiter;
    private Vector3 offset;
    private Vector3 velocity;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        cam = GetComponent<Camera>();
        offset = new Vector3(0f, 0f, -5f);
        zoomLimiter = 50f;
    }

    // FixedUpdate is called multiple times per frame.
    void FixedUpdate()
    {
        if(targets.Count != 0)
        {
            //Creates a Bounds: a box aligned with coordinate axes and fully enclosing some object.
            var bounds = new Bounds(targets[0].position, new Vector3(1f, 1f, 1f));

            //Takes every target in the array and encapsulates it in the Bounds.
            for (int i=0; i<targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }

            //Gets the center of the encapsulated targets
            Vector3 centerPosition = bounds.center;

            //nudges the center by an offset alredy established on unity.
            Vector3 desiredPosition = centerPosition + offset;

            //Adjusts the camera to the vertical limits
            if(desiredPosition.y > verticalLimitUp)
            {
                desiredPosition = new Vector3(desiredPosition.x, verticalLimitUp, desiredPosition.z);
            }
            else if(desiredPosition.y < verticalLimitDown)
            {
                desiredPosition = new Vector3(desiredPosition.x, verticalLimitDown, desiredPosition.z);
            }

            //Moves the camera from the CurrentPosition to the desiredPosition
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, delayToCenter);

            //Gets the desiredZoom from the width of the Bounds, and limits it by Min and Max Zoom
            float desiredZoom = Mathf.Lerp(maxZoom, minZoom, bounds.size.x/zoomLimiter);

            //Sets the desiredZoom on the camera
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, desiredZoom, Time.deltaTime);
        }
        
    }
}
