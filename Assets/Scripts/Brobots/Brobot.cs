using System.Collections.Generic;
using UnityEngine;

public class Brobot : MonoBehaviour
{
    Vector3 m_Displacement;
    Animator m_Animator;
    BoxCollider2D m_BoxCollider;
    Rigidbody2D m_RigidBody;
    public float Speed {get; private set; }
    public bool Direction { get; private set; }
    [field: SerializeField] public BrobotType Type { get; private set; }
    public bool m_easyMode;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_BoxCollider = GetComponent<BoxCollider2D>();
        m_RigidBody = GetComponent<Rigidbody2D>();
    }
    public void Init(bool direction, float speed)
    {
        Direction = direction;
        Speed = speed;
        m_Displacement = new Vector3(Direction ? 1 : -1, 0, 0) * Speed;
    }

    public void TryDap(BrobotType inputType) // The dap should succeed if we cross an other brobot (going in the opposite direction, and if the input is right)
    {
        Debug.Log("tryna dap...");
        List<Collider2D> overlappingColliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();

        if (m_BoxCollider.OverlapCollider(filter, overlappingColliders) == 0)
            return; // If no collider overlaps this one, we don't dap

        foreach (Collider2D box in overlappingColliders) {
            Brobot brobot = box.GetComponent<Brobot>();
            if (brobot.Direction == this.Direction)
                continue; // If the collider is going in the same direction, we don't dap
            
            if (inputType == brobot.Type || m_easyMode) {
                Debug.Log("DAP!!!");
                BrobotEvents.SuccessfulDap?.Invoke(brobot);
                return;
            }
        }
    }

    private void FixedUpdate()
    {
        //m_Animator.Play("rrWalk");
        m_RigidBody.velocity = m_Displacement;
    }
}
