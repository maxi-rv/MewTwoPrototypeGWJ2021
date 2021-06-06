using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //Singleton
    private static GameController instance;

    //COMPONENTS
    [SerializeField] private UIController uiController;
    [SerializeField] private CameraController cameraController;
    
    //VARIABLES
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private string firstSceneName;
    private PlayerController playerController;
    private GameObject playerInstance;  
    private string currentSceneName;
    private bool onPlayingLevel;
    private bool playerIsDead;

    void Awake()
    {
        if(instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        onPlayingLevel = false;
        uiController.showStartMessage();
    }

    void Start() 
    {
        //Wwise
        //AkSoundEngine.SetState("Dead_Or_Alive", "Alive");
    }

    // Update is called once per frame
    void Update()
    {
        //Press Enter to Start
        if(Input.GetKey(KeyCode.Return) && !onPlayingLevel)
        {
            //Loads first Scene
            currentSceneName = firstSceneName;
            loadScene(currentSceneName);
            onPlayingLevel = true;

            //Instantiates player
            instantiatePlayer();

            //Updates HUD
            uiController.disableMessage();
            uiController.activateBar();
            uiController.currentHP = playerController.currentHP;
            uiController.enableShurikenCounter();
            uiController.setShurikenCounter(playerController.shurikenCant);
        }
        else if(onPlayingLevel) //While playing a level
        {
            uiController.currentHP = playerController.currentHP;
            uiController.setShurikenCounter(playerController.shurikenCant);

            //Player Dies
            if(playerController.currentHP <= 0)
            {
                if (playerIsDead && Input.GetKey(KeyCode.Return))
                {
                    uiController.disableMessage();
                    destroyPlayer();
                    reloadScene(currentSceneName);
                    instantiatePlayer();
                }
                else
                {
                    uiController.showRetryMessage();
                    playerIsDead = true;
                }
            }
        }     
    }

    private void instantiatePlayer()
    {
        // Instantiating the Player
        Vector3 position = new Vector3(0f, 0f, 0f);
        Quaternion rotation = new Quaternion(0f, 0f, 0f, 0f);
        playerInstance = Instantiate(playerPrefab, position, rotation);

        // Setting some variables
        playerController = playerInstance.GetComponent<PlayerController>();
        cameraController.targets.Add(playerInstance.transform);
        uiController.maxHP = playerController.maxHP;
        uiController.currentHP = playerController.currentHP;
        uiController.activateBar();
        playerIsDead = false;
    }

    private void destroyPlayer()
    {
        cameraController.targets.Remove(playerInstance.transform);
        GameObject.Destroy(playerController.gameObject);
    }

    private void loadScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        currentSceneName = scene;
    }

    private void reloadScene(string scene)
    {
        SceneManager.UnloadSceneAsync(scene);
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }

    private void unloadScene(string scene)
    {
        destroyPlayer();
        SceneManager.UnloadSceneAsync(scene);
    }
}
