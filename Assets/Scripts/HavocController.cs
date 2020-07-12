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
    [SerializeField, Range(0.0f, 10.0f)] float m_throwLinger = 1.5f;
    [SerializeField, Range(0.0f, 10.0f)] float m_throwDecay = 1.5f;

    public bool IsTesting { get; set; }
    public float ThrowPower 
    { 
        get 
        {
            float chargeMax = m_maxCharge * m_maxForceThreshold;
            if (chargeMax == 0.0f)
            {
                chargeMax = float.MinValue;
            }
            return Mathf.Clamp01(m_throwTime / chargeMax);
        }
    }

    DXT3R m_dXT3R = null;
    HavocMovement m_havocMovement = null;
    HavocAnimator m_havocAnimator = null;
    float m_throwTime = 0.0f;
    bool m_thrown = false;
    bool m_canDashFromCharge = false;
    bool m_canThrowFromCharge = false;

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
                if (m_throwTime > 0.0f)
                {
                    Throw();
                }

                m_throwTime -= Time.deltaTime * 2.0f;
                if (m_throwTime < 0.0f) m_throwTime = 0.0f;
            }

            HavocNavigator.Instance.Power = ThrowPower;
        }
    }

    void Throw()
    {
        if (m_throwTime < m_minCharge)
        {
            // Give them a funny message???
            print("FAIL");
            return;
        }

        m_thrown = true;
        m_throwTime = 0.0f;
        HavocNavigator.Instance.Power = 0.0f;

        float force = Mathf.Lerp(m_minThrowForce, m_maxThrowForce, ThrowPower);

        // Need direction
        m_dXT3R.Prototype.gameObject.SetActive(true);
        m_dXT3R.Prototype.transform.position = transform.position;
        m_dXT3R.Prototype.Throw(HavocNavigator.Instance.CurrentDir, force, m_throwLinger, m_throwDecay, 0.1f);
    }
}
