using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "HavocWave", menuName = "HavocWave")]
public class HavocWave : ScriptableObject
{
    public const int MAX_LEVEL = 20;

    public string Title;
    [TextArea]
    public string Dialogue;
    [Range(0.0f, 180.0f)]
    public float Duration;
    [Range(0, MAX_LEVEL)]
    public int Difficulty;
}
