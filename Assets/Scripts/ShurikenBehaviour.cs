using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenBehaviour : MonoBehaviour
{
    public float rotationSpeed;
    public float destructionTime;
    private Collider2D hitBox;

    // Start is called before the first frame update
    void Start()
    {
        // COMPONENTS
        hitBox = gameObject.GetComponent<Collider2D>();

        //Despues de cierto tiempo, la flecha se elimina.
        Invoke("DestroyShuriken", destructionTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0f, 0f, rotationSpeed);
    }

    //Sent when ANOTHER object trigger collider enters a trigger collider attached to this object.
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy")
            DestroyShuriken();
    }

    void DestroyShuriken()
    {
        Destroy(gameObject);
    }
}
