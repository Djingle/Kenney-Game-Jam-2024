using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameraman : MonoBehaviour
{
    public Transform Target {  get; private set; }
    public static Cameraman Instance { get; private set; }
    
    public float m_CamDelay = .2f;
    public float m_AbsoluteXOffset = 2;
    float m_XOffset;
    float m_CamVelocity = 0f;
    float m_PixelToUnits = 64f;
    Coroutine m_TransitionCoroutine = null;
    private void Awake()
    {
        // Keep the Cameraman when loading new scenes
        DontDestroyOnLoad(gameObject);

        // Singleton checks
        if (Instance == null) { // If there is no instance of Cameraman yet, then this one becomes the only instance
            Instance = this;
        } else {                // If a Cameraman instance already exists, destroy the new one
            Debug.LogWarning("Cameraman Instance already exists, destroying the duplicate");
            Destroy(gameObject);
            return;
        }

        BrobotEvents.SuccessfulDap += (b) => ChangeTarget(b);
        FactoryEvents.SpawnedInitialBrobot += (b) => ChangeTarget(b);
    }

    private void ChangeTarget(Brobot brobot)
    {
        Target = brobot.transform;
        m_XOffset = brobot.Direction ? m_AbsoluteXOffset : - m_AbsoluteXOffset;
        if (m_TransitionCoroutine != null) StopCoroutine(m_TransitionCoroutine);
        m_TransitionCoroutine = StartCoroutine(SmoothTransition(brobot, m_CamDelay));
    }

    private IEnumerator SmoothTransition(Brobot brobot, float transitionTime)
    {
        float targetX;
        while (transitionTime > 0) {
            transitionTime -= Time.deltaTime;
            targetX = Mathf.SmoothDamp(transform.position.x, Target.position.x + m_XOffset, ref m_CamVelocity, transitionTime/2);
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
            yield return null;
        }
        m_TransitionCoroutine = null;
    }


    private void LateUpdate()
    {
        if (!GameManager.Instance.PlayerBrobot) return;
        if (m_TransitionCoroutine == null) {
            float targetX = Target.position.x + m_XOffset;
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        }
    }
    public  float RoundToNearestPixel(float unityUnits)
    {
        float valueInPixels = unityUnits * m_PixelToUnits;
        valueInPixels = Mathf.Round(valueInPixels);
        float roundedUnityUnits = valueInPixels * (1 / m_PixelToUnits);
        return roundedUnityUnits;
    }
}
