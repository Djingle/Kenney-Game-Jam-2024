using UnityEngine;
using System.Collections;
public class GameManager : MonoBehaviour
{
    // The GameManager singleton instance : the GameManager Script can be accessed anywhere with : GameManager.Instance
    public static GameManager Instance { get; private set; }
    // The state the game is currently in. It should only be updated by ChangeState
    public GameState State { get; private set; }
    public Brobot PlayerBrobot { get; private set; }
    public bool EasyMode { get; set; } = false;
    public int Score { get; set; } = 0;
    public float Speed { get; set; } = 5;
    public GameObject m_MainMenuPrefab;
    public GameObject m_MainMenu;

    float m_SpawnXOffset = 20;
    float m_StartSpawnSpeedFactor = 25; // The higher, the faster the game accelerates
    float m_GameSpeedUpFactor = 100;
    float m_GameStartTime;
    bool m_SpawnCoroutineRunning;
    public bool m_cheatMode;

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
        BrobotEvents.SuccessfulDap += OnSuccessfulDap;
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
                if (PlayerBrobot == null) PlayerBrobot = Factory.Instance.SpawnBot(new Vector3(-15, 0, 0), true, Speed);
                if (GameCanvas.Instance != null) GameCanvas.Instance.gameObject.SetActive(true);
                if (MainMenu.Instance != null) MainMenu.Instance.gameObject.SetActive(false);

                // Play the background music and set the loop to true inside the playing state
                Brobot.backgroundMusic.loop = true;
                Brobot.backgroundMusic.Play();

                GameCanvas.mainMenuMusic.Stop();

                if (EasyMode) {
                    Speed = 2f;
                    m_GameSpeedUpFactor = 200;
                    m_StartSpawnSpeedFactor = 25;
                }
                m_GameStartTime = Time.time;
                if (!m_SpawnCoroutineRunning) {
                    StartCoroutine(SpawnBot(2));
                    m_SpawnCoroutineRunning = true;
                }

                break;
            case GameState.Menu:
                //StartCoroutine(Cameraman.Instance.SmoothMove(0, 1f));
                if (MainMenu.Instance != null) MainMenu.Instance.gameObject.SetActive(true);
                if (CreditsMenu.Instance != null) CreditsMenu.Instance.gameObject.SetActive(false);
                if (GameCanvas.Instance != null) GameCanvas.Instance.gameObject.SetActive(false);

                if (!m_SpawnCoroutineRunning) {
                    StartCoroutine(SpawnBot(2));
                    m_SpawnCoroutineRunning = true;
                }

                break;

            case GameState.GameOver:
                if (GameOver.Instance != null) GameOver.Instance.gameObject.SetActive(true);
                if (GameCanvas.Instance != null) GameCanvas.Instance.gameObject.SetActive(false);

                GameCanvas.mainMenuMusic.Stop();
                StopAllCoroutines();
                m_SpawnCoroutineRunning = false;
                Exterminator.Instance.m_TutorialLinesSaid = 0;
                break;

            case GameState.Credits:
                if (CreditsMenu.Instance != null) CreditsMenu.Instance.gameObject.SetActive(true);
                if (MainMenu.Instance != null) MainMenu.Instance.gameObject.SetActive(false);
                if (GameCanvas.Instance != null) GameCanvas.Instance.gameObject.SetActive(false);
                break;
        }

        // Send the event to every listening script
        GameManagerEvents.StateChanged?.Invoke(newState);
    }

    private void OnSuccessfulDap(Brobot brobot)
    {
        Score++;
        ChangePlayerBrobot(brobot);
        if (EasyMode && Speed < 5) Speed += 1;
    }

    private void ChangePlayerBrobot(Brobot b)
    {
        if (PlayerBrobot != null) PlayerBrobot.SetPlayer(false);
        PlayerBrobot = b;
        PlayerBrobot.SetPlayer(true);
    }

    private IEnumerator SpawnBot(float timeToWait)
    {
        m_SpawnCoroutineRunning = true;
        yield return new WaitForSeconds(timeToWait);
        if (State == GameState.Playing) {
            Vector3 spawnPos = new Vector3(PlayerBrobot.transform.position.x + (PlayerBrobot.Direction ? m_SpawnXOffset : -m_SpawnXOffset), 0, 0);
            Factory.Instance.SpawnBot(spawnPos, !PlayerBrobot.Direction, Speed);
        } else if (State == GameState.Menu || State == GameState.Credits) {
            int rand = Random.Range(0, 2);
            Brobot b;
            if (rand == 0) b = Factory.Instance.SpawnBot(new Vector3(-20, 0, 0), true, Speed);
            else b = Factory.Instance.SpawnBot(new Vector3(20, 0, 0), false, Speed);
            ChangePlayerBrobot(b);
        }
        FactoryEvents.SpawnedBot?.Invoke();
    }

    private void OnSpawnedBot()
    {
        float timeToWait=0;
        float randomBias = Random.Range(-1f, 1f);
        if (State == GameState.Playing) {
            timeToWait = m_GameSpeedUpFactor / (m_StartSpawnSpeedFactor + (Time.time - m_GameStartTime));
            timeToWait = Mathf.Clamp(timeToWait + randomBias, 0f, 10f);
        } else if (State == GameState.Menu || State == GameState.Credits) {
            timeToWait = 5f + randomBias;
        }
        m_SpawnCoroutineRunning = false;
        StartCoroutine(SpawnBot(timeToWait + randomBias));
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