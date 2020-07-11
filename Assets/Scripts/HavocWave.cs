using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HavocWave", menuName = "HavocWave")]
public class HavocWave : ScriptableObject
{
    public string Title;
    [TextArea]
    public string Dialogue;
    [Range(0.0f, 180.0f)]
    public float Duration;
    [Range(0, 20)]
    public int Difficulty;
}
