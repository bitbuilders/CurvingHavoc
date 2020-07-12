using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavocNavigator : Singleton<HavocNavigator>
{
    [SerializeField, Range(0.0f, 20.0f)] float m_maxRange = 5.0f;
    [SerializeField, Range(0.0f, 50.0f)] float m_rotationSpeed = 10.0f;
    [SerializeField, Range(0.0f, 5.0f)] float m_offset = 0.5f;
    [SerializeField] GameObject m_head = null;
    [SerializeField] GameObject m_line = null;

    public Vector2 Dir { get; set; } = Vector2.up;
    public Vector2 CurrentDir { get; set; }
    public float Power { get; set; }

    List<GameObject> m_lines = null;
    float m_lastPower = 0.0f;

    private void Awake()
    {
        m_lines = new List<GameObject>();

        for (int i = 0; i < Mathf.FloorToInt(m_maxRange); i++)
        {
            m_lines.Add(Instantiate(m_line));
            m_lines[i].GetComponent<SpriteRenderer>().enabled = false;
            m_lines[i].transform.SetParent(transform);
        }

        SetLineOpacity(0.0f);
    }

    private void LateUpdate()
    {
        if (Power <= 0.0f)
        {
            if (m_lastPower > 0.0f)
            {
                Hide();
            }
            return;
        }
        else
        {
            if (m_lastPower <= 0.0f)
            {
                Show();
            }
        }

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        float horiz = Input.GetAxis("Look_Horizontal");
        float vert = Input.GetAxis("Look_Vertical");
        float vertPS4 = Input.GetAxis("Look_Horizontal_PS4");
        if (Mathf.Approximately(vert, 0.0f))
        {
            vert = vertPS4;
            if (Mathf.Approximately(vert, 0.0f))
            {
                // Switch
            }
        }
        Vector2 joystick = new Vector2(horiz, -1.0f * vert);

        if (!Mathf.Approximately(joystick.x, 0.0f) || !Mathf.Approximately(joystick.x, 0.0f))
        {
            Dir = joystick;
        }
        else if (!Mathf.Approximately(mouseDelta.x, 0.0f) || !Mathf.Approximately(mouseDelta.x, 0.0f) || Input.GetButtonDown("Throw"))
        {
            Vector2 worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = worldMouse - (Vector2)HavocInc.Instance.CurrentTester.transform.position;
            Dir = dir;
        }

        CurrentDir = Vector2.Lerp(CurrentDir, Dir, Time.deltaTime * m_rotationSpeed);
        transform.up = CurrentDir;
        transform.position = HavocInc.Instance.CurrentTester.transform.position;

        m_head.transform.localPosition = Vector3.up * Power * m_maxRange;
        SetLinePositions();

        Dir.Normalize();
        m_lastPower = Power;
    }

    void SetLinePositions()
    {
        float dist = Power * m_maxRange - m_offset;
        float numOfLines = Mathf.Floor(dist);
        float diff = dist - numOfLines;
        float extra = diff / numOfLines;

        foreach (GameObject line in m_lines)
        {
            line.GetComponent<SpriteRenderer>().enabled = false;
        }

        for (int i = 0; i < (int)numOfLines; i++)
        {
            GameObject line = m_lines[i];
            line.GetComponent<SpriteRenderer>().enabled = true;

            line.transform.localPosition = (Vector3.up * i + (Vector3.up * extra * i)) + Vector3.up * m_offset;
            line.transform.localScale = new Vector3(1.0f, 1.0f + extra, 0.0f);
        }
    }

    public void Show()
    {
        StartCoroutine(ShowLine());
    }

    public void Hide()
    {
        m_lastPower = 0.0f;
        StartCoroutine(FadeLine());
    }

    IEnumerator ShowLine()
    {
        float max = 0.25f;
        for (float i = 0.0f; i <= 0.5f; i += Time.deltaTime)
        {
            SetLineOpacity((i / max));
            yield return null;
        }

        SetLineOpacity(1.0f);
    }

    IEnumerator FadeLine()
    {
        float max = 0.25f;
        for (float i = 0.0f; i <= 0.5f; i += Time.deltaTime)
        {
            SetLineOpacity(1.0f - (i / max));
            yield return null;
        }

        SetLineOpacity(0.0f);
    }

    void SetLineOpacity(float a)
    {
        Color c = Color.white;
        c.a = a;
        m_head.GetComponent<SpriteRenderer>().color = c;

        foreach (GameObject line in m_lines)
        {
            line.GetComponent<SpriteRenderer>().color = c;
        }
    }
}
