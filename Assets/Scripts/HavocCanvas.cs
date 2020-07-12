using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HavocCanvas : Singleton<HavocCanvas>
{
    [SerializeField] Image m_screenTint;
    [SerializeField] TMP_Text m_title;
    [SerializeField] HavocDialogue m_dialogue;

    public TMP_Text Title { get { return m_title; } }
    public HavocDialogue Dialogue { get { return m_dialogue; } }

    public void FadeTitle(bool fadeIn, float fadeTime, AnimationCurve fadeOverTime)
    {
        StartCoroutine(FadeText(m_title, fadeIn, fadeTime, fadeOverTime));
    }

    public void FadeTint(bool fadeIn, float fadeTime, AnimationCurve fadeOverTime)
    {
        StartCoroutine(FadeImage(m_screenTint, fadeIn, fadeTime, fadeOverTime));
    }

    public void ShowDialogue()
    {
        Dialogue.Show();
    }

    public void HideDialogue()
    {
        Dialogue.Hide();
    }

    public void ShowTitle()
    {
        SetTextOpacity(m_title, 1.0f);
        SetImageOpacity(m_screenTint, 1.0f);
    }

    public void HideTitle(float textFadeTime, float tintFadeTime, float tintDelay)
    {
        StartCoroutine(ClearScreen(textFadeTime, tintFadeTime, tintDelay));
    }

    IEnumerator ClearScreen(float textFadeTime, float tintFadeTime, float tintDelay)
    {
        FadeTitle(false, textFadeTime, null);

        yield return new WaitForSeconds(tintDelay);

        FadeTint(false, tintFadeTime, null);
    }

    IEnumerator FadeText(TMP_Text text, bool fadeIn, float duration, AnimationCurve fadeOverTime)
    {
        for (float i = 0.0f; i < duration; i += Time.deltaTime)
        {
            float a = i / duration;
            if (!fadeIn) a = 1.0f - a;

            SetTextOpacity(text, a);
            yield return null;
        }

        SetTextOpacity(text, fadeIn ? 1.0f : 0.0f);
    }

    IEnumerator FadeImage(Image image, bool fadeIn, float duration, AnimationCurve fadeOverTime)
    {
        for (float i = 0.0f; i < duration; i += Time.deltaTime)
        {
            float a = i / duration;
            if (!fadeIn) a = 1.0f - a;

            SetImageOpacity(image, a);
            yield return null;
        }

        SetImageOpacity(image, fadeIn ? 1.0f : 0.0f);
    }

    void SetTextOpacity(TMP_Text text, float a)
    {
        Color c = text.color;
        c.a = a;
        text.color = c;
    }

    void SetImageOpacity(Image image, float a)
    {
        Color c = image.color;
        c.a = a;
        image.color = c;
    }
}
