using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogBehaviour : MonoBehaviour
{
    private bool playedOnce;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
        playedOnce = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.CompareTag("Ground") && !playedOnce)
        {
            AkSoundEngine.PostEvent("Log", gameObject);
            playedOnce = true;
        }
    }
}
