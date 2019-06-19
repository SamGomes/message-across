﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Globals
{
    public enum PlayerId
    {
        PLAYER_0,
        PLAYER_1,
        PLAYER_2,
        NONE
    }

    public enum ButtonId
    {
        BTN_0,
        BTN_1,
        BTN_2,
        NONE
    }
    
    public enum KeyInteractionType
    {
        NONE,
        TAKE,
        GIVE
    }

    public static KeyInteractionType[] keyInteractionTypes = (KeyInteractionType[]) Enum.GetValues(typeof(Globals.KeyInteractionType));
    public static ButtonId[] buttonIds = (ButtonId[]) Enum.GetValues(typeof(Globals.ButtonId));
    
    public static int currLevelId = 0;
}

[Serializable]
public class ScoreValue
{
    public bool usefulForMe;
    public bool usefulForOther;
    public int myValue;
    public int otherValue;
}
[Serializable]
public class ScoreSystem
{
    public List<ScoreValue> giveScores;
    public List<ScoreValue> takeScores;
}

[Serializable]
public class ExercisesListWrapper
{
    public List<Exercise> exercises;
}
[Serializable]
public struct GameSettings
{
    public int maxSimultaneousKeyPresses;
    public List<ExercisesListWrapper> exercisesGroups;
    public List<Player> players;
    public int gameId;

    public ScoreSystem scoreSystem;
}

[Serializable]
public struct PerformanceMetrics
{
    public Dictionary<Player,int> singlebuttonHits;
    public int multiplayerButtonHits;
}




public class GameManager : MonoBehaviour
{

    private int exerciseGroupIndex;

    public GameSettings settings;
    private PerformanceMetrics performanceMetrics;



    public GameSceneManager gameSceneManager;

    public bool isGameplayPaused;
    public bool isGameplayStarted;

    public GameObject namesInputLocation;
    public GameObject playerNameInputFieldPrefabRef;

    public GameObject hpPanel;
    public GameObject displayPanel;
    public GameObject scorePanel;
    public GameObject timePanel;
    public GameObject reqPanel;
    public GameObject track;

    public InputManager inputManager;
    public LogManager logManager;

    public GameObject playersPanel;
    public AntSpawner[] antSpawners;
    public LetterSpawner[] letterSpawners;

    public List<Button> gameButtons;

    
    public Dictionary<Player, Exercise> currExercises;
    public Dictionary<Player, string> currWordStates;


    public float timeLeft;

    public void PauseGame()
    {
        Time.timeScale = 0;
        isGameplayPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        isGameplayPaused = false;
    }

    public string OrderedFormOfNumber(int i)
    {
        string suffix = "th";
        switch (i)
        {
            case 1:
                suffix = "st";
                break;
            case 2:
                suffix = "nd";
                break;
            case 3:
                suffix = "rd";
                break;
        }
        return i + suffix;
    }
    private void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


