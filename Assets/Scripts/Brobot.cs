using UnityEngine;

public class Brobot : MonoBehaviour
{
    Vector3 m_Displacement;
    Animator m_Animator;
    public float Speed {get; private set; }
    public bool Direction { get; private set; }
    [field: SerializeField] public BrobotType Type {  get; private set; }

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }
    public void Init(bool direction, float speed)
    {
        Direction = direction;
        Speed = speed;
        m_Displacement = new Vector3(Direction ? 1 : -1, 0, 0) * Speed;
    }

    private void Update()
    {
        //m_Animator.Play("rrWalk");
        transform.position += m_Displacement;
    }
}
