using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // The GameManager singleton instance : the GameManager Script can be accessed anywhere with : GameManager.Instance
    public static GameManager Instance { get; private set; }
    // The state the game is currently in. It should only be updated by ChangeState
    public GameState State { get; private set; }
    public Brobot ActiveBrobot { get; private set; }

    float m_SpawnXOffset = 30;
    float m_Speed = 5f;
    float m_gameSpeedUpFactor = 15; // The higher, the faster the game accelerates
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

        FactoryEvents.SpawnedPair += OnSpawnedPair;
        BrobotEvents.SuccessfulDap += (b) => ChangeActiveBrobot(b);
    }

    private void Start()
    {
        Brobot b = Factory.Instance.SpawnBot(new Vector3(-10, 0, 0), true, m_Speed);
        FactoryEvents.SpawnedInitialBrobot?.Invoke(b);
        ChangeActiveBrobot(b);
        StartCoroutine(SpawnBrobotPair(.1f));
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
        GameManagerEvents.StateChanged?.Invoke(newState);
    }

    private void ChangeActiveBrobot(Brobot b)
    {
        ActiveBrobot = b;
    }

    private IEnumerator SpawnBrobotPair(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Vector3 leftSpawnPos = new Vector3(-m_SpawnXOffset, 0, 0);
        Vector3 rightSpawnPos = new Vector3(m_SpawnXOffset, 0, 0);
        Factory.Instance.SpawnBot(leftSpawnPos, true, m_Speed);
        Factory.Instance.SpawnBot(rightSpawnPos, false, m_Speed);
        FactoryEvents.SpawnedPair?.Invoke();
    }

    private void OnSpawnedPair()
    {
        float timeToWait = 50f / (m_gameSpeedUpFactor + Time.timeSinceLevelLoad);
        float randomBias = Random.Range(-1f, 1f);
        timeToWait = Mathf.Clamp(timeToWait + randomBias, 0f, 10f);
        StartCoroutine(SpawnBrobotPair(timeToWait));
    }
}

// Different states the game can be in. Can be accessed with : GameState.exampleState
public enum GameState
{
    Playing,
    Menu
}