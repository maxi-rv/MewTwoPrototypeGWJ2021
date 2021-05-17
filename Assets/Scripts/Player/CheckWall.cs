using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWall : MonoBehaviour
{
    // VARIABLES
    public bool againstWallLeft;
    public bool againstWallRight;
    [SerializeField] private CheckWallAUX checkRight;
    [SerializeField] private CheckWallAUX checkLeft;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        againstWallLeft = false;
        againstWallRight = false;
    }

    void FixedUpdate() 
    {
        againstWallLeft = checkLeft.getAgainstWall();
        againstWallRight = checkRight.getAgainstWall();
    }
}
