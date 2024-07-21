using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    // Make the text object visible in the inspector
    public TextMeshProUGUI playerScoreText;

    public static GameOver Instance { get; private set; }
    public Canvas m_Canvas;

    private AudioSource gameOverMusic;

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

        gameOverMusic = GetComponentInChildren<AudioSource>();

        GameState state = new GameState();

        // If the state is not equal to game over, then hide the game over object
        if (state != GameState.GameOver)
        { 
            gameObject.SetActive(false); 
        }
    }

    private void Start()
    {
        // Play the game over music and its loop to true
        gameOverMusic.loop = true;
        gameOverMusic.Play();
    }

    private void Update()
    {
        // If a background music is found in the game, stop playing it as soon as the game over menu pops up
        if (Brobot.backgroundMusic.isPlaying == true)
        {
            Brobot.backgroundMusic.Stop();
        }

        if (gameOverMusic.isPlaying == false)
        {
            gameOverMusic.Play();
        }

        // Display the player's final score on update
        playerScoreText.text = "Final Score: " + GameManager.playerScore.ToString();
        playerScoreText.fontSize = 20;
        playerScoreText.alignment = TextAlignmentOptions.Center;
        playerScoreText.color = Color.white;

    }

    public void PressRestartButton()
    {
        if (GameCanvas.menuSelectSound != null)
        {
            GameCanvas.menuSelectSound.Play();
        }

        gameObject.SetActive(false);

        GameManager.Instance.ChangeState(GameState.Playing);

        // Reset the player score and miss count to 0
        GameManager.playerScore = 0;
        Brobot.MissCount = 0;
    }

    public void PressQuitButton()
    {
        gameObject.SetActive(false);

        GameManager.Instance.ChangeState(GameState.Menu);

        // Reset the player score and miss count to 0
        GameManager.playerScore = 0;
        Brobot.MissCount = 0;
    }
}
