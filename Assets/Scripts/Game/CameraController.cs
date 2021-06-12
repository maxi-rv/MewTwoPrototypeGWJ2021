using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // COMPONENTS
    private Camera cam;
    
    // VARIABLES
    public List<Transform> targets;
    public float verticalLimitUp;
    public float verticalLimitDown;
    public float horizontalLimitLeft;
    public float horizontalLimitRight;
    [SerializeField] private float delayToCenter;
    private float maxZoom;
    private float minZoom;
    private float zoomLimiter;
    [SerializeField] private Vector3 offset;
    private Vector3 velocity;
    private Bounds bounds;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        cam = GetComponent<Camera>();
        offset = new Vector3(0f, 0f, -5f);
        zoomLimiter = 50f;
    }

    void Update() 
    {
        /*
        if(velocity.normalized.y >= 1f)
        {
            offset =  new Vector3(0f,-0.5f,-5f);
        }
        else if(velocity.normalized.y <= -1f)
        {
            offset =  new Vector3(0f,0.5f,-5f);
        }
        else
        {
            offset =  new Vector3(0f,0f,-5f);
        }
        */
    }

    // FixedUpdate is called multiple times per frame.
    void FixedUpdate()
    {
        if(targets.Count != 0)
        {
            //Creates a Bounds: a box aligned with coordinate axes and fully enclosing some object.
            bounds = new Bounds(targets[0].position, new Vector3(1f, 1f, 1f));

            //Takes every target in the array and encapsulates it in the Bounds.
            for (int i=0; i<targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }

            moveCamera();
        }
        
        
    }

    void moveCamera()
    {
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

        //Adjusts the camera to the horizontal limits
        if(desiredPosition.x > horizontalLimitRight)
        {
            desiredPosition = new Vector3(horizontalLimitRight, desiredPosition.y, desiredPosition.z);
        }
        else if(desiredPosition.x < horizontalLimitLeft)
        {
            desiredPosition = new Vector3(horizontalLimitLeft, desiredPosition.y, desiredPosition.z);
        }

        //Moves the camera from the CurrentPosition to the desiredPosition
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, delayToCenter);
    }

    void zoomCamera()
    {
        //Gets the desiredZoom from the width of the Bounds, and limits it by Min and Max Zoom
        float desiredZoom = Mathf.Lerp(minZoom, maxZoom, bounds.size.x/zoomLimiter);

        //Sets the desiredZoom on the camera
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, desiredZoom, Time.deltaTime);
    }
}
