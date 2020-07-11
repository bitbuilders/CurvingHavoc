using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class H4VC33 : MonoBehaviour
{
    [Header("Spin"), Space(10)]
    [SerializeField, Range(0.0f, 720.0f)] float m_minSpin = 5.0f;
    [SerializeField, Range(0.0f, 720.0f)] float m_maxSpin = 60.0f;

    [Header("Speed"), Space(0)]
    [SerializeField, Range(0.0f, 100.0f)] float m_minForce = 5.0f;
    [SerializeField, Range(0.0f, 100.0f)] float m_maxForce = 40.0f;

    [Header("Follow"), Space(0)]
    [SerializeField, Range(0.0f, 720.0f)] float m_minChase = 5.0f;
    [SerializeField, Range(0.0f, 720.0f)] float m_maxChase = 60.0f;

    [Header("Abilities"), Space(10)]
    [SerializeField, Range(0.0f, 20.0f)] float m_minActiveDuration = 5.0f;
    [SerializeField, Range(0.0f, 20.0f)] float m_maxActiveDuration = 10.0f;
    [SerializeField, Range(0.0f, 20.0f)] float m_inactiveDuration = 5.0f;

    [Header("Die"), Space(10)]
    [SerializeField, Range(0.0f, 10.0f)] float m_dieTime = 2.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_endDrag = 2.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_endAngularDrag = 2.0f;

    public DXT3R Owner { get; set; }
    public float Lifetime { get; set; }
    public float Power { get; private set; } = 1.0f;
    public float InversePower { get { return 1.0f - Power; } }
    public float SpinDirection { get; set; } = 1.0f;
    public int Level { get; set; }
    public int MaxLevel { get; set; } = 20;
    public bool IsWeak { get; private set; }

    public float Spin { get { return GetValueFromRange(m_minSpin, m_maxSpin); } }
    public float Force { get { return GetValueFromRange(m_minForce, m_maxForce); } }
    public float Chase { get { return GetValueFromRange(m_minChase, m_maxChase); } }


    Animator m_animator = null;
    Rigidbody2D m_rigidBody2D = null;
    PolygonCollider2D m_collider = null;
    float m_life = 0.0f;
    float m_nextDeactivation = 0.0f;
    float m_activeTime = 0.0f;
    float m_inactiveTime = 0.0f;
    float m_angle = 0.0f;

    void Awake()
    {
        Owner = FindObjectOfType<DXT3R>();
        m_animator = GetComponentInChildren<Animator>();
        m_rigidBody2D = GetComponent<Rigidbody2D>();
        m_collider = GetComponentInChildren<PolygonCollider2D>();

        m_nextDeactivation = m_maxActiveDuration;
        StartCoroutine(Lifespan());
    }

    IEnumerator Lifespan()
    {
        while (m_life < Lifetime)
        {
            m_life += Time.deltaTime;

            yield return null;
        }

        if (!IsWeak)
        {
            IsWeak = true;
            Deactivate();
        }

        float dt = 0.0f;
        float drag = m_rigidBody2D.drag;
        float angularDrag = m_rigidBody2D.angularDrag;
        while (dt < m_dieTime)
        {
            dt += Time.deltaTime;
            Power = 1.0f - (dt / m_dieTime);

            m_rigidBody2D.drag = Mathf.Lerp(drag, m_endDrag, InversePower);
            m_rigidBody2D.angularDrag = Mathf.Lerp(angularDrag, m_endAngularDrag, InversePower);

            yield return null;
        }

        Power = 0.0f;
        m_rigidBody2D.freezeRotation = false;
    }

    private void Update()
    {
        if (Power <= 0.0f) return;

        float prevTime = m_activeTime;
        m_activeTime += Time.deltaTime;
        if (m_activeTime >= m_nextDeactivation)
        {
            if (prevTime < m_nextDeactivation)
            {
                Deactivate();
                IsWeak = true;
            }

            m_inactiveTime += Time.deltaTime;
            if (m_inactiveTime >= m_inactiveDuration)
            {
                Activate();
                ResetActive();
                IsWeak = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (Power <= 0.0f) return;

        m_angle += Spin * SpinDirection * Power * Time.deltaTime;
        m_rigidBody2D.SetRotation(m_angle);

        m_rigidBody2D.velocity = m_rigidBody2D.velocity.normalized * Force * Power;
        float a = Chase * Mathf.Deg2Rad * Time.deltaTime;
        m_rigidBody2D.velocity = Vector3.RotateTowards(m_rigidBody2D.velocity, Owner.transform.position - transform.position, a, 0.0f);
    }

    public void Throw(Vector2 force, float colliderDelay)
    {
        m_rigidBody2D.AddForce(force, ForceMode2D.Impulse);
        m_collider.isTrigger = true;
    }

    IEnumerator ColliderEnable(float time)
    {
        yield return new WaitForSeconds(time);
        m_collider.isTrigger = false;
    }

    public void Pickup()
    {
        Destroy(gameObject);
    }

    void Activate()
    {
        m_animator.SetTrigger("Activate");
    }

    void Deactivate()
    {
        m_animator.SetTrigger("Deactivate");
    }

    void ResetActive()
    {
        m_nextDeactivation = Random.Range(m_minActiveDuration, m_maxActiveDuration);
        m_activeTime = 0.0f;
        m_inactiveTime = 0.0f;
    }

    float GetValueFromRange(float min, float max)
    {
        return Mathf.Lerp(min, max, (float)Level / (float)MaxLevel);
    }
}
