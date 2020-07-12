using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavocPole : MonoBehaviour
{
    [SerializeField, Range(0, HavocWave.MAX_LEVEL)] int m_wave = HavocWave.MAX_LEVEL;
    [SerializeField, Range(0, 100)] int m_zIndexNormal = 5;
    [SerializeField, Range(0, 100)] int m_zIndexInFront = 15;

    SpriteRenderer m_spriteRenderer = null;

    void Start()
    {
        if (HavocInc.Instance.Wave >= m_wave)
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        float y = transform.position.y;
        DXT3R dXT3R = HavocInc.Instance.CurrentTester;
        if (dXT3R)
        {
            if (dXT3R.transform.position.y > y)
            {
                m_spriteRenderer.sortingOrder = m_zIndexInFront;
            }
            else
            {
                m_spriteRenderer.sortingOrder = m_zIndexNormal;
            }
        }
    }
}
