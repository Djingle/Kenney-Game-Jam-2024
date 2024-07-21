using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Factory : MonoBehaviour
{
    public static Factory Instance {  get; private set; }

    // Prefab references
    public GameObject m_BlueBotPrefab;
    public GameObject m_GreenBotPrefab;
    public GameObject m_RedBotPrefab;
    public GameObject m_YellowBotPrefab;

    public GameObject m_ScorePopUpPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // Singleton checks
        if (Instance == null) { // If there is no instance of Factory yet, then this one becomes the only instance
            Instance = this;
        } else {                // If a Factory instance already exists, destroy the new one
            Debug.LogWarning("Factory Instance already exists, destroying the duplicate");
            Destroy(gameObject);
            return;
        }
        BrobotEvents.SuccessfulDap += (b) => SpawnPopUp(b);
    }

    public Brobot SpawnBot(Vector3 position, bool direction, float speed, BrobotType type = BrobotType.Blue)
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
        RaycastHit2D hit;
        hit = Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.zero);
        while (hit.collider && hit.transform.gameObject.GetComponent<Brobot>()) {
            position.x = position.x + (direction ? -3 : 3);
            hit = Physics2D.Raycast(new Vector2(position.x, position.y), Vector2.zero);
        }
        GameObject instance = Instantiate(botPrefab, position, Quaternion.identity);
        Brobot brobot = instance.GetComponent<Brobot>();
        brobot.Init(direction, speed);
        return brobot;
    }

    public TextPopUp SpawnPopUp(Brobot brobot, string text = "", float lifeTime = TextPopUp.k_BaseLifeTime, float force = TextPopUp.k_BaseForce)
    {
        Vector3 spawnPos = brobot.transform.position;
        spawnPos.y += 2;
        GameObject popUp = Instantiate(m_ScorePopUpPrefab, spawnPos, Quaternion.identity);
        TextPopUp scorePopUp = popUp.GetComponent<TextPopUp>();
        scorePopUp.Init(brobot, text, lifeTime, force);
        return scorePopUp;
    }
}
