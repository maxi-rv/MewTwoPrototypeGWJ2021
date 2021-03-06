using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenBehaviour : MonoBehaviour
{
    public float rotationSpeed;
    public float destructionTime;
    public float shurikenSpeed;
    public Vector2 velocity;
    private Collider2D hitBox;
    private string thisTag;

    // Start is called before the first frame update
    void Awake()
    {
        // COMPONENTS
        hitBox = gameObject.GetComponent<Collider2D>();

        //Despues de cierto tiempo, la flecha se elimina.
        Invoke("DestroyShuriken", destructionTime);

        thisTag = gameObject.tag;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed);
        transform.Translate(velocity*Time.deltaTime*shurikenSpeed, Space.World);
    }

    //Sent when ANOTHER object trigger collider enters a trigger collider attached to this object.
    void OnTriggerEnter2D(Collider2D other)
    {
        if(thisTag != other.tag)
        {
            DestroyShuriken();
            AkSoundEngine.PostEvent("Shuriken_Hit", gameObject);
        }
    }

    void DestroyShuriken()
    {
        Destroy(gameObject);
    }
}
