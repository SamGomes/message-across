using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
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


    public enum DiffLetters
    {
        HIGHER,
        EQUAL,
        LOWER
    }

    public static KeyInteractionType[] keyInteractionTypes = (KeyInteractionType[]) Enum.GetValues(typeof(Globals.KeyInteractionType));
    public static ButtonId[] buttonIds = (ButtonId[]) Enum.GetValues(typeof(Globals.ButtonId));
    
    public static int currLevelId = 0;
    public static string gameId = "";


    public static IEnumerator LerpAnimation(GameObject source, Vector3 targetPos, float speed)
    {
        Vector3 sourcePos = source.transform.position;
        float totalDist = (targetPos - sourcePos).sqrMagnitude;
        float currT = 0;
        while (currT < 1.0f && source != null)
        {
            source.transform.position = Vector3.Lerp(sourcePos, targetPos, currT += speed * 0.025f);
            yield return new WaitForSeconds(0.025f);
        }
    }
}

[Serializable]
public class ScoreValue
{
    public bool usefulForMe;
    public bool usefulForOther;

    public Globals.DiffLetters diffLetters;

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
public struct GameSettings
{
    public List<ExercisesListWrapper> exercisesGroups;
    public List<Player> players;
    public int gameId;
    //public int initialTimeLeft;

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
    private AudioManager globalAudioManager;
    private int exerciseGroupIndex;

    public GameSettings settings;
    private PerformanceMetrics performanceMetrics;

    public GameObject canvas;

    public GameSceneManager gameSceneManager;

    public GameObject playerMarkersContainer;
    public GameObject playerMarkerPrefab;
    public GameObject playerUIPrefab;
    

    public bool isGameplayPaused;
    public bool isGameplayStarted;

    public bool isButtonOverlap;

    public GameObject namesInputLocation;
    public GameObject playerNameInputFieldPrefabRef;
    
    public GameObject wordPanelsObject;
    public GameObject scorePanelsObject;

    public GameObject timePanel;
    
    public GameObject emoji;

    public InputManager inputManager;
    public LogManager logManager;
    
    public LetterSpawner[] letterSpawners;

    public List<GameObject> pointerPlaceholders;
    public List<GameObject> playerUIs;
    
    private float timeLeft;


    public void PauseGame()
    {
        foreach(LetterSpawner ls in letterSpawners)
        {
            foreach(Letter letter in ls.GetComponentsInChildren<Letter>())
            {
                letter.isTranslationEnabled = false;
            }
            ls.enabled = false;
        }

        
        isGameplayPaused = true;
    }

