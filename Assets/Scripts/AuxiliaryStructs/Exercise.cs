using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Exercise
{
    public List<PlayerExercise> playerExercises;
}

[Serializable]
public struct PlayerExercise
{
    public string displayMessage;
    public string targetWord;
}
