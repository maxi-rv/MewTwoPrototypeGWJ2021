using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEndPoint : MonoBehaviour
{
    public bool playerReachedEnd;

    // Start is called before the first frame update
    void Start()
    {
        playerReachedEnd = false;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            playerReachedEnd = true;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
}
