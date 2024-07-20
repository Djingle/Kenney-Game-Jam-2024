using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exterminator : MonoBehaviour
{
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
        if (brobot == null || !brobot.HasCrossed) return;
        Destroy(brobot);
    }
}
