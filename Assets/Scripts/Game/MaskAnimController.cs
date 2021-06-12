using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskAnimController : MonoBehaviour
{
    public bool animationFinished;

    // Start is called before the first frame update
    void Awake()
    {
        animationFinished = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void finishedAnimation()
    {
        animationFinished = true;
    }

    public void playSwordSound()
    {
        AkSoundEngine.PostEvent("Sword_Slash", gameObject);
    }
}
