using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exterminator : MonoBehaviour
{
    public int m_TutorialLinesSaid = 0;
    public static Exterminator Instance { get; private set; }
    private void Awake()
    {
        // Singleton checks
        if (Instance == null) { // If there is no instance of Exterminator yet, then this one becomes the only instance
            Instance = this;
        } else {                // If a Exterminator instance already exists, destroy the new one
            Debug.LogWarning("Exterminator Instance already exists, destroying the duplicate");
            Destroy(gameObject);
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        Brobot brobot = otherCollider.GetComponent<Brobot>();
        if (brobot == null) return;
        else if (GameManager.Instance.State == GameState.Playing && brobot.HasCrossed) Destroy(brobot.gameObject);
        else Destroy(brobot.gameObject);
        Debug.Log("destroyed");
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        Debug.Log("Entree");
        new WaitForSeconds(.5f);
        Brobot brobot = otherCollider.GetComponent<Brobot>();
        if (brobot == null || brobot == GameManager.Instance.PlayerBrobot || !GameManager.Instance.EasyMode) return;

        string toSay = "";
        if (m_TutorialLinesSaid == 0) toSay = "Dap me up with ";
        if (m_TutorialLinesSaid == 1) toSay = "And me with ";
        switch (brobot.Type) {
            case BrobotType.Blue:
                toSay += "Q";
                break;
            case BrobotType.Green:
                toSay += "W";
                break;
            case BrobotType.Red:
                toSay += "E";
                break;
            case BrobotType.Yellow:
                toSay += "R";
                break;
            default:
                toSay += "YOUR ASS";
                break;
        }
        if (m_TutorialLinesSaid == 0) toSay += "\nwhen we cross !";
        if (m_TutorialLinesSaid == 1) toSay += "!";
        if (m_TutorialLinesSaid < 2) StartCoroutine(WaitAndSay(brobot, toSay, 1.5f));
        else StartCoroutine(WaitAndSay(brobot, toSay, 0.3f));
        m_TutorialLinesSaid++;
    }

    private IEnumerator WaitAndSay(Brobot brobot, string toSay, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Factory.Instance.SpawnPopUp(brobot, toSay, 3f, 10);
        if (m_TutorialLinesSaid <= 1) {
            yield return new WaitForSeconds(1.2f);
            Factory.Instance.SpawnPopUp(brobot, "Ready ?", 1f, 6);
        }
    }
}
