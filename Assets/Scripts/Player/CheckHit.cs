using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHit : MonoBehaviour
{
    // VARIABLES
    [SerializeField] private bool isHurt;
    [SerializeField] private string thisCharTag;

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
        }
    }
}
