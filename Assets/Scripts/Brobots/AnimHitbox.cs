using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHitbox : MonoBehaviour
{
    Collider2D m_HitBox;
    Brobot m_ParentBrobot;

    private void Awake()
    {
        m_HitBox = GetComponent<Collider2D>();
        m_ParentBrobot = GetComponentInParent<Brobot>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        BrobotHitbox otherBrobot = other.GetComponent<BrobotHitbox>();
        if (otherBrobot == m_ParentBrobot || otherBrobot == null) {
            Debug.Log("Null or Parent");
            return;
        }
        StartCoroutine(m_ParentBrobot.ActivateWannaDap(true, 0));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        BrobotHitbox otherBrobot = other.GetComponent<BrobotHitbox>();
        if (otherBrobot == m_ParentBrobot || otherBrobot == null) {
            Debug.Log("Null or Parent");
            return;
        }
        StartCoroutine(m_ParentBrobot.ActivateWannaDap(false, 0));
    }
}
