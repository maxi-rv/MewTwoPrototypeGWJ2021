using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    // VARIABLES
    [SerializeField] private bool onTheGround;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        onTheGround = false;
    }

    // Sent when ANOTHER object trigger collider enters a trigger collider attached to this object.
    void OnTriggerStay2D(Collider2D other)
    {
        //Compares the hitbox tag with its own tag.
        if(other.gameObject.CompareTag("Ground"))
        {
            onTheGround = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Compares the hitbox tag with its own tag.
        if(other.gameObject.CompareTag("Ground"))
        {
            onTheGround = false;
        }
    }

    public bool getIfOnTheGround()
    {
        return onTheGround;
    }

    public void setIfOnTheGround(bool onTheGround)
    {
        this.onTheGround = onTheGround;
    }
}
