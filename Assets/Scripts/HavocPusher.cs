using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavocPusher : MonoBehaviour
{
    [SerializeField, Range(0, HavocWave.MAX_LEVEL)] int m_wave = HavocWave.MAX_LEVEL;
    [SerializeField] BoxCollider2D m_trigger = null;
    [SerializeField] Vector2 m_normal = Vector2.right;
    [SerializeField, Range(0.0f, 50.0f)] float m_dexForce = 20.0f;
    [SerializeField, Range(0.0f, 50.0f)] float m_boomForce = 10.0f;
    [SerializeField, Range(0.0f, 50.0f)] float m_linger = 0.5f;
    [SerializeField, Range(0.0f, 50.0f)] float m_cooldown = 1.0f;

    Animator m_animator = null;
    AudioSource m_pushSound = null;
    float m_lastPush = -100.0f;

    void Start()
    {
        if (HavocInc.Instance.Wave >= m_wave)
        {
            m_animator = GetComponent<Animator>();
            m_pushSound = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - m_lastPush < m_cooldown) return;

        if (collision.gameObject == HavocInc.Instance.CurrentPrototype.gameObject || (collision.transform.parent &&
            collision.transform.parent.gameObject == HavocInc.Instance.CurrentPrototype.gameObject))
        {
            PushBoom();
            OnPush();
        }
        else if (collision.gameObject == HavocInc.Instance.CurrentTester.gameObject)
        {
            PushDex();
            OnPush();
        }
    }

    void PushBoom()
    {
        Vector2 vel = HavocInc.Instance.CurrentPrototype.GetComponent<Rigidbody2D>().velocity;
        float dot = Vector3.Dot(vel, m_normal);
        Vector2 dir;

        if (dot <= 0.0f)
        {
            dir = Vector2.Reflect(vel, m_normal);

        }
        else
        {
            dir = (vel.normalized + m_normal) / 2.0f;
        }

        HavocInc.Instance.CurrentPrototype.SetDirection(dir.normalized);
        HavocInc.Instance.CurrentPrototype.AddBonusForce(m_boomForce);
    }

    void PushDex()
    {
        HavocInc.Instance.CurrentTester.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        HavocInc.Instance.CurrentTester.GetComponent<Rigidbody2D>().AddForce(m_normal.normalized * m_dexForce, ForceMode2D.Impulse);
        HavocInc.Instance.CurrentTester.EnableMovement(false);
        StartCoroutine(EnableMMovement());
    }

    void OnPush()
    {
        m_lastPush = Time.time;
        m_animator.SetTrigger("Push");
        m_trigger.enabled = false;
        StartCoroutine(Reset());
        m_pushSound.Play();
        HavocCamera.Instance.ShakeScreen(0.5f);
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(m_cooldown);
        m_trigger.enabled = true;
        m_animator.SetTrigger("Reset");
    }

    IEnumerator EnableMMovement()
    {
        yield return new WaitForSeconds(m_linger);
        HavocInc.Instance.CurrentTester.EnableMovement(true);
    }
}
