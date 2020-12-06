using System;
using System.Collections;
using System.Collections.Generic;
using AuxiliaryStructs;
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
    public string path;

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
public class ScoreSystemParam
{
    public string prefix;
    public string filename;
}

[Serializable]
public struct GeneralSettings
{
    public List<ScoreSystemParam> scoreSystemParams;
    public List<PlayerInfo> playersParams;
    public int numLevels;
    public float playersLettersSpawnP;
    public string logMode;
    public string databaseName;
    public string mongoConnector;
}

[Serializable]
public struct MANetworkSettings
{
    public string currMultiplayerOption;

    //for online games
    public string currOnlineOption;
    public string serverIP;
}

[Serializable]
public struct GameSettings
{
    public MANetworkSettings networkSettings;
    public ExerciseGroupsWrapper exercisesGroups;
    public GeneralSettings generalSettings;
    public ScoreSystem scoreSystem;
}

