using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsMenu : MonoBehaviour
{
    public void PressBackButton()
    {
        GameManager.Instance.ChangeState(GameState.Menu);
    }
}
