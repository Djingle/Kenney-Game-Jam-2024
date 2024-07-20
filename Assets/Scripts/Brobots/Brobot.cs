using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Brobot : MonoBehaviour
{
    Vector3 m_Displacement;
    Animator m_Animator;
    BoxCollider2D m_BoxCollider;
    Rigidbody2D m_RigidBody;
    SpriteRenderer m_SpriteRenderer;
    const float k_DespawnDistance = 13.5f;
    const float k_ColliderOffset = 0.0f;
    public float Speed {get; private set; }
    public bool Direction { get; private set; }
    [field: SerializeField] public BrobotType Type { get; private set; }

    public bool HasDapped { get; private set; }
    public bool HasCrossed { get; private set; }
    public int MissCount { get; private set; } = 0;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_BoxCollider = GetComponent<BoxCollider2D>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Init(bool direction, float speed)
    {
        Direction = direction;
        Speed = speed;
        m_Displacement = new Vector3(Direction ? 1 : -1, 0, 0) * Speed;
        m_SpriteRenderer.flipX = !Direction;
        m_BoxCollider.offset = new Vector2(Direction ? k_ColliderOffset : -k_ColliderOffset, 0);
    }

    public void TryDap(BrobotType inputType) // The dap should succeed if we cross an other brobot (going in the opposite direction, and if the input is right)
    {
        List<Collider2D> overlappingColliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();

        if (m_BoxCollider.OverlapCollider(filter, overlappingColliders) == 0) {
            MissDap();
            return;
        }

        foreach (Collider2D box in overlappingColliders) {
            Brobot brobot = box.GetComponent<Brobot>();
            if (brobot == null || brobot.HasDapped) continue;
            if (brobot.Direction == this.Direction)
                continue; // If the collider is going in the same direction, we don't dap
            
            if (inputType == brobot.Type || GameManager.Instance.EasyMode) {
                BrobotEvents.SuccessfulDap?.Invoke(brobot);
                HasDapped = true;
                return;
            }
        }
    }

    public void MissDap()
    {
        MissCount++;
        if (MissCount == 3 && !GameManager.Instance.m_cheatMode) {
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
    }

    public void SetAsPlayerBrobot()
    {
        //Debug.Log(gameObject.name + ": I'm the player !");
    }

    private void OnTriggerExit2D(Collider2D otherCollider) // When I leave an other Brobot
    {
        Brobot otherBrobot = otherCollider.GetComponent<Brobot>();
        if (otherBrobot == null) return;
        if (otherBrobot.Direction == this.Direction) return; // I just passed someone, whatever
        if (GameManager.Instance.PlayerBrobot == otherBrobot) HasCrossed = true; // Hey that was the player ! I crossed him !
        if (HasCrossed && !HasDapped) { // But if we didn't dap
            BrobotEvents.FailedDap?.Invoke(otherBrobot); // Imma tell everyone
            otherBrobot.MissDap(); // And call MissDap on him
        }
    }

    private void FixedUpdate()
    {
        //m_Animator.Play("rrWalk");*

        m_RigidBody.velocity = m_Displacement;
    }
}
