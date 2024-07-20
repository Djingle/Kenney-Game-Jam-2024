using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    // Make the text object inaccessible to other classes
    private Text playerScoreText;

    // Start is called before the first frame update
    void Start()
    {
        // Start by finding our player score text object first so that Unity knows what text I want to code
        playerScoreText = GameObject.Find("PlayerScoreText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        // Display the player's final score on update
        playerScoreText.text = "Final Score: " + GameManager.playerScore.ToString();
        playerScoreText.fontSize = 20;
        playerScoreText.alignment = TextAnchor.MiddleCenter;
        playerScoreText.color = Color.white;
    }

    public void PressRestartButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void PressQuitButton()
    {
        SceneManager.LoadScene("Menu");
    }
}
