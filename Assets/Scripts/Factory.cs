using System.Collections;
using UnityEngine;

public class Factory : MonoBehaviour
{
    public static Factory Instance {  get; private set; }

    // Prefab references
    public GameObject m_BlueBotPrefab;
    public GameObject m_GreenBotPrefab;
    public GameObject m_RedBotPrefab;
    public GameObject m_YellowBotPrefab;

    private void Awake()
    {
        // Keep the Factory when loading new scenes
        DontDestroyOnLoad(gameObject);

        // Singleton checks
        if (Instance == null) { // If there is no instance of Factory yet, then this one becomes the only instance
            Instance = this;
        } else {                // If a Factory instance already exists, destroy the new one
            Debug.LogWarning("Factory Instance already exists, destroying the duplicate");
            Destroy(this);
            return;
        }
    }

    public Brobot SpawnBot(Vector3 position, bool direction, float speed) // Should return a Brobot
    {
        int spawnType = Random.Range(0, 4);
        GameObject botPrefab;
        switch (spawnType) {
            case 0:
                botPrefab = m_BlueBotPrefab; break;
            case 1:
                botPrefab = m_GreenBotPrefab; break;
            case 2:
                botPrefab = m_RedBotPrefab; break;
            case 3:
                botPrefab = m_YellowBotPrefab; break;
            default:
                botPrefab = m_BlueBotPrefab; break;
        }
        GameObject instance = Instantiate(botPrefab, position, Quaternion.identity);
        Brobot brobot = instance.GetComponent<Brobot>();
        brobot.Init(direction, speed);
        return brobot;
    }
}
