using TMPro;
using UnityEngine;

public class TextPopUp : MonoBehaviour
{
    TextMeshPro m_TextMesh;
    Rigidbody2D m_RigidBody;
    public const float k_BaseLifeTime = .5f;
    public const float k_BaseForce = 5f;
    string m_Text;
    

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_TextMesh = GetComponent<TextMeshPro>();
    }

    public void Init(Brobot brobot, string text="", float lifeTime = k_BaseLifeTime, float force = k_BaseForce)
    {
        if (text == "") {
            int score = GameManager.Instance.Score;
            m_Text = GameManager.Instance.Score.ToString();
            m_TextMesh.color = brobot.TextColor;

            if (score == 10 || score == 25 || score == 50 || score == 100) {
                lifeTime = 1.5f;
                force = 8;
                m_TextMesh.fontSize = 15;
            }
        m_TextMesh.color = brobot.TextColor;
        } else m_Text = text;
        m_RigidBody.velocity = new Vector2(0, force);
        m_TextMesh.SetText(m_Text);

        
        Destroy(gameObject, lifeTime);
    }
}