    public void ResumeGame()
    {
        foreach (LetterSpawner ls in letterSpawners)
        {
            foreach (Letter letter in ls.GetComponentsInChildren<Letter>())
            {
                letter.isTranslationEnabled = true;
            }
            ls.enabled = true;
        }
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


    private void UpdateButtonOverlaps(Player currPlayer, int potentialIndex)
    {
        isButtonOverlap = false;
        foreach (Player player in settings.players)
        {
            if (player != currPlayer && player.GetActivebuttonIndex() == potentialIndex)
            {
                isButtonOverlap = true;
                break;
            }
        }

        foreach (Player player in settings.players)
        {
            foreach(GameObject obj in player.GetMaskedHalf())
            {
                obj.SetActive(!isButtonOverlap);
            }
        }
    }



    private IEnumerator YieldedStart()
    {
        for(int i=0; i < 20; i++)
        {
            Globals.gameId += (char)('A' + UnityEngine.Random.Range(0, 26));
        }

        globalAudioManager = new AudioManager();

        logManager = new MongoDBLogManager();
        logManager.InitLogs(this);
        
        isGameplayPaused = false;
        gameSceneManager.MainSceneLoadedNotification();
        
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

        //timeLeft = settings.initialTimeLeft;
        //timePanel.GetComponent<Animator>().StopPlayback();

        //InvokeRepeating("TimeDependentEvents", 0.0f, 1.0f);


        gameSceneManager.StartAndPauseGame(); //for the initial screen


        performanceMetrics = new PerformanceMetrics();
        performanceMetrics.singlebuttonHits = new Dictionary<Player, int>();
       

        for (int i = 0; i < settings.players.Count; i++)
        {
            Player currPlayer = settings.players[i];
            
            currPlayer.Init(this, playerMarkerPrefab, playerMarkersContainer, wordPanelsObject.transform.GetChild(i).gameObject, scorePanelsObject.transform.GetChild(i).gameObject, (i%2==0));

            currPlayer.GetWordPanel().transform.Find("panel/Layout").GetComponent<SpriteRenderer>().color = currPlayer.GetButtonColor();


            //set buttons for touch screen
            GameObject playerUI = playerUIs[i];
            playerUI.GetComponentInChildren<Image>().color = currPlayer.GetButtonColor(); //set background color (background as first child)
            UnityEngine.UI.Button[] playerButtons = playerUI.GetComponentsInChildren<UnityEngine.UI.Button>();
            
            for (int buttonI = 0; buttonI < playerButtons.Length; buttonI++)
            {
                UnityEngine.UI.Button currButton = playerButtons[buttonI];
                if (buttonI < pointerPlaceholders.Count)
                {
                    int innerButtonI = buttonI; //for corotine to save the iterated values
                    currButton.onClick.AddListener(delegate()
                    {
                        currPlayer.SetActiveButton(innerButtonI, pointerPlaceholders[innerButtonI].transform.position);
                        UpdateButtonOverlaps(currPlayer, innerButtonI);
                    });
                }
                else
                {
                    int j = buttonI - pointerPlaceholders.Count + 1;
                    Globals.KeyInteractionType iType = (Globals.KeyInteractionType) j;
                    EventTrigger trigger = currButton.gameObject.AddComponent<EventTrigger>();
                    EventTrigger.Entry pointerDown = new EventTrigger.Entry();
                    pointerDown.eventID = EventTriggerType.PointerDown;
                    pointerDown.callback.AddListener(delegate (BaseEventData eventData)
                    {
                        currPlayer.SetActiveInteraction(iType);
                        //int activeIndex = currPlayer.GetActivebuttonIndex();
                        currPlayer.GetGameButton().RegisterButtonDown();
                        if (iType == Globals.KeyInteractionType.GIVE) currPlayer.numGivePresses++;
                        else if (iType == Globals.KeyInteractionType.TAKE) currPlayer.numTakePresses++;
                    });
                    trigger.triggers.Add(pointerDown);
                    EventTrigger.Entry pointerUp = new EventTrigger.Entry();
                    pointerUp.eventID = EventTriggerType.PointerUp;
                    pointerUp.callback.AddListener(delegate (BaseEventData eventData)
                    {
                        currPlayer.SetActiveInteraction(iType);
                        //int activeIndex = currPlayer.GetActivebuttonIndex();
                        currPlayer.GetGameButton().RegisterButtonUp();
                        if (iType == Globals.KeyInteractionType.GIVE) currPlayer.numGivePresses++;
                        else if (iType == Globals.KeyInteractionType.TAKE) currPlayer.numTakePresses++;
                    });
                    trigger.triggers.Add(pointerUp);
                }
                
            }

            

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
            
            isButtonOverlap = true;
            for (int j = 1; j < Globals.keyInteractionTypes.Length ; j++)
            {
                Globals.KeyInteractionType iType = (Globals.KeyInteractionType) j;
                inputManager.AddKeyBinding(
                    new List<KeyCode>(){ keys[j] }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys)
                    {
                        currPlayer.SetActiveInteraction(iType);
                        int activeIndex = currPlayer.GetActivebuttonIndex();
                        currPlayer.GetGameButton().RegisterButtonDown();
                        if (iType == Globals.KeyInteractionType.GIVE) currPlayer.numGivePresses++;
                        else if (iType == Globals.KeyInteractionType.TAKE) currPlayer.numTakePresses++;
                    }, false);
                inputManager.AddKeyBinding(
                    new List<KeyCode>() { keys[j] }, InputManager.ButtonPressType.UP, delegate (List<KeyCode> triggeredKeys)
                    {
                        currPlayer.SetActiveInteraction(iType);
                        int activeIndex = currPlayer.GetActivebuttonIndex();
                        currPlayer.GetGameButton().RegisterButtonUp();
                        if (iType == Globals.KeyInteractionType.GIVE) currPlayer.numGivePresses++;
                        else if (iType == Globals.KeyInteractionType.TAKE) currPlayer.numTakePresses++;
                    }, false);
            }
            inputManager.AddKeyBinding(
                    new List<KeyCode>() { keys[pointerPlaceholders.Count] }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys)
                    {
                        int potentialIndex = (currPlayer.GetActivebuttonIndex() - 1);
                        potentialIndex = (potentialIndex < 0)? 0 : potentialIndex;
                        currPlayer.SetActiveButton(potentialIndex, pointerPlaceholders[potentialIndex].transform.position);
                        UpdateButtonOverlaps(currPlayer, potentialIndex);
                    }, false);
            inputManager.AddKeyBinding(
                    new List<KeyCode>() { keys[pointerPlaceholders.Count + 1] }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys)
                    {
                        int potentialIndex = (currPlayer.GetActivebuttonIndex() + 1);
                        potentialIndex = (potentialIndex > (pointerPlaceholders.Count - 1)) ? (pointerPlaceholders.Count - 1) : potentialIndex;
                        currPlayer.SetActiveButton(potentialIndex, pointerPlaceholders[potentialIndex].transform.position);
                        UpdateButtonOverlaps(currPlayer, potentialIndex);
                    }, false);

