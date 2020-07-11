using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HavocInc : Singleton<HavocInc>
{
    [SerializeField] List<HavocWave> m_waves = null;
    [SerializeField] DXT3R m_testSubject = null;


}
