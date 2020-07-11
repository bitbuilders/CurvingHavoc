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
    public Vector2 LastInputDirection { get; private set; } = Vector2.down;
    public Vector2 Velocity { get { return m_velocity; } }
    public float SpeedFactor { get { return m_velocity.sqrMagnitude / (m_maxSpeed * m_maxSpeed * ACCEL_FACTOR); } }
    public float InputStrength { get; set; }
    public bool Dashing { get; private set; }
    public bool IsTesting { get; set; }


    Rigidbody2D m_rigidbody2D = null;
    Coroutine m_dashRoutine = null;
    Vector2 m_velocity = Vector2.zero;
    Vector2 m_inpt = Vector2.zero;

    void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        if (!Mathf.Approximately(InputDirection.x, 0.0f) ||
            !Mathf.Approximately(InputDirection.y, 0.0f))
        {
            LastInputDirection = InputDirection;
        }
    }

    void FixedUpdate()
    {
        if (Dashing || !IsTesting) return;

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

    public void Dash(float distance, float time, bool overrideCurrentDash = true)
    {
        if (Dashing && m_dashRoutine != null && !overrideCurrentDash)
        {
            return;
        }
        else if (m_dashRoutine != null)
        {
            StopCoroutine(m_dashRoutine);
        }

        Dashing = true;
        m_dashRoutine = StartCoroutine(DoDash(distance, time));
    }

    IEnumerator DoDash(float distance, float time)
    {
        float t = 0.0f;
        float speed = distance / time;
        Vector2 dir = LastInputDirection;
        while (t <= time)
        {
            t += Time.deltaTime;
            Vector2 force = dir * speed * ACCEL_FACTOR;

            m_rigidbody2D.velocity = force;
            yield return null;
        }

        Dashing = false;
        m_dashRoutine = null;
    }
}
