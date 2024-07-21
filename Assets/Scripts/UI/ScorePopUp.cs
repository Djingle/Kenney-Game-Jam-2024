using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePopUp : MonoBehaviour
{
    TextMeshPro m_Text;
    Rigidbody2D m_RigidBody;
    public float m_LifeTime = 1f;
    public float m_force = 1f;
    

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_Text = GetComponent<TextMeshPro>();
    }

    public void Init()
    {
        m_RigidBody.velocity = new Vector2(0, m_force);
        m_Text.SetText(GameManager.Instance.Score.ToString());
        Destroy(gameObject, m_LifeTime);
    }
}
