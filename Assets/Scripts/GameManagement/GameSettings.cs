using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    
[Serializable]
public class ScoreValue
{
    public bool usefulForMe;
    public bool usefulForOther;

    public string diffLetters;

    public int myValue;
    public int otherValue;
}
[Serializable]
public class ScoreSystem
{   
    public List<ScoreValue> giveScores;
    public List<ScoreValue> takeScores;

    public int completeWordMyScore;
    public int completeWordOtherScore;
}

[Serializable]
public class ExercisesListWrapper
{
    public List<Exercise> exercises;
}

[Serializable]
public class ExerciseGroupsWrapper
{
    public List<ExercisesListWrapper> exerciseGroups;
}

[Serializable]
public struct GeneralSettings
{
    public List<Player> players;
    public int numLevels;
    public float playersLettersSpawnP;
    public string logMode;
    public string databaseName;
    public string mongoDbKey;
}

[Serializable]
public struct GameSettings
{
    public ExerciseGroupsWrapper exercisesGroups;
    public GeneralSettings generalSettings;
    public ScoreSystem scoreSystem;
}

