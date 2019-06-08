using System;
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

    public enum PlayersToPressButtonAlternative
    {
        SINGLE_PLAYER,
        MULTIPLAYER
    }

    public enum InteractionType
    {
        NONE,
        COOPERATION,
        COMPETITION,
        SELF_IMPROVEMENT
    }

    public static PlayersToPressButtonAlternative[] playersToPressButtonAlternatives = (PlayersToPressButtonAlternative[]) Enum.GetValues(typeof(Globals.PlayersToPressButtonAlternative));
    public static InteractionType[] interactionTypes = (InteractionType[]) Enum.GetValues(typeof(Globals.InteractionType));

    public static ButtonId[] buttonIds = (ButtonId[]) Enum.GetValues(typeof(Globals.ButtonId));
    

    public static int numPlayersToPressButtonAlternatives = playersToPressButtonAlternatives.Length;

    public static int currLevelId = 0;
}

[Serializable]
public struct GameSettings
{
    public int maxSimultaneousKeyPresses;
    public List<Exercise> exercises;
    public List<Player> players;
    public int gameId;
}

[Serializable]
public struct PerformanceMetrics
{
    public Dictionary<Player,int> singlebuttonHits;
    public int multiplayerButtonHits;
}

public class GameManager : MonoBehaviour
{

    private GameSettings settings;
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


    public GameObject playersPanel;
    public AntSpawner[] antSpawners;
    public LetterSpawner[] letterSpawners;

    public List<Button> gameButtons;

    public string currWordState;

    public float timeLeft;

    public int lifes;

    public Globals.PlayersToPressButtonAlternative currNumPlayersCombo;
    public Globals.InteractionType currRewardType;

    public Exercise currExercise { get; set; }

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

