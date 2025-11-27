using NUnit.Framework;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CreateAssetMenu(fileName = "LevelRef", menuName = "Scriptable Objects/LevelRef")]
public class LevelRef : ScriptableObject
{

    [Header("The list for random selection of scenes")]
    [Header("----------------------------------------")]
    [Space]
    [Header("NAMING CONVENTION: Level (round#_levelIdx")]
    [Header("EXAMPLE: Level 1_3")]
    [Header("(For the third possible level in round 1)")]
    [Header("**FOLLOW NAMING CONVENTION**")]
    [Header("----------------------------------------")]

    public List<string> sceneList = new List<string>();
}
