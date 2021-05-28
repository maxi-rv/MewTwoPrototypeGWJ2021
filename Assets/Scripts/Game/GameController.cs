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
    private PlayerController playerController;
    private GameObject playerInstance;  
    private string currentSceneName;
    private bool onPlayingLevel;
    private bool playerDestroyed;

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

        this.loadScene("LevelTest");
        this.instantiatePlayer();
    }

    void Start() 
    {
        //Wwise
        //AkSoundEngine.SetState("Dead_Or_Alive", "Alive");
    }

    // Update is called once per frame
    void Update()
    {
        uiController.currentHP = playerController.currentHP;

            if(playerController.currentHP <= 0)
            {
                if (!playerDestroyed)
                {
                    uiController.currentHP = playerController.currentHP;
                    destroyPlayer();
                    reloadScene(currentSceneName);
                    instantiatePlayer();
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
        playerDestroyed = false;
    }

    private void destroyPlayer()
    {
        cameraController.targets.Remove(playerInstance.transform);
        GameObject.Destroy(playerController.gameObject);
        playerDestroyed = true;
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
