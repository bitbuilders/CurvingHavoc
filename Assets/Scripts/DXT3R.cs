using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DXT3R : MonoBehaviour
{
    [SerializeField, Range(0, 10)] int m_lives = 1;
    [SerializeField, Range(0.0f, 20.0f)] float m_deathKnockback = 5.0f;
    [SerializeField] AudioClip m_dieSound = null;

    public H4VC33 Prototype { get; set; }
    public Color DeathColor { get; set; } = Color.white;
    public int Lives { get; set; }
    public bool IsAlive { get { return Lives > 0; } }

    HavocController m_havocController = null;
    HavocMovement m_havocMovement = null;
    AudioSource m_audioSource = null;

    private void Awake()
    {
        m_havocController = GetComponent<HavocController>();
        m_havocMovement = GetComponent<HavocMovement>();
        m_audioSource = GetComponent<AudioSource>();
        Lives = m_lives;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("DXT3R"), LayerMask.NameToLayer("H4VC33"), false);
    }

    public void EnableMovement(bool enable)
    {
        m_havocController.IsTesting = enable;
        m_havocMovement.IsTesting = enable;
    }

    public void StartTesting()
    {
        m_havocController.IsTesting = true;
        m_havocMovement.IsTesting = true;
    }

    public void Kill(Vector2 dir)
    {
        m_havocMovement.IsTesting = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().AddForce(dir.normalized * m_deathKnockback, ForceMode2D.Impulse);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("DXT3R"), LayerMask.NameToLayer("H4VC33"), true);
        StartCoroutine(FadeOutOfExistence());

        m_audioSource.clip = m_dieSound;
        m_audioSource.Play();
    }

    IEnumerator FadeOutOfExistence()
    {
        Color c = DeathColor;
        float t = 0.65f;
        for (float i = 0.0f; i < t; i += Time.deltaTime)
        {
            float a = 1.0f - i / t;

            c.a = a;
            GetComponent<SpriteRenderer>().color = c;
            yield return null;
        }

        c.a = 0.0f;
        GetComponent<SpriteRenderer>().color = c;
    }
}
