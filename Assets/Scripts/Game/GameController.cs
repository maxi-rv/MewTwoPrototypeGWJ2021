using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Koffie.SimpleTasks;

public class GameController : MonoBehaviour
{
    //Singleton
    private static GameController instance;

    //COMPONENTS
    [SerializeField] private UIController uiController;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private MaskAnimController maskAnimController;
    public LevelController levelController;

    //Wwise Event IDs
    private uint gameMusicID;
    private uint ambient1ID;
    private uint ambient2ID;
    
    //VARIABLES
    [SerializeField] private GameObject playerPrefab;
    private PlayerController playerController;
    private GameObject playerInstance;  
    private string currentSceneName;
    private bool onPlayingLevel;
    private bool playerIsDead;
    private int currentLevel;
    private bool playerSpawnPositionSetted;
    private bool flagSetted;

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

        //Preparing first message
        onPlayingLevel = false;
        playerSpawnPositionSetted = false;
        flagSetted = false;
        currentLevel = -1;
        uiController.showStartMessage();

        //Preparing Game Music Event and State
        AkSoundEngine.SetState("Dead_Or_Alive", "None");
        gameMusicID = AkSoundEngine.PostEvent("Game_Music", gameObject);
    }

    void Start() 
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if(maskAnimController.animationFinished && !flagSetted)
        {
            uiController.playFade();
            STasks.Do(() => uiController.disableMaskAnimation(), after: 1.0f);
            flagSetted = true;
        }

        //Press Enter to Start
        if(Input.GetKey(KeyCode.Return) && !onPlayingLevel && currentLevel!=0 && maskAnimController.animationFinished)
        {
            currentLevel = 0;
            uiController.playFade();

            STasks.Do(() => loadIntroScene(), after: 1.0f);
            STasks.Do(() => uiController.playFade(), after: 21.0f);
            STasks.Do(() => loadFirstLevel(), after: 22.0f);
        }
        else if(onPlayingLevel) //While playing a level
        {
            if(levelController!=null && !playerSpawnPositionSetted)
            {
                setPlayerPosition(levelController.getSpawnPoint());
                playerSpawnPositionSetted = true;
            }

            uiController.currentHP = playerController.currentHP;
            uiController.setShurikenCounter(playerController.shurikenCant);

            //Check if player finishes current level!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if(levelController!=null && levelController.playerReachedEnd)
            {
                levelController.playerReachedEnd = false;
                levelController = null;

                if(currentLevel==1)
                {
                    uiController.playFade();
                    STasks.Do(() => unloadScene(currentSceneName), after: 1.0f);
                    STasks.Do(() => loadSecondLevel(), after: 1.05f);
                }
                if(currentLevel==2)
                {
                    uiController.playFade();
                    STasks.Do(() => unloadScene(currentSceneName), after: 1.0f);
                    STasks.Do(() => loadThirdLevel(), after: 1.05f);
                }
                if(currentLevel==3)
                {
                    //QUE HACEMOS?!
                    uiController.playFade();
                    STasks.Do(() => Application.Quit(), after: 1.0f);
                }
            }

            //if Player Dies
            if(playerController.currentHP <= 0)
            {
                if (playerIsDead && Input.GetKey(KeyCode.Return))
                {
                    //Resets the current level
                    uiController.disableMessage();
                    destroyPlayer();
                    levelController = null;
                    reloadScene(currentSceneName);

                    //Instantiates player and sets its position
                    instantiatePlayer();
                    if(levelController!=null && !playerSpawnPositionSetted)
                    {
                        setPlayerPosition(levelController.getSpawnPoint());
                        playerSpawnPositionSetted = true;
                    }

                    //Set Wwise state for Game Music
                    if(currentLevel==1)
                    {
                        AkSoundEngine.SetState("Dead_Or_Alive", "Tutorial");
                        gameMusicID = AkSoundEngine.PostEvent("Game_Music", gameObject);
                    }  
                    else if(currentLevel==2)
                    {
                        AkSoundEngine.SetState("Dead_Or_Alive", "Alive");
                        gameMusicID = AkSoundEngine.PostEvent("Game_Music", gameObject);
                    }     
                    else if(currentLevel==3)
                    {
                        AkSoundEngine.SetState("Dead_Or_Alive", "Level3");
                        gameMusicID = AkSoundEngine.PostEvent("Game_Music", gameObject);
                    }
                        
                }
                else
                {   
                    //Shows retry message
                    uiController.showRetryMessage();
                    playerIsDead = true;

                    //Set Wwise state for Game Music
                    AkSoundEngine.SetState("Dead_Or_Alive", "Dead");
                }
            }
        }     
    }

    private void loadIntroScene()
    {
        //Loads intro Scene
        currentSceneName = "Intro";
        loadScene(currentSceneName);
        
        //Post intro music Event
        gameMusicID = AkSoundEngine.PostEvent("Game_Music", gameObject);
        AkSoundEngine.SetState("Dead_Or_Alive", "Tutorial");
        gameMusicID = AkSoundEngine.PostEvent("Game_Music", gameObject);

        //Updates HUD
        uiController.disableMessage();
        uiController.disableBackgroundMenu();
    }

    private void loadFirstLevel()
    {
        unloadScene(currentSceneName);
        currentLevel = 1;
        loadScene("Level"+currentLevel);
        onPlayingLevel = true;

        //Set Camera
        cameraController.verticalLimitDown = 1f;
        cameraController.verticalLimitUp = 3f;
        cameraController.horizontalLimitRight = 25f;
        cameraController.horizontalLimitLeft = -2f;

        //Instantiates player and prepares to set its position
        instantiatePlayer();
        playerSpawnPositionSetted = false;

        //Set Wwise state for Game Music
        AkSoundEngine.SetState("Dead_Or_Alive", "Tutorial");
        gameMusicID = AkSoundEngine.PostEvent("Game_Music", gameObject);
    }

    private void loadSecondLevel()
    {
        currentLevel = 2;
        loadScene("Level"+currentLevel);
        onPlayingLevel = true;

        //Set Camera
        cameraController.verticalLimitDown = 1f;
        cameraController.verticalLimitUp = 3f;
        cameraController.horizontalLimitRight = 142f;
        cameraController.horizontalLimitLeft = -2f;

        //Prepares to set player position
        playerSpawnPositionSetted = false;

        //Set Wwise state for Game Music
        AkSoundEngine.SetState("Dead_Or_Alive", "Alive");
        gameMusicID = AkSoundEngine.PostEvent("Game_Music", gameObject);
    }

    private void loadThirdLevel()
    {
        currentLevel = 3;
        loadScene("Level"+currentLevel);
        onPlayingLevel = true;

        //Set Camera
        cameraController.verticalLimitDown = -1.2f;
        cameraController.verticalLimitUp = 0f;
        cameraController.horizontalLimitRight = 51f;
        cameraController.horizontalLimitLeft = 0f;

        //Prepares to set player position
        playerSpawnPositionSetted = false;

        //Set Wwise state for Game Music
        AkSoundEngine.SetState("Dead_Or_Alive", "Level3");
        gameMusicID = AkSoundEngine.PostEvent("Game_Music", gameObject);
        ambient2ID = AkSoundEngine.PostEvent("Ambiente2", gameObject);
    }

    private void instantiatePlayer()
    {
        // Instantiating the Player
        Vector3 position = new Vector3(0f, 0f, 0f);
        Quaternion rotation = new Quaternion(0f, 0f, 0f, 0f);
        playerInstance = Instantiate(playerPrefab, position, rotation);
        playerInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);

        // Setting some variables
        playerController = playerInstance.GetComponent<PlayerController>();
        cameraController.targets.Add(playerInstance.transform);
        uiController.maxHP = playerController.maxHP;
        uiController.currentHP = playerController.currentHP;
        uiController.activateBar();
        uiController.enableShurikenCounter();
        uiController.setShurikenCounter(playerController.shurikenCant);
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
        SceneManager.UnloadSceneAsync(scene);
        AkSoundEngine.StopAll();
    }

    //Places Player on SpawnPoint
    //PLAYER MUST ALREDY BE INSTANTIATED
    private void setPlayerPosition(Vector3 pos)
    {
        playerInstance.transform.position = pos;
    }
}
