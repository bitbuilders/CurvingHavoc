using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavocInc : Singleton<HavocInc>
{
    [SerializeField] List<HavocWave> m_waves = null;
    [SerializeField] DXT3R m_testSubject = null;
    [SerializeField] H4VC33 m_prototype = null;

    public DXT3R CurrentTester { get; private set; }
    public H4VC33 CurrentPrototype { get; private set; }

    int m_currentIteration = 0;

    private void Awake()
    {
        if (FindObjectsOfType<HavocInc>(true).Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        CreateTester();
        BeginTest();
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
}
