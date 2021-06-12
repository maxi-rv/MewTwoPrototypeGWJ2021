using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{   
    //GameObjects
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject endPoint;

    //Components
    [SerializeField] private Collider2D endPointCollider;

    //Variables
    public bool playerReachedEnd;

    void Awake()
    {
        spawnPoint.GetComponent<SpriteRenderer>().enabled = false;
        endPoint.GetComponent<SpriteRenderer>().enabled = false;

        GameObject.Find("GameController").GetComponent<GameController>().levelController = gameObject.GetComponent<LevelController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerReachedEnd = false;
    }

    // Update is called once per frame
    void Update()
    {
        playerReachedEnd = endPoint.GetComponent<CheckEndPoint>().playerReachedEnd;
    }

    public Vector3 getSpawnPoint()
    {
        return this.spawnPoint.transform.position;
    }
}
