using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavocController : MonoBehaviour
{
    HavocMovement m_havocMovement = null;
    bool m_canDashFromCharge = false;
    bool m_canThrowFromCharge = false;

    private void Awake()
    {
        m_havocMovement = GetComponent<HavocMovement>();
        m_canDashFromCharge = true;
        m_canThrowFromCharge = true;
    }

    private void Update()
    {
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector2 input = new Vector2(horiz, vert);
        m_havocMovement.InputDirection = input;

        bool dashButton = Input.GetButtonDown("Dash");
        float dashCharge = Input.GetAxis("DashCharge");
        if (dashButton || (!Mathf.Approximately(dashCharge, 0.0f) && m_canDashFromCharge))
        {
            print("Dash!");
            m_canDashFromCharge = false;
        }
        else if (Mathf.Approximately(dashCharge, 0.0f))
        {
            m_canDashFromCharge = true;
        }

        bool throwButton = Input.GetButtonDown("Throw");
        float throwCharge = Input.GetAxis("ThrowCharge");
        if (throwButton || (!Mathf.Approximately(throwCharge, 0.0f) && m_canThrowFromCharge))
        {
            print("Throw!");
            m_canThrowFromCharge = false;
        }
        else if (Mathf.Approximately(throwCharge, 0.0f))
        {
            m_canThrowFromCharge = true;
        }
    }
}
