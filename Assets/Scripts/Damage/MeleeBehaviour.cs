using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBehaviour : MonoBehaviour
{
    // VARIABLES
    private string thisCharTag;
    public string otherTag;
    

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        thisCharTag = gameObject.tag;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Compares the hitbox tag with its own tag.
        if(!other.gameObject.CompareTag(thisCharTag))
        {
            AkSoundEngine.PostEvent("Sword_Hit", gameObject);
        }
    }
}
