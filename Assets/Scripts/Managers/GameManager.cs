using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // The GameManager singleton instance : the GameManager Script can be accessed anywhere with : GameManager.Instance
    public static GameManager Instance { get; private set; }
    // The state the game is currently in. It should only be updated by ChangeState
    public GameState State { get; private set; }
    // The event called each time GameManager.Instance.ChangeState is called. Any script that cares about a change of state should listen to this event
    public static event Action<GameState> StateChanged;

    public static int playerScore;

    private void Awake()
    {
        // Keep the GameManager when loading new scenes
        DontDestroyOnLoad(gameObject);

        // Singleton checks
        if (Instance == null) { // If there is no instance of GameManager yet, then this one becomes the only instance
            Instance = this;
        } else {                // If a GameManager instance already exists, destroy the new one
            Debug.LogWarning("GameManager Instance already exists, destroying the duplicate");
            Destroy(this);
            return;
        }
    }

    public void ChangeState(GameState newState)
    {
        // Change the state variable
        State = newState;
        // Run some code depending on the new state
        switch (newState) {
            case GameState.Playing:
                break;
            case GameState.Menu:
                break;
        }

        // Send the event to every listening script
        StateChanged?.Invoke(newState);
    }
}

// Different states the game can be in. Can be accessed with : GameState.exampleState
public enum GameState
{
    Playing,
    Menu
}