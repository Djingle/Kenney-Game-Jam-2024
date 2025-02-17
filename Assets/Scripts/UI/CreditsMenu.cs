using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenu : MonoBehaviour
{
    public static CreditsMenu Instance { get; private set; }
    public Canvas m_Canvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("GameManager Instance already exists, destroying the duplicate");
            Destroy(gameObject);
            return;
        }

        m_Canvas = GetComponentInChildren<Canvas>();
        m_Canvas.worldCamera = Camera.main;
        m_Canvas.planeDistance = 8;

        GameState state = new GameState();

        // If the state is not equal to credits menu, then hide the credits object
        if (state != GameState.Credits)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (GameCanvas.mainMenuMusic != null && GameCanvas.mainMenuMusic.isPlaying == false)
        {
            GameCanvas.mainMenuMusic.Play();
        }
    }

    public void PressBackButton()
    {
        if (GameCanvas.menuSelectSound != null)
        {
            GameCanvas.menuSelectSound.Play();
        }

        gameObject.SetActive(false);

        GameManager.Instance.ChangeState(GameState.Menu);
    }
}
