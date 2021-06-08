using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHit : MonoBehaviour
{
    // VARIABLES
    public bool isHurt;
    private string thisCharTag;
    public string otherTag;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        isHurt = false;
        thisCharTag = gameObject.tag;
    }

    // Sent when ANOTHER object trigger collider enters a trigger collider attached to this object.
    void OnTriggerEnter2D(Collider2D other)
    {
        //Compares the hitbox tag with its own tag.
        if(!other.gameObject.CompareTag(thisCharTag))
        {
            isHurt = true;
            otherTag = other.tag;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //Compares the hitbox tag with its own tag.
        if(!other.gameObject.CompareTag(thisCharTag))
        {
            isHurt = true;
            otherTag = other.tag;
        }
    }
}