    private void UpdateButtonColors()
    {
        gameButtons.ForEach(delegate (Button button) { button.GetComponent<SpriteRenderer>().color = Color.white; });
        for(int i=0; i< gameButtons.Count; i++)
        {
            gameButtons[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/button");
            int numActivePlayers = 0;
            foreach(Player player in settings.players)
            {
                int activeIndex = player.GetActivebuttonIndex();
                if (activeIndex == i)
                {
                    gameButtons[activeIndex].GetComponent<SpriteRenderer>().color = player.GetButtonColor();
                    numActivePlayers++;
                }
            }
            if (numActivePlayers > 1)
            {
                //gameButtons[i].GetComponent<SpriteRenderer>().color = new Color(1.0f,0.5f,0.0f,1.0f);
                gameButtons[i].GetComponent<SpriteRenderer>().color = Color.white;
                gameButtons[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/mixedButton");
            }
        }
    }


    private IEnumerator YieldedStart()
    {

        currExercises = new Dictionary<Player, Exercise>();
        currWordStates = new Dictionary<Player, string>();


        isGameplayPaused = false;
        gameSceneManager.MainSceneLoadedNotification();

        //settings = new GameSettings();

        //settings.maxSimultaneousKeyPresses = 2;

        //settings.players = new List<Player>();
        //settings.players.Add(new Player(new List<KeyCode>() { KeyCode.Q, KeyCode.W, KeyCode.E }, new List<string> { "YButtonJoy1", "BButtonJoy1" }));
        //settings.players.Add(new Player(new List<KeyCode>() { KeyCode.I, KeyCode.O, KeyCode.P }, new List<string> { "YButtonJoy2", "BButtonJoy2" }));
        ////players.Add(new Player(Utilities.PlayerId.PLAYER_2, new KeyCode[] { KeyCode.V, KeyCode.B, KeyCode.N }, new string[] { "YButtonJoy3" , "BButtonJoy3" }));


        //settings.exercises = new List<Exercise>();
        //settings.exercises.Add(new Exercise("Fck yourself:_", "HFESUIHFUESIGHUFISEHUFESIHFESI"));
        ////exercises.Add(new Exercise("Word to match: CAKE \n Your Word:_", "CAKE"));
        ////exercises.Add(new Exercise("Word to match: BANANA \n Your Word:_", "BANANA"));
        ////exercises.Add(new Exercise("Word to match: PIE \n Your Word:_", "PIE"));
        ////exercises.Add(new Exercise("Word to match: PIZZA \n Your Word:_", "PIZZA"));
        ////exercises.Add(new Exercise("Word to match: CROISSANT \n Your Word:_", "CROISSANT"));
        ////exercises.Add(new Exercise("Word to match: DONUT \n Your Word:_", "DONUT"));
        ////exercises.Add(new Exercise("Word to match: CHERRY \n Your Word:_", "CHERRY"));
        ////exercises.Add(new Exercise("Word to match: XMASCOOKIES \n Your Word:_", "XMASCOOKIES"));
        ////exercises.Add(new Exercise("Word to match: KIWI \n Your Word:_", "KIWI"));
        ////exercises.Add(new Exercise("Word to match: QUICHE \n Your Word:_", "QUICHE"));
        ////exercises.Add(new Exercise("Word to match: MANGO \n Your Word:_", "MANGO"));
        ////exercises.Add(new Exercise("Word to match: FISH \n Your Word:_", "FISH"));
        ////exercises.Add(new Exercise("Word to match: VANILLA \n Your Word:_", "VANILLA"));
        ////exercises.Add(new Exercise("Word to match: JELLY \n Your Word:_", "JELLY"));


        string path = Application.streamingAssetsPath + "/config.cfg";
        string configText = "";
        if (path.Contains("://") || path.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();
            configText = www.downloadHandler.text;
        }
        else
        {
            configText = File.ReadAllText(path);
        }

        //string json = JsonUtility.ToJson(settings, true);
        settings = JsonUtility.FromJson<GameSettings>(configText);

        //Application.targetFrameRate = 60;

        timeLeft = 100.0f;
        InvokeRepeating("DecrementTimeLeft", 0.0f, 1.0f);

        ChangeGameParametrizations(true);

        gameSceneManager.StartAndPauseGame(); //for the initial screen

        performanceMetrics = new PerformanceMetrics();
        performanceMetrics.singlebuttonHits = new Dictionary<Player, int>();
        for (int i = 0; i < settings.players.Count; i++)
        {
            Player currPlayer = settings.players[i];
            currPlayer.Init();
            GameObject newNameInput = Instantiate(playerNameInputFieldPrefabRef, namesInputLocation.transform);
            InputField input = newNameInput.GetComponent<InputField>();
            input.placeholder.GetComponent<Text>().text = OrderedFormOfNumber(i + 1) + " player name";

            ChangeTargetWord(currPlayer);

            input.onValueChanged.AddListener(delegate
            {
                List<InputField> inputFields = new List<InputField>(namesInputLocation.GetComponentsInChildren<InputField>());
                settings.players[inputFields.IndexOf(input)].SetName(input.text);
            });

            performanceMetrics.singlebuttonHits.Add(currPlayer, 0);

            List<KeyCode> keys = currPlayer.GetMyKeys();

            UpdateButtonColors();

            for (int j = 1; j < Globals.keyInteractionTypes.Length ; j++)
            {
                Globals.KeyInteractionType currInt = (Globals.KeyInteractionType) j;
                inputManager.AddKeyBinding(
                    new List<KeyCode>(){ keys[j] }, InputManager.ButtonPressType.PRESSED, delegate (List<KeyCode> triggeredKeys)
                    {
                        currPlayer.SetActiveInteraction(currInt);
                        int activeIndex = currPlayer.GetActivebuttonIndex();
                        gameButtons[activeIndex].RegisterButtonPress();//Utilities.interactionTypes[j]);
                    }, false, false);
            }
            inputManager.AddKeyBinding(
                    new List<KeyCode>() { keys[gameButtons.Count] }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys)
                    {
                        int potentialIndex = (currPlayer.GetActivebuttonIndex() - 1);
                        int activeIndex = (potentialIndex < 0)? potentialIndex = (gameButtons.Count - 1) : potentialIndex;
                        currPlayer.SetActiveButtonIndex(activeIndex);
                        UpdateButtonColors();
                    }, false, false);
            inputManager.AddKeyBinding(
                    new List<KeyCode>() { keys[gameButtons.Count + 1] }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys)
                    {
                        int activeIndex = (currPlayer.GetActivebuttonIndex() + 1) % (gameButtons.Count);
                        currPlayer.SetActiveButtonIndex(activeIndex);
                        UpdateButtonColors();
                    }, false, false);
        }
        performanceMetrics.multiplayerButtonHits = 0;
        isGameplayStarted = true;


        inputManager.AddKeyBinding(
              new List<KeyCode>() { (KeyCode)(-1), (KeyCode)(-1) }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> pressedKeys)
              {
                  int playersPressingButtons = 0;
                  //pressedKeys.ForEach(i => Debug.Log(i));

                  foreach (Player player in settings.players)
                  {
                      List<KeyCode> playerKeys = player.GetMyKeys();
                      foreach (KeyCode key in pressedKeys)
                      {
                          if (playerKeys.Contains(key))
                          {
                              playersPressingButtons++;
                              break;
                          }
                      }
                  }

                  if (playersPressingButtons > 1)
                  {
                      performanceMetrics.multiplayerButtonHits++;
                  }

              }, false, false);

