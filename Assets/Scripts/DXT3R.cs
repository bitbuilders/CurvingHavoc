using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DXT3R : MonoBehaviour
{
    public H4VC33 Prototype { get; set; }

    HavocController m_havocController = null;
    HavocMovement m_havocMovement = null;

    private void Awake()
    {
        m_havocController = GetComponent<HavocController>();
        m_havocMovement = GetComponent<HavocMovement>();
    }

    public void StartTesting()
    {
        m_havocController.IsTesting = true;
        m_havocMovement.IsTesting = true;
    }
}
