using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    // VARIABLES
    public bool onTheGround;
    public string otherTag;
    public int otherInstanceID;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        onTheGround = false;
    }

    // Sent when ANOTHER object trigger collider enters a trigger collider attached to this object.
    void OnTriggerEnter2D(Collider2D other)
    {
        //Compares the hitbox tag with its own tag.
        if(other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform"))
        {
            onTheGround = true;
            otherTag = other.gameObject.tag;
            otherInstanceID = other.gameObject.GetInstanceID();
            AkSoundEngine.PostEvent("Walking", gameObject);
        }
    }
}
