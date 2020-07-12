using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HavocCanvas : Singleton<HavocCanvas>
{
    [SerializeField] Image m_screenTint;
    [SerializeField] TMP_Text m_title;
    [SerializeField] HavocDialogue m_dialogue;
    [SerializeField] GameObject m_pauseMenu;
    [SerializeField] Button m_resume;
    [SerializeField] Button m_cancel;

    public TMP_Text Title { get { return m_title; } }
    public HavocDialogue Dialogue { get { return m_dialogue; } }
    public bool Paused { get; private set; }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (HavocInc.Instance.CurrentTester.IsAlive && HavocInc.Instance.CurrentPrototype.IsAlive)
            {
                Pause(!Paused);
            }
        }
    }

    public void Pause(bool pause)
    {
        Paused = pause;
        m_pauseMenu.SetActive(Paused);
        Time.timeScale = Paused ? 0.0f : 1.0f;

        if (Paused)
        {
            FindObjectOfType<EventSystem>(true).SetSelectedGameObject(m_cancel.gameObject);
            FindObjectOfType<EventSystem>(true).SetSelectedGameObject(m_resume.gameObject);
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

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

    public void HideTitle(float delay, float textFadeTime, float tintFadeTime, float tintDelay)
    {
        StartCoroutine(ClearScreen(delay, textFadeTime, tintFadeTime, tintDelay));
    }

    IEnumerator ClearScreen(float delay, float textFadeTime, float tintFadeTime, float tintDelay)
    {
        yield return new WaitForSeconds(delay);

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
