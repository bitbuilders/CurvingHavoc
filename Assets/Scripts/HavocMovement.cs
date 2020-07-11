using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavocMovement : MonoBehaviour
{
    [SerializeField, Range(0.0f, 100.0f)] float m_accel = 15.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_maxSpeed = 3.0f;
    [SerializeField, Range(0.0f, 1.0f)] float m_friction = 0.9f;

    const float ACCEL_FACTOR = 1.0f;

    public Vector2 InputDirection 
    { 
        get { return m_inpt; } 
        set 
        {
            InputStrength = Mathf.Clamp01(m_inpt.magnitude);
            m_inpt = value.normalized; 
        } 
    }
    public Vector2 Velocity { get { return m_velocity; } }
    public float SpeedFactor { get { return m_velocity.sqrMagnitude / (m_maxSpeed * m_maxSpeed * ACCEL_FACTOR); } }
    public float InputStrength { get; set; }

    Rigidbody2D m_rigidbody2D = null;
    Vector2 m_velocity = Vector2.zero;
    Vector2 m_inpt = Vector2.zero;

    void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float acc = m_accel * ACCEL_FACTOR;
        m_velocity += InputDirection * InputStrength * acc * Time.deltaTime;

        if (Mathf.Approximately(Mathf.Abs(InputDirection.x), 0.0f))
        {
            m_velocity.x *= m_friction;
        }
        if (Mathf.Approximately(Mathf.Abs(InputDirection.y), 0.0f))
        {
            m_velocity.y *= m_friction;
        }

        if (m_velocity.sqrMagnitude > m_maxSpeed * m_maxSpeed * ACCEL_FACTOR)
        {
            m_velocity = m_velocity.normalized * m_maxSpeed * ACCEL_FACTOR;
        }

        m_rigidbody2D.velocity = m_velocity;
    }
}