    void UpdateButtonColors()
    {
        gameButtons.ForEach(delegate (Button button) { button.GetComponent<SpriteRenderer>().color = Color.white; });
        for(int i=0; i< gameButtons.Count; i++)
        {
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
                gameButtons[i].GetComponent<SpriteRenderer>().color = new Color(1.0f,0.5f,0.0f,1.0f);
            }
        }
    }


    IEnumerator YieldedStart()
    {
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

        //string json = JsonUtility.ToJson(settings,true);
        settings = JsonUtility.FromJson<GameSettings>(configText);

        lifes = 50;

        //Application.targetFrameRate = 60;

        timeLeft = 100.0f;
        InvokeRepeating("DecrementTimeLeft", 0.0f, 1.0f);

        ChangeTargetWord();
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


            input.onValueChanged.AddListener(delegate
            {
                List<InputField> inputFields = new List<InputField>(namesInputLocation.GetComponentsInChildren<InputField>());
                settings.players[inputFields.IndexOf(input)].SetName(input.text);
            });

            performanceMetrics.singlebuttonHits.Add(currPlayer, 0);

            List<KeyCode> keys = currPlayer.GetMyKeys();

            UpdateButtonColors();

            for (int j = 0; j < keys.Count-1; j++)
            {
                inputManager.AddKeyBinding(
                    new List<KeyCode>(){ keys[j] }, InputManager.ButtonPressType.PRESSED, delegate (List<KeyCode> triggeredKeys)
                    {
                        int activeIndex = currPlayer.GetActivebuttonIndex();
                        gameButtons[activeIndex].RegisterButtonPress();//Utilities.interactionTypes[j]);
                    }, false);
            }
            inputManager.AddKeyBinding(
                    new List<KeyCode>() { keys[keys.Count-1] }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys)
                    {
                        int activeIndex = (currPlayer.GetActivebuttonIndex() + 1) % (gameButtons.Count);
                        currPlayer.SetActiveButtonIndex(activeIndex);
                        UpdateButtonColors();
                    }, false);
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

              }, false);

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
          }, false);

        inputManager.AddKeyBinding(new List<KeyCode> { KeyCode.Space }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys) { gameSceneManager.StartAndPauseGame(); }, false);

        LogManager logManager = new MongoDBLogManager();
        logManager.InitLogs(this);
        logManager.WriteToLog("behavioralchangingcrossantlogs", "logs", new Dictionary<string, string>() { { "a", "0" }, { "b", "1" } });
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

        //if no lifes end game immediately
        if (lifes < 1)
        {
            gameSceneManager.EndGame();
        }

        //if time's up change word
        if (timeLeft == 0.0f)
        {
            ChangeLevel();
            timeLeft = 100.0f;
            Hurt();
        }

        hpPanel.GetComponent<UnityEngine.UI.Text>().text = "Lifes: "+ lifes;
        //scorePanel.GetComponent<UnityEngine.UI.Text>().text = "Team Score: "+ score;
        timePanel.GetComponent<UnityEngine.UI.Text>().text = "Time: "+ timeLeft;


        UnityEngine.UI.Text playerPanelText = playersPanel.transform.GetComponentInChildren<UnityEngine.UI.Text>();
        playerPanelText.text = "";
        for (int i=0; i < settings.players.Count; i++)
        {
            playerPanelText.text += "\n Player "+ settings.players[i].GetName()+" Score: " + settings.players[i].score;
        }

        //update curr display message
        int missingLength = this.currExercise.targetWord.Length - currWordState.Length;
        string[] substrings = this.currExercise.displayMessage.Split('_');

        string displayString = "";
        if (substrings.Length > 0)
        {
            displayString = substrings[0];
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
        reqPanel.GetComponent<ReqScript>().UpdateRequirement(displayString);

    }



    void ChangeLevel()
    {
        PoppupQuestionnaires();
        ChangeTargetWord();
        ChangeGameParametrizations(false);
    }

    void DecrementTimeLeft()
    {
        if(timeLeft > 0.0f){
            timeLeft--; 
        }
    }

    void PoppupQuestionnaires()
    {
        gameSceneManager.PauseForQuestionnaires();
        //spawn questionnaires before changing word
        foreach (Player player in settings.players)
        {
            int totalButtonHits = performanceMetrics.singlebuttonHits[player] + performanceMetrics.multiplayerButtonHits;
            float playerHitsPercentage = ((float)performanceMetrics.singlebuttonHits[player] / (float)totalButtonHits) * 100.0f;
            float simultaneousHitsPercentage = ((float)performanceMetrics.multiplayerButtonHits / (float)totalButtonHits) * 100.0f;
            Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSeM3Xn5qDBdX7QCtyrPILLbqpYj3ueDcLa_-9CbxCPzxVsMzg/viewform?usp=pp_url&entry.100873100=" + player.GetName() + "&entry.631185473=" + currWordState + "&entry.159491668=" + currNumPlayersCombo + "&entry.1252688229=" + playerHitsPercentage + "&entry.1140424083=" + simultaneousHitsPercentage); //spawn questionaires
        }
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

    void ChangeGameParametrizations(bool firstTimeCall)
    {
        inputManager.InitKeys();
        
        int numKeysToPress = settings.maxSimultaneousKeyPresses;
        
        //choose num players combo
        int randomAltIndex = UnityEngine.Random.Range(0, Globals.numPlayersToPressButtonAlternatives);
        Globals.PlayersToPressButtonAlternative chosenPAAlternative = Globals.playersToPressButtonAlternatives[randomAltIndex];
        randomAltIndex = UnityEngine.Random.Range(0, Globals.numPlayersToPressButtonAlternatives);
        Globals.InteractionType chosenRTAlternative = Globals.interactionTypes[randomAltIndex];

        
        this.currNumPlayersCombo = firstTimeCall ? Globals.PlayersToPressButtonAlternative.MULTIPLAYER : chosenPAAlternative;
        this.currRewardType = chosenRTAlternative;

        //foreach (Button button in this.gameButtons)
        //{
        //    if (this.currNumPlayersCombo == Utilities.PlayersToPressButtonAlternative.SINGLE_PLAYER)
        //    {
        //        foreach (Player player in settings.players)
        //        {
        //            List<KeyCode> generatedKeyCombo = GenerateKeyCombo(new HashSet<KeyCode>(player.GetMyKeys()), numKeysToPress).ToList();
        //            while (generatedKeyCombo != null && inputManager.ContainsKeyBinding(generatedKeyCombo)){
        //                generatedKeyCombo = GenerateKeyCombo(new HashSet<KeyCode>(player.GetMyKeys()), numKeysToPress).ToList();
        //            }
        //            inputManager.AddKeyBinding(
        //                generatedKeyCombo, InputManager.ButtonPressType.PRESSED, delegate (List<KeyCode> triggeredKeys)
        //                {
        //                    gameButtons[(int)button.buttonCode].RegisterButtonPress();
        //                }, false);
        //        }
        //    }
        //    else if (this.currNumPlayersCombo == Utilities.PlayersToPressButtonAlternative.MULTIPLAYER)
        //    {
        //        List<Player> selectedKeysPlayers = new List<Player>();
        //        HashSet<KeyCode> possibleKeys = new HashSet<KeyCode>();
        //        for (int i = 0; i < numKeysToPress; i++)
        //        {
        //            Player selectedPlayer = settings.players[i % settings.players.Count];
        //            selectedKeysPlayers.Add(selectedPlayer);

        //            int randomIndex = UnityEngine.Random.Range(0, selectedPlayer.GetMyKeys().Count);
        //            possibleKeys.Add(selectedPlayer.GetMyKeys().ElementAt(randomIndex));
        //        }

        //        List<KeyCode> generatedKeyCombo = GenerateKeyCombo(possibleKeys, numKeysToPress).ToList();
        //        inputManager.AddKeyBinding(
        //            generatedKeyCombo, InputManager.ButtonPressType.PRESSED, delegate (List<KeyCode> triggeredKeys)
        //            {
        //                foreach (Player selectedPlayer in selectedKeysPlayers)
        //                {
        //                    gameButtons[(int)button.buttonCode].RegisterButtonPress();
        //                }
        //            }, false);
        //    }



            //HashSet<KeyCode> generatedKeyCombo = new HashSet<KeyCode>();

            //int randomPlayerIndex = UnityEngine.Random.Range(0, players.Count);
            //Player selectedPlayer = player;

            //possibleKeys.UnionWith(selectedPlayer.GetMyKeys());
            //while (generatedKeyCombo.Count < numKeysToPress)
            //{

            //    int randomCodeIndex = UnityEngine.Random.Range(0, possibleKeys.Count);
            //    KeyCode currCode = possibleKeys.ElementAt(randomCodeIndex);
            //    //possibleKeys.RemoveAt(randomIndex);

            //    generatedKeyCombo.Add(currCode);
            //    selectedKeysPlayers.Add(selectedPlayer);
            //}
            //buttonKeyCombos.Add(generatedKeyCombo);
            //inputManager.AddKeyBinding(
            //    generatedKeyCombo, InputManager.ButtonPressType.PRESSED, delegate () {
            //        gameButtons[(int)button.buttonCode].RegisterUserButtonPress(selectedKeysPlayers);
            //});

        //}


        for(int i=0; i<letterSpawners.Length; i++)
        {
            if(!firstTimeCall && letterSpawners[i].minIntervalRange > 0.3 && letterSpawners[i].maxIntervalRange > 0.4)
            {
                letterSpawners[i].minIntervalRange -= 0.1f;
                letterSpawners[i].maxIntervalRange -= 0.1f;
            }
        }


        //change ants modes
        track.GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("Textures/track", typeof(Sprite));

        string targetWord = this.currExercise.targetWord;

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

    void ChangeTargetWord()
    {
        int random = UnityEngine.Random.Range(0, settings.exercises.Count);
        Exercise newExercise = settings.exercises[random];

        currWordState = "";
        displayPanel.GetComponent<DisplayPanel>().SetTargetImage(newExercise.targetWord);

        this.currExercise = newExercise;
    }

    void Hurt()
    {
        lifes--;
    }

    public void RecordHit(char letterText)
    {
        this.currWordState += letterText;
        this.currWordState = this.currWordState.ToUpper();
        string currTargetWord = this.currExercise.targetWord;

        if (currWordState.Length <= currTargetWord.Length && currTargetWord[currWordState.Length - 1] == currWordState[currWordState.Length - 1])
        {
            //diferent rewards in different reward conditions
            Player currHitter = null;
            foreach (Player player in settings.players)
            {
                if (player.GetMyKeys().Contains(inputManager.GetLastPressedKey()))
                {
                    currHitter = player;
                }
            }
            switch (currRewardType)
            {
                case Globals.InteractionType.COOPERATION:
                    foreach (Player player in settings.players)
                    {
                        player.score += 50;
                    }
                    break;
                case Globals.InteractionType.COMPETITION:
                    foreach (Player player in settings.players)
                    {
                        if (currHitter == player)
                        {
                            currHitter.score += 100;
                        }
                        else
                        {
                            player.score -= 50;
                        }
                    }
                    break;
                case Globals.InteractionType.SELF_IMPROVEMENT:
                    currHitter.score += 50;
                    break;
            }

        }
        else
        {
            Hurt();
            currWordState = currWordState.Remove(currWordState.Length - 1);
            return;
        }
        

        if (currWordState.CompareTo(currTargetWord) == 0)
        {
            timeLeft += currTargetWord.Length*4;
            timeLeft = 100.0f;

            //init track and play ant anims
            GameObject[] letters = GameObject.FindGameObjectsWithTag("letter");
            foreach (GameObject letter in letters)
            {
                Destroy(letter);
            }
            //foreach (LetterSpawner letterSpawner in letterSpawners)
            //{
            //    letterSpawner.SetScore(score);
            //}
            foreach (AntSpawner antSpawner in antSpawners)
            {
                antSpawner.SpawnAnt(currTargetWord);
            }

            ChangeLevel();

        }
    }

    
    //private Utilities.OutputRestriction ChooseOutputRestriction()
    //{
    //    int randomIndex = UnityEngine.Random.Range(0, Utilities.numOutputRestriction);
    //    return Utilities.outputRestrictions[randomIndex];
    //}


    public List<Player> GetPlayers()
    {
        return settings.players;
    }


}


