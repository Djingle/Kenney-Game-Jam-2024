using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    Canvas m_Canvas;
    public static GameCanvas Instance {  get; private set; }
    private void Awake()
    {
        // Singleton checks
        if (Instance == null) { // If there is no instance of GameCanvas yet, then this one becomes the only instance
            Instance = this;
        } else {                // If a GameCanvas instance already exists, destroy the new one
            Debug.LogWarning("GameCanvas Instance already exists, destroying the duplicate");
            Destroy(gameObject);
            return;
        }
        m_Canvas = GetComponent<Canvas>();
        m_Canvas.worldCamera = Camera.main;
        m_Canvas.planeDistance = 9;
    }
}
