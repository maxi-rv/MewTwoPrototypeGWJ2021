using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleShuriken : MonoBehaviour
{
    public float rotationSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            GameObject.Destroy(gameObject);
        }
    }
}
