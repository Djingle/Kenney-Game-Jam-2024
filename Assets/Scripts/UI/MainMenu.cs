using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Toggle m_EasyToggle;
    private void Awake()
    {
        m_EasyToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(m_EasyToggle); });
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

}
