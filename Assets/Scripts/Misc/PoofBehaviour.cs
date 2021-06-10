using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoofBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.PostEvent("Enemy_Death", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyItself()
    {
        Destroy(gameObject);
    }
}
