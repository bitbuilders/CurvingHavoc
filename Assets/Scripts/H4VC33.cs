using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H4VC33 : MonoBehaviour
{
    [Header("Constants")]
    [SerializeField, Range(0.0f, 100.0f)] float m_constantSpin = 5.0f;
    [SerializeField, Range(-1, 1)] int m_spinDirection = 1;

    [Header("Initial"), Space(10)]
    [SerializeField, Range(0.0f, 100.0f)] float m_initialSpinBurst = 15.0f;

    PolygonCollider2D m_collider = null;
    Rigidbody2D m_rigidBody2D = null;
    float m_dir = 0.0f;

    void Awake()
    {
        m_collider = GetComponent<PolygonCollider2D>();
        m_rigidBody2D = GetComponent<Rigidbody2D>();

        m_dir = Mathf.Abs(m_spinDirection) == 1.0f ? m_spinDirection : 1.0f;
        //m_rigidBody2D.AddTorque(m_initialSpinBurst * m_dir);
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        m_rigidBody2D.AddTorque(m_constantSpin * m_dir * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        Vector2 force = normal * 5.0f;

        m_rigidBody2D.AddForce(force, ForceMode2D.Impulse);
    }
}
