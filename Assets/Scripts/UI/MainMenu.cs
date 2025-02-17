using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }
    public Toggle m_EasyToggle;
    public Canvas m_Canvas;
    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogWarning("GameManager Instance already exists, destroying the duplicate");
            Destroy(gameObject);
            return;
        }
        m_EasyToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(m_EasyToggle); });
        m_Canvas = GetComponentInChildren<Canvas>();
        m_Canvas.worldCamera = Camera.main;
        m_Canvas.planeDistance = 8;

        GameManagerEvents.StateChanged += OnStateChanged;
    }

    private void Update()
    {
        if (GameCanvas.mainMenuMusic != null && GameCanvas.mainMenuMusic.isPlaying == false)
        {
            GameCanvas.mainMenuMusic.Play();
        }
    }

    public void PressPlayButton()
    {
        if (GameCanvas.menuSelectSound != null)
        {
            GameCanvas.menuSelectSound.Play();
        }
        
        GameManager.Instance.ChangeState(GameState.Playing);
    }

    public void PressCreditsButton()
    {
        if (GameCanvas.menuSelectSound != null)
        {
            GameCanvas.menuSelectSound.Play();
        }

        GameManager.Instance.ChangeState(GameState.Credits);
    }

    void ToggleValueChanged(Toggle change)
    {
        Debug.Log("ison : " + m_EasyToggle.isOn);
        GameManager.Instance.EasyMode = m_EasyToggle.isOn;
    }

    private void OnStateChanged(GameState state)
    {
        if (state != GameState.Menu) gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        GameManager.Instance.EasyMode = m_EasyToggle.isOn;
        GameManagerEvents.StateChanged -= OnStateChanged;
    }
}
