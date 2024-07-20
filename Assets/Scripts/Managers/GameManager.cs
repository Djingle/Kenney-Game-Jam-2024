using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // The GameManager singleton instance : the GameManager Script can be accessed anywhere with : GameManager.Instance
    public static GameManager Instance { get; private set; }
    // The state the game is currently in. It should only be updated by ChangeState
    public GameState State { get; private set; }
    public Brobot PlayerBrobot { get; private set; }
    public bool EasyMode { get; set; } = false;

    private Text playerScoreText;

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

        FactoryEvents.SpawnedPair += OnSpawnedPair;
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
                SceneManager.LoadScene("Game");
                break;
            case GameState.Menu:
                SceneManager.LoadScene("Menu");
                break;
            case GameState.GameOver:
                StopAllCoroutines();
                SceneManager.LoadScene("GameOver");
                break;
            case GameState.Credits:
                SceneManager.LoadScene("Credits");
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
        Brobot b = Factory.Instance.SpawnBot(new Vector3(-10, 0, 0), true, m_Speed);
        FactoryEvents.SpawnedInitialBrobot?.Invoke(b);
        ChangePlayerBrobot(b);
        StartCoroutine(SpawnBrobotPair(.1f));
        GameCanvas.Instance.gameObject.SetActive(!EasyMode);

        playerScoreText = GameObject.Find("ScoreText").GetComponent<Text>();

        /*playerScoreText.text = "Score: " + playerScore.ToString();
        playerScoreText.fontSize = 30;
        playerScoreText.alignment = TextAnchor.UpperLeft;
        playerScoreText.color = Color.blue;*/
    }

    private void ChangePlayerBrobot(Brobot b)
    {
        PlayerBrobot = b;
        PlayerBrobot.SetAsPlayerBrobot();
    }

    private IEnumerator SpawnBrobotPair(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        //Vector3 leftSpawnPos = new Vector3(PlayerBrobot.transform.position.x - m_SpawnXOffset, 0, 0);
        //Vector3 rightSpawnPos = new Vector3(PlayerBrobot.transform.position.x + m_SpawnXOffset, 0, 0);
        //Factory.Instance.SpawnBot(leftSpawnPos, true, m_Speed);
        Vector3 spawnPos = new Vector3(PlayerBrobot.transform.position.x + (PlayerBrobot.Direction ? m_SpawnXOffset : -m_SpawnXOffset), 0, 0);
        Factory.Instance.SpawnBot(spawnPos, !PlayerBrobot.Direction, m_Speed);
        FactoryEvents.SpawnedPair?.Invoke();
    }

    private void OnSpawnedPair()
    {
        if (State != GameState.Playing) return;
        float timeToWait = 100f / (m_gameSpeedUpFactor + Time.timeSinceLevelLoad);
        float randomBias = Random.Range(-1f, 1f);
        timeToWait = Mathf.Clamp(timeToWait + randomBias, 0f, 10f);
        StartCoroutine(SpawnBrobotPair(timeToWait));
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