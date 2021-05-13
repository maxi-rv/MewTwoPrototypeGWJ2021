using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWall : MonoBehaviour
{
    // VARIABLES
    [SerializeField] private bool againstWallLeft;
    [SerializeField] private bool againstWallRight;
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

    // Returns if it detects collision to a wall from the right.
    public bool getAgainstWallRight()
    {
        return againstWallRight;
    }

    // Returns if it detects collision to a wall from the left.
    public bool getAgainstWallLeft()
    {
        return againstWallLeft;
    }
}
