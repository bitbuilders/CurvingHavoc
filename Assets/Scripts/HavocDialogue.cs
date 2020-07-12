using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HavocDialogue : MonoBehaviour
{
    public TMP_Text Text = null;

    [SerializeField, Range(0.0f, 5.0f)] float m_introTime = 0.5f;
    [SerializeField, Range(0.0f, 5.0f)] float m_outroTime = 0.5f;
    [SerializeField] AnimationCurve m_intro = null;
    [SerializeField] AnimationCurve m_outro = null;

    AudioSource m_announcerSound = null;

    private void Awake()
    {
        SetPosition(GetComponent<RectTransform>().rect.height);
        m_announcerSound = GetComponent<AudioSource>();
    }

    public void Show()
    {
        StartCoroutine(Move(true, m_introTime, m_intro));
        m_announcerSound.Play();
    }

    public void Hide()
    {
        StartCoroutine(Move(false, m_outroTime, m_outro));
    }

    IEnumerator Move(bool moveIn, float time, AnimationCurve posOverTime)
    {
        Rect rect = GetComponent<RectTransform>().rect;
        float height = rect.height;
        float startY = height;
        float endY = 0.0f;

        for (float i = 0.0f; i < time; i += Time.deltaTime)
        {
            float a = i / time;
            if (!moveIn) a = 1.0f - a;

            float t = posOverTime.Evaluate(a);
            float pos = Mathf.Lerp(startY, endY, t);
            SetPosition(pos);

            yield return null;
        }

        SetPosition((moveIn) ? endY : startY);
    }

    void SetPosition(float y)
    {
        GetComponent<RectTransform>().anchoredPosition = Vector3.up * y;
    }
}
