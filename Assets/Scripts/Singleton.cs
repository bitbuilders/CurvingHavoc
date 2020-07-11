using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T m_instance;

    public static T Instance
    {
        get
        {
            if (!m_instance)
            {
                m_instance = FindObjectOfType<T>(true);
            }

            return m_instance;
        }
    }
}
