using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HavocInc : Singleton<HavocInc>
{
    [SerializeField] List<HavocWave> m_waves = null;
    [SerializeField] DXT3R m_testSubject = null;
    [SerializeField] H4VC33 m_prototype = null;
    [SerializeField, Range(0.0f, 10.0f)] float m_endOfWaveDelay = 3.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_deathDelay = 3.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_titleEndDelay = 1.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_firstDialogueDelay = 3.0f;

    public DXT3R CurrentTester { get; private set; }
    public H4VC33 CurrentPrototype { get; private set; }
    public int Wave { get { return m_currentIteration; } }
    public string WaveTitle { get { return m_waves[Wave].Title; } }
    public string WaveDialogue { get { return m_waves[Wave].Dialogue; } }

    int m_currentIteration = 0;
    float m_deadTime = 0.0f;
    float m_winTime = 0.0f;
    int m_tip = 0;
    bool m_gameover = false;
    bool m_dead = false;

    private void Awake()
    {
        if (FindObjectsOfType<HavocInc>(true).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        //m_currentIteration = 3;
        SceneManager.sceneLoaded += OnSceneLoad;

        m_currentIteration++;
        CreateTester();
        //CurrentPrototype.Lifetime = 0.0f;
    }

    private void Start()
    {
        HavocNarrative.Instance.PlayTitle(true);
    }

    private void Update()
    {
        if (m_gameover) return;

        if (!CurrentTester.IsAlive)
        {
            m_deadTime += Time.deltaTime;
            if (m_deadTime >= m_deathDelay)
            {
                StartCoroutine(Restart());
                m_gameover = true;
                m_dead = true;
            }
        }
        else if (!CurrentPrototype.IsAlive)
        {
            m_winTime += Time.deltaTime;
            if (m_winTime >= m_endOfWaveDelay)
            {
                StartCoroutine(Progress());
                m_gameover = true;
                m_dead = false;
            }
        }
    }

    public void CreateTester(bool assignPrototype = true, bool demo = false)
    {
        CurrentTester = Instantiate(m_testSubject.gameObject).GetComponent<DXT3R>();
        CurrentTester.name = "DXT3R";
        CurrentTester.transform.position = Vector2.zero;

        if (assignPrototype)
        {
            AssignPrototypeToTester(demo);
        }
    }

    public void AssignPrototypeToTester(bool demo = false)
    {
        CurrentPrototype = Instantiate(m_prototype.gameObject).GetComponent<H4VC33>();
        CurrentPrototype.name = "H4VC33";
        CurrentPrototype.gameObject.SetActive(false);
        CurrentPrototype.transform.position = CurrentTester.transform.position;

        HavocWave wave = m_waves[Wave];
        CurrentPrototype.Lifetime = wave.Duration;
        CurrentPrototype.Level = wave.Difficulty;
        CurrentPrototype.MaxLevel = HavocWave.MAX_LEVEL;
        CurrentPrototype.Owner = CurrentTester;

        CurrentTester.Prototype = CurrentPrototype;
    }

    public void BeginTest()
    {
        CurrentTester.StartTesting();
    }

    public void Reset()
    {

    }

    public void OnTitleFinish()
    {
        StartCoroutine(TitleToDialogue());
    }

    IEnumerator TitleToDialogue()
    {
        BeginTest();

        yield return new WaitForSeconds(m_titleEndDelay);

        float delay = (Wave <= 1) ? m_firstDialogueDelay : -1.0f;
        HavocNarrative.Instance.PlayDialogue(delay);
    }

    public void OnDialogueFinish()
    {

    }

    IEnumerator Restart()
    {
        float tintTime = 1.0f;
        HavocCanvas.Instance.FadeTint(true, tintTime, null);

        StartCoroutine(FadeVolume());

        yield return new WaitForSeconds(tintTime + 0.5f);

        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    IEnumerator Progress()
    {
        float tintTime = 1.0f;
        HavocCanvas.Instance.FadeTint(true, tintTime, null);
        m_currentIteration++;

        StartCoroutine(FadeVolume());

        yield return new WaitForSeconds(tintTime + 0.5f);

        if (Wave >= m_waves.Count)
        {
            SceneManager.LoadScene(1, LoadSceneMode.Single);
            SceneManager.sceneLoaded -= OnSceneLoad;
            Destroy(gameObject);
        }
        else
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }

    IEnumerator FadeVolume()
    {
        AudioSource audio = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
        float startVol = audio.volume;
        float time = 0.5f;
        for (float i = 0.0f; i < time; i += Time.deltaTime)
        {
            float a = 1.0f - i / time;
            audio.volume = startVol * a;
            yield return null;
        }

        audio.volume = 0.0f;
    }

    void NewTester()
    {
        Destroy(CurrentTester.gameObject);
        Destroy(CurrentPrototype.gameObject);
        CreateTester();
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (!m_gameover) return;

        StartCoroutine(OnLoad());
    }

    IEnumerator OnLoad()
    {
        yield return new WaitForSeconds(0.0f);

        int tip = m_dead ? m_tip : -1;
        if (m_dead)
        {
            Reset();
        }
        m_dead = false;

        CreateTester();
        HavocNarrative.Instance.PlayTitle(true, tip);
        m_tip++;
        m_gameover = false;
        m_deadTime = 0.0f;
        m_winTime = 0.0f;
    }
}
