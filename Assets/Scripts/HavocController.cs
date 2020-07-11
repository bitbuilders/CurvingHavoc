using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavocController : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField, Range(0.0f, 10.0f)] float m_dashDistance = 2.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_dashTime = 0.5f;

    [Header("Throw"), Space(5)]
    [SerializeField, Range(0.0f, 20.0f)] float m_maxCharge = 5.0f;
    [SerializeField, Range(0.0f, 20.0f)] float m_minCharge = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] float m_maxForceThreshold = 0.5f;
    [SerializeField, Range(0.0f, 100.0f)] float m_minThrowForce = 10.0f;
    [SerializeField, Range(0.0f, 100.0f)] float m_maxThrowForce = 20.0f;

    public bool IsTesting { get; set; }

    DXT3R m_dXT3R = null;
    HavocMovement m_havocMovement = null;
    HavocAnimator m_havocAnimator = null;
    float m_throwTime = 0.0f;
    bool m_thrown = false;
    bool m_canDashFromCharge = false;
    bool m_canThrowFromCharge = false;
    bool m_chargingThrow = false;

    private void Awake()
    {
        m_dXT3R = GetComponent<DXT3R>();
        m_havocMovement = GetComponent<HavocMovement>();
        m_havocAnimator = GetComponent<HavocAnimator>();
        m_canDashFromCharge = true;
    }

    private void Update()
    {
        if (!IsTesting) return;

        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector2 input = new Vector2(horiz, vert);
        m_havocMovement.InputDirection = input;

        bool dashButton = Input.GetButtonDown("Dash");
        float dashCharge = Input.GetAxis("DashCharge");
        if (dashButton || (!Mathf.Approximately(dashCharge, 0.0f) && m_canDashFromCharge))
        {
            print("Dash!");
            m_havocMovement.Dash(m_dashDistance, m_dashTime);
            m_havocAnimator.Slide(m_dashTime);
            m_canDashFromCharge = false;
        }
        else if (Mathf.Approximately(dashCharge, 0.0f))
        {
            m_canDashFromCharge = true;
        }

        if (!m_thrown)
        {
            bool throwButton = Input.GetButtonDown("Throw");
            bool throwHold = Input.GetButton("Throw");
            float throwCharge = Input.GetAxis("ThrowCharge");
            if (throwButton || (!Mathf.Approximately(throwCharge, 0.0f) && m_canThrowFromCharge))
            {
                print("Throw!");
                m_chargingThrow = true;
                // Do throw FX

            }
            if (throwHold || !Mathf.Approximately(throwCharge, 0.0f))
            {
                m_throwTime += Time.deltaTime;
                if (m_throwTime >= m_maxCharge)
                {
                    Throw();
                }
            }
            else
            {
                m_chargingThrow = false;
                Throw();
            }
        }
    }

    void Throw()
    {
        if (m_throwTime < m_minCharge)
        {
            // Give them a funny message???
            return;
        }

        float chargeMax = m_maxCharge * m_maxForceThreshold;
        if (chargeMax == 0.0f)
        {
            chargeMax = float.MinValue;
        }

        float power = Mathf.Clamp01(m_throwTime / chargeMax);
        float force = Mathf.Lerp(m_minThrowForce, m_maxThrowForce, power);
        
        // Need direction
    }
}
