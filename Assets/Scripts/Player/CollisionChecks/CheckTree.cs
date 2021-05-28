using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTree : MonoBehaviour
{
    // VARIABLES
    public bool overATree;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        overATree = false;
    }

    // Sent when ANOTHER object trigger collider enters a trigger collider attached to this object.
    void OnTriggerStay2D(Collider2D other)
    {
        //Compares the hitbox tag with its own tag.
        if(other.gameObject.CompareTag("Tree"))
        {
            overATree = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Compares the hitbox tag with its own tag.
        if(other.gameObject.CompareTag("Tree"))
        {
            overATree = false;
        }
    }
}
