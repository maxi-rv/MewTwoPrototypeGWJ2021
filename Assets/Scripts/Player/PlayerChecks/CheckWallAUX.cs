using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWallAUX : MonoBehaviour
{
    private bool againstWall;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        againstWall = false;
    }

    // Sent when ANOTHER object trigger collider enters a trigger collider attached to this object.
    void OnTriggerStay2D(Collider2D other)
    {
        //Compares the hitbox tag with its own tag.
        if(other.gameObject.CompareTag("Wall"))
        {
            againstWall = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //Compares the hitbox tag with its own tag.
        if(other.gameObject.CompareTag("Wall"))
        {
            againstWall = false;
        }
    }

    public bool getAgainstWall()
    {
        return againstWall;
    }
}
