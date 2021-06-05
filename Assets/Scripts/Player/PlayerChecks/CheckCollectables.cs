using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollectables : MonoBehaviour
{
    // VARIABLES
    public bool foundSomething;
    public string otherTag;

    // Start is called before the first frame update
    void Awake()
    {
        foundSomething = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Compares the hitbox tag with its own tag.
        if(other.gameObject.CompareTag("Food") || other.gameObject.CompareTag("Ammo"))
        {
            foundSomething = true;
            otherTag = other.gameObject.tag;
        }
    }
}
