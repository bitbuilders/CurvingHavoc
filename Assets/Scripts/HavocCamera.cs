using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavocCamera : Singleton<HavocCamera>
{
    [SerializeField, Range(0.0f, 10.0f)] float m_shakeAmplitude = 0.5f;
    [SerializeField, Range(0.0f, 10.0f)] float m_shakeRate = 1.5f;
    [SerializeField, Range(0.0f, 1.0f)] float m_shakeFalloff = 0.8f;
    [SerializeField, Range(0.0f, 1.0f)] float m_shakeRamp = 0.2f;

    Vector2 velocity = Vector2.zero;
    float m_duration = 0.0f;
    float m_time = 0.0f;

    private void Update()
    {
        m_time += Time.deltaTime;

        if (m_time > m_duration)
        {
            transform.position = Vector3.back * 10.0f;
            return;
        }

        float min = m_shakeRamp * m_duration;
        float max = m_shakeFalloff * m_duration;
        float p = 1.0f;
        if (m_time < min) p = m_time / min;
        else if (m_time > max) p = 1.0f - (m_time - max) / (m_duration - max);

        float x = (Mathf.PerlinNoise(Mathf.Cos(Time.time * m_shakeRate), 0.0f) * 2.0f - 1.0f) * m_shakeAmplitude;
        float y = (Mathf.PerlinNoise(0.0f, Mathf.Sign(Time.time * m_shakeRate)) * 2.0f - 1.0f) * m_shakeAmplitude;
        Vector2 v = new Vector2(x, y) * p;
        transform.position = (Vector3)v + Vector3.back * 10.0f;
    }

    public void ShakeScreen(float duration)
    {
        m_duration = duration;
        m_time = 0.0f;
    }
}
