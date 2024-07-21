using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Brobot : MonoBehaviour
{
    Vector3 m_Displacement;
    Animator m_Animator;
    BoxCollider2D m_BoxCollider;
    Rigidbody2D m_RigidBody;
    SpriteRenderer[] m_SpriteRenderers;
    const float k_DespawnDistance = 13.5f;
    const float k_ColliderOffset = 0.0f;
    public float Speed {get; private set; }
    public bool Direction { get; private set; }
    [field: SerializeField] public BrobotType Type { get; private set; }

    public Sprite m_lilDamageSprite, m_BigDamageSprite;
    public bool HasDapped { get; private set; }
    public bool HasCrossed { get; private set; }

    int MissCount = 0;


    public static AudioSource backgroundMusic;

    private AudioSource gameOverSound;
    public static AudioSource menuMovementSound;
    public static AudioSource dapFailSound;

    private AudioSource TenScoreStreakSound;
    private AudioSource TwentyFiveStreakSound;
    private AudioSource FiftyStreakSound;
    private AudioSource HundredStreakSound;
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_BoxCollider = GetComponent<BoxCollider2D>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        

        // Find the background music immediately a brobot spawns in the game or menu
        backgroundMusic = GameObject.Find("GameMusic").GetComponent<AudioSource>();

        gameOverSound = GameObject.Find("GameOverSound").GetComponent<AudioSource>();

        menuMovementSound = GameObject.Find("MenuMovementSound").GetComponent<AudioSource>();

        GameState state = new GameState();

        if (state == GameState.Menu)
        {
            menuMovementSound.Play();
        }

        dapFailSound = GameObject.Find("MissDapSound").GetComponent<AudioSource>();

        TenScoreStreakSound = GameObject.Find("10ScoreStreak").GetComponent<AudioSource>();
        TwentyFiveStreakSound = GameObject.Find("25ScoreStreak").GetComponent<AudioSource>();
        FiftyStreakSound = GameObject.Find("50ScoreStreak").GetComponent<AudioSource>();
        HundredStreakSound = GameObject.Find("100ScoreStreak").GetComponent<AudioSource>();
    }

    public void Init(bool direction, float speed)
    {
        Direction = direction;
        Speed = speed;
        m_Displacement = new Vector3(Direction ? 1 : -1, 0, 0) * Speed;

        if (!Direction) {
            m_BoxCollider.offset *= new Vector2(-1, 0);
            foreach (SpriteRenderer s in m_SpriteRenderers) {
                s.flipX = true;
                Vector3 spritePos = s.transform.localPosition;
                spritePos.x = -spritePos.x;
                s.transform.localPosition = spritePos;
            }
        }
        SetSortingLayer(Direction ? "Brobot1" : "Brobot2");
        m_Animator.SetFloat("Offset", Random.Range(0, 1));
    }

    public void TryDap(BrobotType inputType) // The dap should succeed if we cross an other brobot (going in the opposite direction, and if the input is right)
    {
        List<Collider2D> overlappingColliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        //Debug.Log("tryna dap...");

        if (m_BoxCollider.OverlapCollider(filter, overlappingColliders) == 0) {
            MissDap();
            //Debug.Log("no bro to dap");
            return;
        }

        foreach (Collider2D box in overlappingColliders) {
            Brobot brobot = box.GetComponent<Brobot>();
            if (brobot == null || brobot.HasDapped || brobot.Direction == this.Direction) continue;

            if (inputType == brobot.Type || GameManager.Instance.EasyMode) {
                BrobotEvents.SuccessfulDap?.Invoke(brobot);
                HasDapped = true;
                GameManager.Instance.Score += 1;

                // Play the different score streak sounds once the player hit that streak mark
                switch (GameManager.Instance.Score) {
                    default: break;
                    case 10: TenScoreStreakSound.Play(); break;
                    case 25: TwentyFiveStreakSound.Play(); break;
                    case 50: FiftyStreakSound.Play(); break;
                    case 100: HundredStreakSound.Play(); break;
                }
                return;
            } else MissDap();
        }
    }

    private void SetSortingLayer(string LayerName)
    {
        foreach (SpriteRenderer s in m_SpriteRenderers) {
            s.sortingLayerName = LayerName;
        }
    }

    public void SetPlayer(bool isPlayer)
    {
        if (isPlayer) {
            SetSortingLayer("BrobotPlayer");
        }
        else {
            SetSortingLayer(Direction ? "Brobot1" : "Brobot2");
        }
    }

    public void MissDap()
    {
        if (GameManager.Instance.State == GameState.Playing)
        {
            Debug.Log("miss count : " + MissCount);
            MissCount++;
            GameManager.Instance.MissCount++;
            dapFailSound.Play();
            if (MissCount == 1) m_SpriteRenderers[0].sprite = m_lilDamageSprite;
            else if (MissCount == 2) m_SpriteRenderers[0].sprite = m_BigDamageSprite;
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
