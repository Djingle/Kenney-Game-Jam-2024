using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class BrobotHitbox : MonoBehaviour
{
    Collider2D m_HitBox;
    Brobot m_ParentBrobot;

    private void Awake()
    {
        m_HitBox = GetComponent<Collider2D>();
        m_ParentBrobot = GetComponentInParent<Brobot>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        BrobotHitbox otherHitbox = other.GetComponent<BrobotHitbox>();
        if (otherHitbox == null) return;
        else {
            Brobot otherBrobot = otherHitbox.GetComponentInParent<Brobot>();
            m_ParentBrobot.LeaveOtherBrobot(otherBrobot);
        }
    }
}
