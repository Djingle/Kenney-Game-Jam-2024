using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Brobot : MonoBehaviour
{
    public float raydistance;

    Vector3 m_Displacement;
    Animator m_Animator;
    BoxCollider2D m_BrobotHitbox;
    Rigidbody2D m_RigidBody;
    SpriteRenderer[] m_SpriteRenderers;
    public AudioSource AudioSource {  get; private set; }
    public bool Direction { get; private set; }
    [field: SerializeField] public BrobotType Type { get; private set; }
    [field: SerializeField] public Color TextColor { get; private set; }

    public int MissCount { get; private set; } = 0;

    public Sprite m_lilDamageSprite, m_BigDamageSprite;
    public bool HasDapped { get; private set; }
    public bool HasCrossed { get; private set; }

    public bool HasMissed { get; private set; } = false;
    //public const float k_ColliderXOffset = .9f;


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
        m_BrobotHitbox = GetComponentInChildren<BrobotHitbox>().GetComponent<BoxCollider2D>();
        m_RigidBody = GetComponent<Rigidbody2D>();
        m_SpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        AudioSource = GetComponent<AudioSource>();


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
        m_Displacement = new Vector3(Direction ? 1 : -1, 0, 0);

        if (!Direction) {
            Vector3 inverseScale = transform.localScale;
            inverseScale.x *= -1;
            transform.localScale = inverseScale;
        }
        SetSortingLayer(Direction ? "Brobot1" : "Brobot2");
        m_Animator.SetFloat("Offset", Random.Range(0, 1));
        m_Animator.SetBool("WannaDap", false);
    }

    public void TryDap(BrobotType inputType) // The dap should succeed if we cross an other brobot (going in the opposite direction, and if the input is right)
    {
        HasMissed = false;
        List<Collider2D> overlappingColliders = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Brobot"));
        filter.useTriggers = true;
        Debug.Log("tryna dap...");

        if (m_BrobotHitbox.OverlapCollider(filter, overlappingColliders) == 0) {
            Debug.Log("no bro");
            return;
        }

        foreach (Collider2D box in overlappingColliders) {
            Brobot bro = box.GetComponentInParent<Brobot>();
            if (bro == null) { Debug.Log("bro component not found"); continue; }
            if (bro.HasDapped) { Debug.Log("bro has already dapped"); continue; }
            if (bro.Direction == this.Direction) { Debug.Log("bro going in same direction"); continue; }

            if (inputType == bro.Type) { // DAP
                HasDapped = true;
                bro.AudioSource.Play();
                BrobotEvents.SuccessfulDap?.Invoke(bro);

                // Play the different score streak sounds once the player hit that streak mark
                switch (GameManager.Instance.Score) {
                    default: break;
                    case 10: TenScoreStreakSound.Play(); break;
                    case 25: TwentyFiveStreakSound.Play(); break;
                    case 50: FiftyStreakSound.Play(); break;
                    case 100: HundredStreakSound.Play(); break;
                }
            } else {
                MissDap();
                HasMissed = true;
            }
            return;
        }
    }

    public IEnumerator ActivateWannaDap(bool activate, float time)
    {
        yield return new WaitForSeconds(time);
        m_Animator.SetBool("WannaDap", activate);
    }

    private void SetSortingLayer(string LayerName)
    {
        foreach (SpriteRenderer s in m_SpriteRenderers) {
            s.sortingLayerName = LayerName;
        }
    }

    public void SetPlayer(bool isPlayer)
    {
        m_Animator.SetTrigger("DapTrigger");
        m_Animator.SetBool("WannaDap", false);
        if (isPlayer) {
            SetSortingLayer("BrobotPlayer");
            m_BrobotHitbox.isTrigger = false;
        }
        else {
            SetSortingLayer(Direction ? "Brobot1" : "Brobot2");
            m_BrobotHitbox.isTrigger = true;
        }
    }

    public void MissDap()
    {
        m_Animator.SetTrigger("NoDapTrigger");
        if (GameManager.Instance.State == GameState.Playing)
        {
            MissCount++;
            dapFailSound.Play();
            if (MissCount == 1) m_SpriteRenderers[0].sprite = m_lilDamageSprite;
            else if (MissCount == 2) m_SpriteRenderers[0].sprite = m_BigDamageSprite;
        }
        if (MissCount >= 3 && !GameManager.Instance.m_cheatMode) {
            gameOverSound.Play();
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
    }

    public void LeaveOtherBrobot(Brobot otherBrobot) // When I leave an other Brobot
    {
        if (otherBrobot.Direction == this.Direction) return; // I just passed someone, whatever
        if (GameManager.Instance.PlayerBrobot == otherBrobot) HasCrossed = true; // Hey that was the player ! I crossed him !
        if (HasCrossed && !HasDapped && !HasMissed && !otherBrobot.HasMissed) { // But if we didn't dap
            BrobotEvents.FailedDap?.Invoke(otherBrobot); // Imma tell everyone
            otherBrobot.MissDap(); // And call MissDap on him
        }
    }

    private void FixedUpdate()
    {

        m_RigidBody.velocity = m_Displacement * GameManager.Instance.Speed;

        //float offset = Direction ? 1 : -1;
        //Vector3 raypos = transform.position;
        //raypos.y += offset * 2;
        //RaycastHit2D inFront = Physics2D.Raycast(raypos, new Vector3(offset, 0, 0), 10);
        //if (inFront.collider == null) {
        //    m_Animator.SetBool("WannaDap", false);
        //    return;
        //}
        //if (inFront.collider.GetComponent<Brobot>() != null) {
        //    Debug.Log("ouiiii");
        //    m_Animator.SetBool("WannaDap", true);
        //} else { m_Animator.SetBool("WannaDap", false); }
    }
}
