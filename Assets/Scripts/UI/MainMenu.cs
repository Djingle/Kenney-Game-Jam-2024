using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance { get; private set; }
    public Toggle m_EasyToggle;
    public Canvas m_Canvas;
    private AudioSource menuMusic;
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

        menuMusic = GetComponentInChildren<AudioSource>();
    }

    private void Start()
    {
        menuMusic.loop = true;
        menuMusic.Play();
    }
    public void PressPlayButton()
    {
        GameManager.Instance.ChangeState(GameState.Playing);
    }

    public void PressCreditsButton()
    {
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
