using UnityEngine;

public enum GameState
{
    Menu,
    Playing,
    Paused
}

public class GameManagerController : MonoBehaviour
{
    public GameState currentState = GameState.Menu;

    // UI Elements for Start Screen
    public GameObject startGameUI;
    
    // UI Elements for Pause Menu
    public GameObject resumeGameUI;
    public AudioSource backgroundMusic;
    public AudioSource pauseSound;
    public AudioSource resumeSound;

    void Start()
    {
        // Initially freeze the game and show the start screen
        FreezeGame(true);
        startGameUI.SetActive(true); // show the start screen UI
        resumeGameUI.SetActive(false);  // hide the pause menu UI
    }

    void OnEnable()
    {
        PlayerController.OnPlayerRespawn += RestartGame;  // react only to respawn
    }

    void OnDisable()
    {
        PlayerController.OnPlayerRespawn -= RestartGame;
    }


    void Update()
    {
        // Check for user input to start the game
        if (currentState == GameState.Menu && Input.GetKeyDown(KeyCode.S))
        {
            StartGame();
            backgroundMusic.Play();
        }
        // Pause the game when in Playing state and Escape is pressed
        else if (currentState == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
            pauseSound.Play();
            backgroundMusic.Pause();
        }
        // Resume the game when in Paused state and Escape is pressed
        else if (currentState == GameState.Paused && Input.GetKeyDown(KeyCode.R))
        {
            ResumeGame();
            resumeSound.Play();
            backgroundMusic.Play();
        }
    }

    void StartGame()
    {
        currentState = GameState.Playing;
        Debug.Log("Game Started!");
        
        // Hide start screen UI and unfreeze the game
        startGameUI.SetActive(false);
        FreezeGame(false);
    }

    void PauseGame()
    {
        currentState = GameState.Paused;
        Debug.Log("Game Paused!");
        
        // Show pause menu UI and freeze the game
        resumeGameUI.SetActive(true);
        FreezeGame(true);
    }

    void ResumeGame()
    {
        currentState = GameState.Playing;
        Debug.Log("Game Resumed!");
        
        // Hide pause menu UI and unfreeze the game
        resumeGameUI.SetActive(false);
        FreezeGame(false);
    }

    void RestartGame()
    {
        Debug.Log("Restarting Game...");
        currentState = GameState.Playing;
        FreezeGame(false);  // unfreeze the game
    }

    void FreezeGame(bool freeze)
    {
        if (freeze)
        {
            Time.timeScale = 0;  // freeze time (everything is paused)
        }
        else
        {
            Time.timeScale = 1;  // unfreeze time (everything resumes)
        }
    }
}
