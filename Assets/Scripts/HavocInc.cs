using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavocInc : Singleton<HavocInc>
{
    [SerializeField] List<HavocWave> m_waves = null;
    [SerializeField] DXT3R m_testSubject = null;
    [SerializeField] H4VC33 m_prototype = null;
    [SerializeField, Range(0.0f, 10.0f)] float m_endOfWaveDelay = 3.0f;
    [SerializeField, Range(0.0f, 10.0f)] float m_deathDelay = 3.0f;

    public DXT3R CurrentTester { get; private set; }
    public H4VC33 CurrentPrototype { get; private set; }
    public int Wave { get { return m_currentIteration; } }
    public string WaveTitle { get { return m_waves[Wave].Title; } }
    public string WaveDialogue { get { return m_waves[Wave].Dialogue; } }

    int m_currentIteration = 0;

    private void Awake()
    {
        if (FindObjectsOfType<HavocInc>(true).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        CreateTester();
        BeginTest();
    }

    private void Start()
    {
        HavocNarrative.Instance.PlayTitle(true);
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

        if (!demo)
        {
            m_currentIteration++;
        }

        HavocWave wave = m_waves[m_currentIteration];
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

    public void ResetWaves()
    {
        m_currentIteration = 0;
    }


}
