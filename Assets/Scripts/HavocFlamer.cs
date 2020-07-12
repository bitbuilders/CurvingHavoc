using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class HavocFlamer : MonoBehaviour
{
    [SerializeField, Range(0, HavocWave.MAX_LEVEL)] int m_wave = HavocWave.MAX_LEVEL;
    [SerializeField, Range(0.0f, 20.0f)] float m_minCooldown = 5.0f;
    [SerializeField, Range(0.0f, 20.0f)] float m_maxCooldown = 10.0f;
    [SerializeField, Range(0.0f, 20.0f)] float m_duration = 3.5f;
    [SerializeField, Range(0.0f, 20.0f)] float m_killTime = 0.25f;
    [SerializeField, Range(0.0f, 20.0f)] float m_lightDelay = 0.1f;
    [SerializeField] ParticleSystem m_flameParticles = null;
    [SerializeField] Light2D m_light = null;

    public bool Flaming { get; private set; }

    float m_nextCD = 0.0f;
    float m_currentTime = 0.0f;
    float m_lastTime = 0.0f;
    float m_burnTime = 0.0f;

    void Start()
    {
        if (HavocInc.Instance.Wave >= m_wave)
        {
            m_nextCD = Random.Range(m_minCooldown, m_maxCooldown);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        m_currentTime += Time.deltaTime;
        if (m_currentTime >= m_nextCD)
        {
            if (m_lastTime < m_nextCD)
            {
                Flaming = true;
                StartCoroutine(DelayLight(true));
                m_flameParticles.Play(true);
                // FLAME ON
            }

            if (m_currentTime >= m_nextCD + m_duration)
            {
                m_currentTime = 0.0f;
                m_nextCD = Random.Range(m_minCooldown, m_maxCooldown);

                Flaming = false;
                StartCoroutine(DelayLight(false));
                m_flameParticles.Stop(true);
                // FLAME OFF
            }
        }

        m_lastTime = m_currentTime;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!Flaming || !HavocInc.Instance.CurrentTester.IsAlive) return;

        if (collision.gameObject == HavocInc.Instance.CurrentTester.gameObject)
        {
            m_burnTime += Time.deltaTime;
            
            if (m_burnTime >= m_killTime)
            {
                HavocInc.Instance.CurrentTester.Lives--;
                HavocInc.Instance.CurrentTester.DeathColor = Color.red;
                
                if (!HavocInc.Instance.CurrentTester.IsAlive)
                {
                    HavocInc.Instance.CurrentTester.Kill((HavocInc.Instance.CurrentTester.transform.position - transform.position).normalized);
                }
            }
        }
        else
        {
            m_burnTime -= Time.deltaTime;
            m_burnTime = Mathf.Clamp01(m_burnTime);
        }
    }

    IEnumerator DelayLight(bool enable)
    {
        yield return new WaitForSeconds(m_lightDelay);
        m_light.enabled = enable;
    }
}