            currPlayer.SetActiveButton(0, pointerPlaceholders[0].transform.position);
            currPlayer.SetScore(0);
        }

        UpdateButtonOverlaps(settings.players[0], 0);
        ChangeGameParametrizations(true);
        
        performanceMetrics.multiplayerButtonHits = 0;
        isGameplayStarted = true;

        exerciseGroupIndex = UnityEngine.Random.Range(0, settings.exercisesGroups.Count);
        ChangeGameParametrizations(true);

        //inputManager.AddKeyBinding(new List<KeyCode> { KeyCode.Space }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys) {
            globalAudioManager.PlayClip("Audio/wordChangeBad");
            gameSceneManager.StartAndPauseGame();
        //}, false);


        Shuffle<ExercisesListWrapper>(settings.exercisesGroups);


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
        if (isGameplayPaused || !isGameplayStarted)
        {
            return;
        }
        
    }

    private void RecordMetrics()
    {
        //spawn questionnaires before changing word
        foreach (Player player in settings.players)
        {
            StartCoroutine(logManager.WriteToLog("behavioralchangingcrossantlogs", "logs", new Dictionary<string, string>() {
                { "gameId", Globals.gameId.ToString() },
                { "name", player.GetName() },
                { "color", player.GetButtonColor().ToString() },
                { "currWord", player.GetCurrExercise().targetWord },
                { "score", player.GetScore().ToString() },
                { "numberOfGivePresses", player.numGivePresses.ToString() },
                { "numberOfTakePresses", player.numTakePresses.ToString() }
            }));
        }
        
    }

    void ChangeLevel(bool areWordsUnfinished)
    {
        if (areWordsUnfinished)
        {
            globalAudioManager.PlayClip("Audio/wordChangeBad");
            emoji.GetComponent<Animator>().Play("Sad");
        }
        else
        {

            globalAudioManager.PlayClip("Audio/wordChangeGood");
            emoji.GetComponent<Animator>().Play("Smiling");
        }
        //timeLeft = settings.initialTimeLeft;

        foreach(Player player in settings.players)
        {
            player.ResetNumPossibleActions();
        }

        RecordMetrics();
        ChangeGameParametrizations(false);
    }
    

    private void ChangeGameParametrizations(bool firstTimeCall)
    {
        for(int i=0; i<letterSpawners.Length; i++)
        {
            if(!firstTimeCall && letterSpawners[i].minIntervalRange > 0.3 && letterSpawners[i].maxIntervalRange > 0.4)
            {
                letterSpawners[i].minIntervalRange -= 0.1f;
                letterSpawners[i].maxIntervalRange -= 0.1f;
            }
        }

        for (int i = 0; i < letterSpawners.Length; i++)
        {
            letterSpawners[i].UpdateCurrStarredWord("");
        }

        ChangeTargetWords();

    }
    
    private void ChangeTargetWords()
    {
        List<Exercise> selectedExerciseGroup = new List<Exercise>(settings.exercisesGroups[exerciseGroupIndex++ % settings.exercisesGroups.Count].exercises);
        foreach (Player player in settings.players)
        {
            if(selectedExerciseGroup.Count <= 0)
            {
                selectedExerciseGroup = new List<Exercise>(settings.exercisesGroups[exerciseGroupIndex++ % settings.exercisesGroups.Count].exercises);
            }

            int random = UnityEngine.Random.Range(0, selectedExerciseGroup.Count);
            Exercise newExercise = selectedExerciseGroup[random];
            selectedExerciseGroup.RemoveAt(random);
            
            
            //displayPanel.GetComponent<DisplayPanel>().SetTargetImage(newExercise.targetWord);
            player.SetCurrExercise(newExercise);
            player.InitCurrWordState();

            //animate transition
            player.GetWordPanel().GetComponentInChildren<Animator>().Play(0);
        }
    }


    private bool TestAndExecuteHit(bool execute, char letterText, GameObject letter, Player player)
    {
        string currWordState = player.GetCurrWordState();
        string currTargetWord = player.GetCurrExercise().targetWord;

        //check the utility of word
        bool usefulForMe = (currWordState.Length <= currTargetWord.Length && currTargetWord.Contains(letterText));


        if (execute && usefulForMe)
        {
            int changeIndex = currTargetWord.IndexOf(letterText);
            while(true)
            {
                if (changeIndex == -1)
                {
                    return false;
                }else if(currWordState[changeIndex] == '_')
                {
                    break;
                }
                changeIndex = currTargetWord.IndexOf(letterText, changeIndex+1);
            }
            
            StringBuilder sb = new StringBuilder(currWordState);
            sb[changeIndex] = letterText;
            currWordState = sb.ToString();
            
            player.SetCurrWordState(currWordState);
            letter.GetComponent<Letter>().isTranslationEnabled = false;
            StartCoroutine(AnimateLetter(letter,player));
        }

        return usefulForMe;
    }

    private IEnumerator AnimateLetter(GameObject letter, Player player)
    {
        yield return Globals.LerpAnimation(letter, player.GetWordPanel().transform.position, 1.0f);
        UnityEngine.Object.Destroy(letter);
    }

    public void RecordHit(char letterText, GameObject letter, Player currHitter)
    {
        
        if (currHitter.GetCurrNumPossibleActionsPerLevel() <= 0)
        {
            return;
        }

        currHitter.DecreasePossibleActionsPerLevel();

        bool usefulForMe = false;
        bool usefulForOther = false;

        //diferent rewards in different utility conditions
        Globals.KeyInteractionType playerIT = currHitter.GetActiveInteraction();
        List<ScoreValue> scores = new List<ScoreValue>();
        switch (playerIT)
        {
            case Globals.KeyInteractionType.GIVE:
                usefulForMe = TestAndExecuteHit(false, letterText, letter, currHitter);
                foreach (Player usefulTargetPlayer in settings.players)
                {
                    if (usefulTargetPlayer == currHitter)
                    {
                        continue;
                    }
                    usefulForOther = TestAndExecuteHit(true, letterText, letter, usefulTargetPlayer);
                    if (usefulForOther)
                    {
                        //letter.GetComponentInChildren<SpriteRenderer>().color = player.GetButtonColor();
                        break;
                    }

                }
                scores = settings.scoreSystem.giveScores;
                break;
            case Globals.KeyInteractionType.TAKE:
                usefulForMe = TestAndExecuteHit(true, letterText, letter, currHitter);
                if (usefulForMe)
                {
                    //letter.GetComponentInChildren<SpriteRenderer>().color = player.GetButtonColor();
                }
                foreach (Player usefulTargetPlayer in settings.players)
                {
                    if (usefulTargetPlayer == currHitter)
                    {
                        continue;
                    }
                    if (usefulForOther)
                    {
                        break;
                    }

                    usefulForOther = TestAndExecuteHit(false, letterText, letter, usefulTargetPlayer);
                }
                scores = settings.scoreSystem.takeScores;
                break;
        }

        if (usefulForMe || usefulForOther)
        {
            emoji.GetComponent<Animator>().Play("Nice");
        }

        float otherPlayersCompletionMean = 0;
        int otherPlayersCount = settings.players.Count() - 1;
        float currPlayerCompletion = currHitter.GetCurrWordState().Count();
        foreach (Player innerPlayer in settings.players)
        {
            if (innerPlayer == currHitter)
            {
                continue;
            }
            otherPlayersCompletionMean += (float) innerPlayer.GetCurrWordState().Count() / otherPlayersCount;
        }
        Globals.DiffLetters playerDiff = (currPlayerCompletion > otherPlayersCompletionMean) ? Globals.DiffLetters.HIGHER : (currPlayerCompletion == otherPlayersCompletionMean) ? Globals.DiffLetters.EQUAL : Globals.DiffLetters.LOWER;

        bool scoreOptionFound = false;
        foreach (ScoreValue score in scores)
        {
            if (score.usefulForMe == usefulForMe && score.usefulForOther == usefulForOther && playerDiff== score.diffLetters)
            {
                currHitter.SetScore(currHitter.GetScore() + score.myValue);
                foreach (Player innerPlayer in settings.players)
                {
                    if (innerPlayer == currHitter)
                    {
                        continue;
                    }
                    innerPlayer.SetScore(innerPlayer.GetScore() + score.otherValue);
                }
                scoreOptionFound = true;
                break;
            }
        }

        if (!scoreOptionFound)
        {
            Debug.Log("could not find score option for <usefulForMe: " + usefulForMe + ", usefulForOther: " + usefulForOther + ", playerDiff: " + playerDiff + ">");
        }


        bool areWordsUnfinished = false;
        bool arePlayersWithoutActions = true;
        foreach (Player player in settings.players)
        {
            if (player.GetCurrNumPossibleActionsPerLevel() > 0)
            {
                arePlayersWithoutActions = false;
            }

            string currWordState = player.GetCurrWordState();
            string currTargetWord = player.GetCurrExercise().targetWord;
            if (currWordState.CompareTo(currTargetWord) != 0)
            {
                if(!areWordsUnfinished)
                    areWordsUnfinished = true;

                if (!player.isCurrExerciseFinished)
                    player.isCurrExerciseFinished = true;
            }
            else
            {
                if (!player.isCurrExerciseFinished)
                    player.SetScore(player.GetScore() + settings.scoreSystem.completeWordMyScore);
                foreach (Player innerPlayer in settings.players)
                {
                    if (player == innerPlayer)
                    {
                        continue;
                    }
                    if (!innerPlayer.isCurrExerciseFinished)
                        innerPlayer.SetScore(innerPlayer.GetScore() + settings.scoreSystem.completeWordOtherScore);
                }
            }
        }
        if (!areWordsUnfinished || arePlayersWithoutActions)
        {
            ChangeLevel(areWordsUnfinished);
        }
        
    }

    public List<Player> GetPlayers()
    {
        return settings.players;
    }
}


