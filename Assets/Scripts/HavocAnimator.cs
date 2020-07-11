﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavocAnimator : MonoBehaviour
{

    HavocMovement m_havocMovement = null;
    Animator m_animator = null;
    Vector2 m_lastDirection = Vector2.down;

    void Awake()
    {
        m_havocMovement = GetComponent<HavocMovement>();
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!Mathf.Approximately(m_havocMovement.InputDirection.x, 0.0f) ||
            !Mathf.Approximately(m_havocMovement.InputDirection.y, 0.0f))
        {
            // Keep track of last direction for Idle
            m_lastDirection = m_havocMovement.InputDirection;
        }

        if (Mathf.Approximately(m_lastDirection.magnitude, 1.0f) && Mathf.Abs(m_lastDirection.y) > 0.0f)
        {
            // Make moving up/down visually have priority
            m_lastDirection.x = 0.5f * Mathf.Sign(m_lastDirection.x);
            m_lastDirection.Normalize();
        }

        m_animator.SetFloat("XDir", m_lastDirection.x);
        m_animator.SetFloat("YDir", m_lastDirection.y);
        m_animator.SetFloat("Speed", m_havocMovement.SpeedFactor);
    }

    public void Slide()
    {
        m_animator.SetTrigger("Slide");
    }
}