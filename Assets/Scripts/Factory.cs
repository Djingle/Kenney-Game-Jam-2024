using UnityEngine;

public class Factory : MonoBehaviour
{
    public static Factory Instance {  get; private set; }

    // Prefab references
    public GameObject m_GreyBotPrefab;
    public GameObject m_GreenBotPrefab;
    public GameObject m_YellowBotPrefab;
    public GameObject m_RedBotPrefab;

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

    

    public void SpawnPairOfBots(float centerX, float xOffset, float speed)
    {
        float yOffset = -.26f;
        SpawnBot(new Vector3(centerX - xOffset, yOffset, 0), true, speed);
        SpawnBot(new Vector3(centerX + xOffset, yOffset, 0), false, speed);
    }

    public void SpawnBot(Vector3 position, bool direction, float speed) // Should return a Brobot
    {
        int spawnType = Random.Range(0, 4);
        GameObject botPrefab;
        switch (spawnType) {
            case 0:
                botPrefab = m_GreyBotPrefab; break;
            case 1:
                botPrefab = m_GreyBotPrefab; break;
            case 2:
                botPrefab = m_YellowBotPrefab; break;
            case 3:
                botPrefab = m_RedBotPrefab; break;
            default:
                botPrefab = m_GreyBotPrefab; break;
        }
        GameObject instance = Instantiate(botPrefab, position, Quaternion.identity);
        // Brobot brobot = instance.GetComponent<Brobot>();
        // brobot.Init(direction, speed);
        // return brobot;
    }
}
