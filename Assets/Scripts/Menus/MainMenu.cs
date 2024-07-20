using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PressPlayButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void PressCreditsButton()
    {
        SceneManager.LoadScene("Credits");
    }
}
