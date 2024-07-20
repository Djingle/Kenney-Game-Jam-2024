using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance {  get; private set; }

    KeyCode m_PressedKey;

    private void Awake()
    {
        // Singleton checks
        if (Instance == null) { // If there is no instance of InputManager yet, then this one becomes the only instance
            Instance = this;
        } else {                // If a InputManager instance already exists, destroy the new one
            Debug.LogWarning("InputManager Instance already exists, destroying the duplicate");
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.A)) {
            GameManager.Instance.PlayerBrobot.TryDap(BrobotType.Blue);
        } else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyDown(KeyCode.Z)) {
            GameManager.Instance.PlayerBrobot.TryDap(BrobotType.Green);
        } else if (Input.GetKeyDown(KeyCode.E)) {
            GameManager.Instance.PlayerBrobot.TryDap(BrobotType.Red);
        } else if (Input.GetKeyDown(KeyCode.R)) {
            GameManager.Instance.PlayerBrobot.TryDap(BrobotType.Yellow);
        }
    }
}
