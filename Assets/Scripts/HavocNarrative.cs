using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HavocNarrative : Singleton<HavocNarrative>
{
    [SerializeField, Range(0.0f, 2.0f)] float m_timePerCharacter = 0.1f;
    [SerializeField, Range(0.0f, 20.0f)] float m_startDelay = 0.5f;
    [SerializeField, Range(0.0f, 20.0f)] float m_endDelay = 2.0f;
    [SerializeField, Range(0.0f, 20.0f)] float m_titleStartDelay = 0.5f;
    [SerializeField, Range(0.0f, 10.0f)] float m_titleTextFade = 1.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_tintFade = 1.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_tintDelay = 0.5f;

    public void PlayDialogue(float endDelayOverride = -1.0f)
    {
        string dialogue = HavocInc.Instance.WaveDialogue;
        HavocCanvas.Instance.Dialogue.Text.text = dialogue;

        HavocCanvas.Instance.ShowDialogue();

        Action hide = ()=> HavocCanvas.Instance.HideDialogue();
        float endDelay = (endDelayOverride >= 0.0f) ? endDelayOverride : m_endDelay;
        StartCoroutine(IterateText(HavocCanvas.Instance.Dialogue.Text, m_startDelay, endDelay, false, hide));
    }

    public void PlayTitle(bool instant, float endDelayOverride = -1.0f)
    {
        string title = HavocInc.Instance.WaveTitle;
        HavocCanvas.Instance.Title.text = title;

        HavocCanvas.Instance.ShowTitle();

        Action hide = ()=> HavocCanvas.Instance.HideTitle(m_titleTextFade, m_tintFade, m_tintDelay);
        float delay = (endDelayOverride >= 0.0f) ? endDelayOverride : m_endDelay;
        StartCoroutine(IterateText(HavocCanvas.Instance.Title, m_titleStartDelay, delay, instant, hide));
    }

    IEnumerator DelayCallback(float time, Action action)
    {
        yield return new WaitForSeconds(time);

        action?.Invoke();
    }

    IEnumerator IterateText(TMP_Text text, float startDelay, float endDelay, bool instant, Action endAction)
    {
        yield return new WaitForSeconds(startDelay);

        if (instant)
        {
            endAction?.Invoke();
        }
        else
        {
            string t = text.text;
            text.text = "";

            float charTime = 0.0f;
            int c = 0;
            while (c < t.Length)
            {
                charTime += Time.deltaTime;
                if (charTime >= m_timePerCharacter)
                {
                    charTime -= m_timePerCharacter;

                    text.text += t[c];
                    c++;
                }
                yield return null;
            }

            yield return new WaitForSeconds(endDelay);

            endAction?.Invoke();
        }
    }
}
