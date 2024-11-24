using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Include the UI namespace

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
    private AudioSource audioSource;

    void Start()
    {
        // Initially freeze the game and show the start screen
        FreezeGame(true);
        startGameUI.SetActive(true); // show the start screen UI
        resumeGameUI.SetActive(false);  // hide the pause menu UI
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Check for user input to start the game
        if (currentState == GameState.Menu && Input.GetKeyDown(KeyCode.S))
        {
            StartGame();
            audioSource.Play();
        }
        // Pause the game when in Playing state and Escape is pressed
        else if (currentState == GameState.Playing && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
            audioSource.Stop();
        }
        // Resume the game when in Paused state and Escape is pressed
        else if (currentState == GameState.Paused && Input.GetKeyDown(KeyCode.R))
        {
            ResumeGame();
            audioSource.Play();
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
