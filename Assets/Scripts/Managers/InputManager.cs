using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance {  get; private set; }

    KeyCode m_PressedKey;

    private void Awake()
    {
        // Keep the InputManager when loading new scenes
        DontDestroyOnLoad(gameObject);

        // Singleton checks
        if (Instance == null) { // If there is no instance of InputManager yet, then this one becomes the only instance
            Instance = this;
        } else {                // If a InputManager instance already exists, destroy the new one
            Debug.LogWarning("InputManager Instance already exists, destroying the duplicate");
            Destroy(this);
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.A)) {
            GameManager.Instance.ActiveBrobot.TryDap(BrobotType.Blue);
        } else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyDown(KeyCode.Z)) {
            GameManager.Instance.ActiveBrobot.TryDap(BrobotType.Green);
        } else if (Input.GetKeyDown(KeyCode.E)) {
            GameManager.Instance.ActiveBrobot.TryDap(BrobotType.Red);
        } else if (Input.GetKeyDown(KeyCode.R)) {
            GameManager.Instance.ActiveBrobot.TryDap(BrobotType.Yellow);
        }
    }
}
