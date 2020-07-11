using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H4VC33 : MonoBehaviour
{
    [Header("Constants")]
    [SerializeField, Range(0.0f, 100.0f)] float m_constantSpin = 5.0f;
    [SerializeField, Range(0.0f, 100.0f)] float m_constantForce = 5.0f;

    [Header("Initial"), Space(10)]
    [SerializeField, Range(0.0f, 100.0f)] float m_initialSpinBurst = 15.0f;

    [Header("Restraints"), Space(10)]
    [SerializeField, Range(0.0f, 50.0f)] float m_maxSpeed = 10.0f;
    [SerializeField, Range(0.0f, 1000.0f)] float m_maxTorque = 500.0f;

    [Header("Die"), Space(10)]
    [SerializeField, Range(0.0f, 10.0f)] float m_dieTime = 2.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_endDrag = 2.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_endAngularDrag = 2.0f;

    [Header("Bounce"), Space(10)]
    [SerializeField, Range(0.0f, 100.0f)] float m_bounceSpinBurst = 15.0f;
    [SerializeField, Range(0.0f, 100.0f)] float m_maxBounceForce = 40.0f;
    [SerializeField, Range(0.0f, 100.0f)] float m_minBounceForce = 5.0f;

    public DXT3R Owner { get; set; }
    public float BounceForce { get { return Mathf.Lerp(m_minBounceForce, m_maxBounceForce, Level / MaxLevel); } }
    public float Lifetime { get; set; }
    public float Power { get; private set; } = 1.0f;
    public float InversePower { get { return 1.0f - Power; } }
    public float SpinDirection { get; set; } = 1.0f;
    public int Level { get; set; }
    public int MaxLevel { get; set; } = 20;

    PolygonCollider2D m_collider = null;
    Rigidbody2D m_rigidBody2D = null;
    float m_life = 0.0f;

    void Awake()
    {
        Owner = FindObjectOfType<DXT3R>();
        m_collider = GetComponent<PolygonCollider2D>();
        m_rigidBody2D = GetComponent<Rigidbody2D>();
        Lifetime = 60;

        StartCoroutine(Lifespan());
    }

    IEnumerator Lifespan()
    {
        while (m_life < Lifetime)
        {
            m_life += Time.deltaTime;

            yield return null;
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
    }

    private void FixedUpdate()
    {
        m_rigidBody2D.AddTorque(m_constantSpin * SpinDirection * Power * Time.deltaTime);

        Vector2 dir = Owner.gameObject.transform.position - transform.position;
        Vector2 force = dir.normalized * m_constantForce * Power * Time.deltaTime;
        m_rigidBody2D.AddForce(force);

        if (m_rigidBody2D.angularVelocity > m_maxTorque)
        {
            m_rigidBody2D.angularVelocity = m_maxTorque;
        }

        if (m_rigidBody2D.velocity.sqrMagnitude > m_maxSpeed * m_maxSpeed)
        {
            m_rigidBody2D.velocity = m_rigidBody2D.velocity.normalized * m_maxSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        Vector2 force = normal * BounceForce;

        m_rigidBody2D.AddForceAtPosition(force * Power,  collision.contacts[0].point, ForceMode2D.Impulse);
        m_rigidBody2D.AddTorque(m_bounceSpinBurst * Power, ForceMode2D.Impulse);
    }

    public void Throw(Vector2 force)
    {
        m_rigidBody2D.AddForce(force, ForceMode2D.Impulse);
        m_rigidBody2D.AddTorque(m_initialSpinBurst * SpinDirection);
    }

    public void Pickup()
    {
        Destroy(gameObject);
    }
}
