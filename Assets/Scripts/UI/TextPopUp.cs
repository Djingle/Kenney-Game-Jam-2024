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
        if (text == "") m_Text = GameManager.Instance.Score.ToString();
        else m_Text = text;

        m_RigidBody.velocity = new Vector2(0, force);
        m_TextMesh.SetText(m_Text);
        m_TextMesh.color = brobot.TextColor;
        Destroy(gameObject, lifeTime);
    }
}