        inputManager.AddKeyBinding(
          new List<KeyCode>() { (KeyCode)(-1) }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> pressedKeys)
          {
              foreach (Player player in settings.players)
              {
                  if (player.GetMyKeys().Contains(pressedKeys[0]))
                  {
                      performanceMetrics.singlebuttonHits[player]++;
                  }
              }
          }, false, false);

        inputManager.AddKeyBinding(new List<KeyCode> { KeyCode.Space }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys) { gameSceneManager.StartAndPauseGame(); }, false, false);

        logManager = new MongoDBLogManager();
        logManager.InitLogs(this);

        Shuffle<ExercisesListWrapper>(settings.exercisesGroups);
        exerciseGroupIndex = UnityEngine.Random.Range(0, settings.exercisesGroups.Count);
    }

    // Use this for initialization
    void Start()
    {
        isGameplayStarted = false;
        StartCoroutine(YieldedStart());
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log("multi: " + performanceMetrics.multiplayerButtonHits);
        if (isGameplayPaused || !isGameplayStarted)
        {
            return;
        }

        ////if no lifes end game immediately
        //if (lifes < 1)
        //{
        //    gameSceneManager.EndGame();
        //}

        //if time's up change word
        if (timeLeft == 0.0f)
        {
            ChangeLevel();
            timeLeft = 100.0f;
        }

        //hpPanel.GetComponent<UnityEngine.UI.Text>().text = "Lifes: "+ lifes;
        //scorePanel.GetComponent<UnityEngine.UI.Text>().text = "Team Score: "+ score;
        timePanel.GetComponent<UnityEngine.UI.Text>().text = "Time: "+ timeLeft;


        Text playerPanelText = playersPanel.transform.GetComponentInChildren<Text>();
        playerPanelText.text = "";
        for (int i=0; i < settings.players.Count; i++)
        {
            playerPanelText.text += "\n Player "+ settings.players[i].GetName()+" Score: " + settings.players[i].score;
        }

        //update curr display message
        string displayString = "";
        foreach(Player player in settings.players)
        {
            Exercise currExercise = currExercises[player];
            string currWordState = currWordStates[player];
            int missingLength = currExercise.targetWord.Length - currWordState.Length;
            string[] substrings = currExercise.displayMessage.Split('_');

            displayString += player.GetName()+": \n";
            if (substrings.Length > 0)
            {
                displayString += substrings[0];
                displayString += currWordState;
                for (int i = 0; i < missingLength; i++)
                {
                    displayString += "_";
                }
                if (substrings.Length > 1)
                {
                    displayString += substrings[1];
                }
            }
            displayString += "\n";
        }

        reqPanel.GetComponent<Text>().text = displayString;
    }

    private void RecordMetrics()
    {
        //spawn questionnaires before changing word
        foreach (Player player in settings.players)
        {
            ////record metrics
            //int totalButtonHits = performanceMetrics.singlebuttonHits[player] + performanceMetrics.multiplayerButtonHits;
            //float playerHitsPercentage = ((float)performanceMetrics.singlebuttonHits[player] / (float)totalButtonHits) * 100.0f;
            //float simultaneousHitsPercentage = ((float)performanceMetrics.multiplayerButtonHits / (float)totalButtonHits) * 100.0f;
            ////StartCoroutine(logManager.WriteToLog("behavioralchangingcrossantlogs", "logs", new Dictionary<string, string>() {
            ////    { "playerName", player.GetName() },
            ////    { "currWord", currWordState },
            ////    { "playerCollPercentage", simultaneousHitsPercentage.ToString() },
            ////    { "playerCompPercentage", simultaneousHitsPercentage.ToString() },
            ////    { "playerSelfPercentage", playerHitsPercentage.ToString() }
            ////}));
            //StartCoroutine(logManager.WriteToLog("behavioralchangingcrossantlogs", "logs", new Dictionary<string, string>() {
            //    { "playerName", player.GetName() },
            //    { "currWord", currWordStates[player] },
            //    { "playerHitsPercentage", playerHitsPercentage.ToString() },
            //    { "simultaneousHitsPercentage", simultaneousHitsPercentage.ToString() }
            //}));
        }
        
    }

    private void PoppupQuestionnaires()
    {
        //gameSceneManager.PauseForQuestionnaires();
        ////spawn questionnaires before changing word
        //foreach (Player player in settings.players)
        //{
        //    Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSeM3Xn5qDBdX7QCtyrPILLbqpYj3ueDcLa_-9CbxCPzxVsMzg/viewform?usp=pp_url&entry.100873100=" + player.GetName()); //spawn questionaires
        //}
    }

    void ChangeLevel()
    {
        RecordMetrics();
        ChangeGameParametrizations(false);
    }

    void DecrementTimeLeft()
    {
        if(timeLeft > 0.0f){
            timeLeft--; 
        }
    }

    private void OnApplicationQuit()
    {
        //RecordMetrics();
        //PoppupQuestionnaires();
    }


    HashSet<KeyCode> GenerateKeyCombo(HashSet<KeyCode> possibleKeys, int numKeysInEachCombo)
    {
        HashSet<KeyCode> comboPossibleKeys = new HashSet<KeyCode>(possibleKeys);
        HashSet<KeyCode> generatedKeyCombo = new HashSet<KeyCode>();
        while (generatedKeyCombo.Count < numKeysInEachCombo)
        {
            int randomCodeIndex = UnityEngine.Random.Range(0, possibleKeys.Count);
            KeyCode currCode = possibleKeys.ElementAt(randomCodeIndex);

            comboPossibleKeys.Remove(currCode);
            generatedKeyCombo.Add(currCode);
        }
        return generatedKeyCombo;
    }

    private void ChangeGameParametrizations(bool firstTimeCall)
    {
        inputManager.InitKeys();
        
        int numKeysToPress = settings.maxSimultaneousKeyPresses;
        
        
        for(int i=0; i<letterSpawners.Length; i++)
        {
            if(!firstTimeCall && letterSpawners[i].minIntervalRange > 0.3 && letterSpawners[i].maxIntervalRange > 0.4)
            {
                letterSpawners[i].minIntervalRange -= 0.1f;
                letterSpawners[i].maxIntervalRange -= 0.1f;
            }
        }


        //change ants modes
        track.GetComponent<SpriteRenderer>().sprite = (Sprite) Resources.Load("Textures/track", typeof(Sprite));

        //string targetWord = this.currExercise.targetWord;

        for (int i = 0; i < letterSpawners.Length; i++)
        {
            letterSpawners[i].UpdateCurrStarredWord("");
        }
        //for (int i = 0; i < antSpawners.Length; i++)
        //{
        //    Utilities.OutputRestriction currOutputRestriction = antSpawners[i].GetComponent<AntSpawner>().outputRestriction;

        //    if (currOutputRestriction == Utilities.OutputRestriction.STARPOWER)
        //    {
        //        //change the track on star power
        //        track.GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("Textures/starTrack", typeof(Sprite));
        //        letterSpawners[UnityEngine.Random.Range(0,letterSpawners.Length)].UpdateCurrStarredWord(targetWord);
        //    }

        //    antSpawners[i].GetComponent<AntSpawner>().outputRestriction = ChooseOutputRestriction(); //Utilities.OutputRestriction.STARPOWER;
        //}

    }
    
    private void ChangeTargetWord(Player player)
    {
        List<Exercise> selectedExerciseGroup = settings.exercisesGroups[exerciseGroupIndex++].exercises;

        int random = UnityEngine.Random.Range(0, selectedExerciseGroup.Count);
        Exercise newExercise = selectedExerciseGroup[random];

        displayPanel.GetComponent<DisplayPanel>().SetTargetImage(newExercise.targetWord);

        this.currExercises[player] = newExercise;
        this.currWordStates[player] = "";
    }

    public void RecordHit(char letterText)
    {
        List<Player> currHitters = null;
        foreach (Player player in settings.players)
        {
            bool isHitter = player.GetMyKeys().Except(inputManager.GetCurrPressedKeys()).Any();
            if (!isHitter)
            {
                continue;
            }

            
            string currWordState = currWordStates[player];
            currWordState += letterText;
            string currTargetWord = currExercises[player].targetWord;


            //check the utility of words
            bool usefulForMe = (currWordState.Length <= currTargetWord.Length && currTargetWord[currWordState.Length - 1] == currWordState[currWordState.Length - 1]);
            bool usefulForOther = false;
            List<Player> whoIsUsefulFor = new List<Player>();
            foreach (Player innerPlayer in settings.players)
            {
                if(innerPlayer == player)
                {
                    continue;
                }
                string innerWordState = currWordStates[innerPlayer];
                innerWordState += letterText;
                string innerTargetState = currExercises[innerPlayer].targetWord;

                usefulForOther = (innerWordState.Length <= innerTargetState.Length && innerTargetState[innerWordState.Length - 1] == innerWordState[innerWordState.Length - 1]);
                if (usefulForOther)
                {
                    whoIsUsefulFor.Add(innerPlayer);
                }
            }

            //diferent rewards in different utility conditions
            Globals.KeyInteractionType playerIT = player.GetActiveInteraction();
            List<ScoreValue> scores = new List<ScoreValue>();
            switch (playerIT)
            {
                case Globals.KeyInteractionType.GIVE:
                    foreach (Player usefulTargetPlayer in whoIsUsefulFor)
                    {
                        string innerWordState = currWordStates[usefulTargetPlayer];
                        string innerTargetState = currExercises[usefulTargetPlayer].targetWord;
                        innerWordState = innerTargetState.Remove(innerWordState.Length - 1);
                        currWordStates[usefulTargetPlayer] += letterText;
                    }
                    scores = settings.scoreSystem.giveScores;
                    break;
                case Globals.KeyInteractionType.TAKE:
                    if (usefulForMe)
                    {
                        currWordState = currTargetWord.Remove(currWordState.Length - 1);
                        currWordStates[player] += letterText;
                    }
                    scores = settings.scoreSystem.takeScores;
                    break;
            }
            foreach (ScoreValue score in scores)
            {
                if (score.usefulForMe == usefulForMe && score.usefulForOther == usefulForOther)
                {
                    player.score += score.myValue;
                    foreach (Player innerPlayer in settings.players)
                    {
                        if (innerPlayer == player)
                        {
                            continue;
                        }
                        player.score += score.otherValue;
                    }
                    break;
                }
            }



            if (currWordState.CompareTo(currTargetWord) == 0)
            {
                foreach (AntSpawner antSpawner in antSpawners)
                {
                    antSpawner.SpawnAnt(currTargetWord);
                }
                ChangeLevel();
            }

        }
    }

    public List<Player> GetPlayers()
    {
        return settings.players;
    }
}


