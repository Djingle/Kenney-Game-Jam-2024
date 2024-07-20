using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    // The GameManager singleton instance : the GameManager Script can be accessed anywhere with : GameManager.Instance
    public static GameManager Instance { get; private set; }
    // The state the game is currently in. It should only be updated by ChangeState
    public GameState State { get; private set; }
    public Brobot PlayerBrobot { get; private set; }
    public bool EasyMode { get; set; } = false;
    public GameObject m_MainMenuPrefab;
    public GameObject m_MainMenu;

    float m_SpawnXOffset = 20;
    float m_Speed = 5f;
    float m_gameSpeedUpFactor = 25; // The higher, the faster the game accelerates
    public bool m_cheatMode;
    public static int playerScore = 0;

    private void Awake()
    {
        // Keep the GameManager when loading new scenes
        DontDestroyOnLoad(gameObject);

        // Singleton checks
        if (Instance == null) { // If there is no instance of GameManager yet, then this one becomes the only instance
            Instance = this;
        } else {                // If a GameManager instance already exists, destroy the new one
            Debug.LogWarning("GameManager Instance already exists, destroying the duplicate");
            Destroy(gameObject);
            return;
        }

        FactoryEvents.SpawnedBot += OnSpawnedBot;
        BrobotEvents.SuccessfulDap += (b) => ChangePlayerBrobot(b);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        ChangeState(GameState.Menu);
    }

    public void ChangeState(GameState newState)
    {
        // Change the state variable
        State = newState;
        // Run some code depending on the new state
        switch (newState) {
            case GameState.Playing:
                if (PlayerBrobot == null) PlayerBrobot = Factory.Instance.SpawnBot(new Vector3(-15, 0,0), true, m_Speed);
                StopAllCoroutines();
                StartCoroutine(SpawnBot(.1f));

                break;
            case GameState.Menu:
                SceneManager.LoadScene("Game");
                if (MainMenu.Instance != null) MainMenu.Instance.gameObject.SetActive(true);
                if (CreditsMenu.Instance != null) CreditsMenu.Instance.gameObject.SetActive(false);
                break;
            case GameState.GameOver:
                StopAllCoroutines();
                if (GameOver.Instance != null) GameOver.Instance.gameObject.SetActive(true);
                break;
            case GameState.Credits:
                if (CreditsMenu.Instance != null) CreditsMenu.Instance.gameObject.SetActive(true);
                if (MainMenu.Instance != null) MainMenu.Instance.gameObject.SetActive(false);
                break;
        }

        // Send the event to every listening script
        GameManagerEvents.StateChanged?.Invoke(newState);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game") InitGame();
    }

    private void InitGame()
    {
        FactoryEvents.SpawnedBot?.Invoke();
        GameCanvas.Instance.gameObject.SetActive(!EasyMode);
    }

    private void ChangePlayerBrobot(Brobot b)
    {
        PlayerBrobot = b;
    }

    private IEnumerator SpawnBot(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        if (State == GameState.Playing) {
            Vector3 spawnPos = new Vector3(PlayerBrobot.transform.position.x + (PlayerBrobot.Direction ? m_SpawnXOffset : -m_SpawnXOffset), 0, 0);
            Factory.Instance.SpawnBot(spawnPos, !PlayerBrobot.Direction, m_Speed);
        } else if (State == GameState.Menu) {
            Brobot b = Factory.Instance.SpawnBot(new Vector3(-20,0,0), true, m_Speed);
            ChangePlayerBrobot(b);
        }
        FactoryEvents.SpawnedBot?.Invoke();
    }

    private void OnSpawnedBot()
    {
        if (State == GameState.Playing) {
            float timeToWait = 100f / (m_gameSpeedUpFactor + Time.timeSinceLevelLoad);
            float randomBias = Random.Range(-1f, 1f);
            timeToWait = Mathf.Clamp(timeToWait + randomBias, 0f, 10f);
            StartCoroutine(SpawnBot(timeToWait));
        } else if (State == GameState.Menu) {
            StartCoroutine(SpawnBot(3f));
        } else return;
    }
}

// Different states the game can be in. Can be accessed with : GameState.exampleState
public enum GameState
{
    Playing,
    Menu,
    GameOver,
    Credits
}