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
    public static int MissCount = 0;

    public static AudioSource backgroundMusic;

    private AudioSource gameOverSound;
    public static AudioSource menuMovementSound;
    public static AudioSource dapFailSound;


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_BoxCollider = GetComponent<BoxCollider2D>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        // Find the background music immediately a brobot spawns in the game or menu
        backgroundMusic = GameObject.Find("GameMusic").GetComponent<AudioSource>();

        gameOverSound = GameObject.Find("GameOverSound").GetComponent<AudioSource>();

        menuMovementSound = GameObject.Find("MenuMovementSound").GetComponent<AudioSource>();
        menuMovementSound.Play();

        dapFailSound = GameObject.Find("MissDapSound").GetComponent<AudioSource>();
    }

    public void Init(bool direction, float speed)
    {
        Direction = direction;
        Speed = speed;
        m_Displacement = new Vector3(Direction ? 1 : -1, 0, 0) * Speed;
        m_SpriteRenderer.flipX = !Direction;
        if (!Direction) m_BoxCollider.offset *= new Vector2(-1, 0);
    }

    public void TryDap(BrobotType inputType) // The dap should succeed if we cross an other brobot (going in the opposite direction, and if the input is right)
    {
        List<Collider2D> overlappingColliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        Debug.Log("tryna dap...");

        if (m_BoxCollider.OverlapCollider(filter, overlappingColliders) == 0) {
            MissDap();
            Debug.Log("no bro to dap");
            return;
        }

        foreach (Collider2D box in overlappingColliders) {
            Brobot brobot = box.GetComponent<Brobot>();
            if (brobot == null) {
                Debug.Log("no bro");
                continue;
            }
            if (brobot.HasDapped) {
            Debug.Log("bro has already dapped someone");
                continue;
            }
            if (brobot.Direction == this.Direction) {
                Debug.Log("same direction. Can't cap");
                continue; // If the collider is going in the same direction, we don't dap
            }
            
            if (inputType == brobot.Type || GameManager.Instance.EasyMode) {
                BrobotEvents.SuccessfulDap?.Invoke(brobot);
                HasDapped = true;
                GameManager.playerScore += 1;
                Debug.Log("dap");
                return;
            }
        }
    }

    public void MissDap()
    {
        if (GameManager.Instance.State == GameState.Playing)
        { 
            MissCount++;
            dapFailSound.Play();
        }
        if (MissCount == 3 && !GameManager.Instance.m_cheatMode) {
            gameOverSound.Play();
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
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
        //if (Type == BrobotType.Yellow) m_Animator.Play("Walking");

        m_RigidBody.velocity = m_Displacement;
    }
}
