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
    public int gameId;
    public int numLevels;
}

[Serializable]
public struct GameSettings
{
    public ExerciseGroupsWrapper exercisesGroups;
    public GeneralSettings generalSettings;
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
    public Button quitButton;
    public Button resetButton;
    private int numLevelsLeft;

    private int exerciseGroupIndex;

    public GameSettings settings;
    private PerformanceMetrics performanceMetrics;

    public GameObject canvas;
    public GameObject stateCanvas;

    public Text countdownText;


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

    public LogManager logManager;
    
    public LetterSpawner[] letterSpawners;

    public List<GameObject> pointerPlaceholders;
    public List<GameObject> playerUIs;
    
    private float timeLeft;

    private string scoreSystemName; //to be able to recover condition

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
        foreach (Player player in settings.generalSettings.players)
        {
            if (player != currPlayer && player.GetActivebuttonIndex() == potentialIndex)
            {
                isButtonOverlap = true;
                break;
            }
        }

        foreach (Player player in settings.generalSettings.players)
        {
            foreach(GameObject obj in player.GetMaskedHalf())
            {
                obj.SetActive(!isButtonOverlap);
            }
            if(!isButtonOverlap)
            {
                player.GetActiveHalf().AddRange(player.GetMaskedHalf());
            }
            //else
            //{
            //    player.SetActiveHalf(player.GetActiveHalf().Except(player.GetMaskedHalf()).ToList());
            //}
            player.UpdateActiveHalf(false);
        }
    }



    private IEnumerator YieldedStart()
    {
        quitButton.onClick.AddListener(delegate(){
            Globals.gameSceneManager.EndGame();
        });
        resetButton.onClick.AddListener(delegate () {
            Globals.gameSceneManager.EndGame();
            Globals.gameSceneManager.StartGame();
        });

        for (int i=0; i < 20; i++)
        {
            Globals.gameId += (char)('A' + UnityEngine.Random.Range(0, 26));
        }
        
        logManager = new DebugLogManager();
        logManager.InitLogs(this);
        
        isGameplayPaused = false;
        
        string generalConfigPath = Application.streamingAssetsPath + "/generalConfig.cfg";
        switch (Globals.gameParam)
        {
            case Globals.ExercisesConfig.NEUTRAL: //special condition also removes the score
                scorePanelsObject.transform.GetChild(0).GetComponentInChildren<Text>().gameObject.SetActive(false);
                scorePanelsObject.transform.GetChild(1).GetComponentInChildren<Text>().gameObject.SetActive(false);
                scoreSystemName = "scoreSystemConfigNeutral";
                break;
            case Globals.ExercisesConfig.COMPETITIVE:
                scoreSystemName = "scoreSystemConfigComp";
                break;
            case Globals.ExercisesConfig.INDIVIDUALISTIC:
                scoreSystemName = "scoreSystemConfigIndividualistic";
                break;
            case Globals.ExercisesConfig.MUTUAL_HELP:
                scoreSystemName = "scoreSystemConfigMutualHelp";
                break;
            case Globals.ExercisesConfig.P_ALTROISTIC:
                scoreSystemName = "scoreSystemConfigPAltruistic";
                break;
        }

        string scoreConfigPath = Application.streamingAssetsPath + "/"+ scoreSystemName + ".cfg";
        string exercisesConfigPath = Application.streamingAssetsPath + "/exercisesConfig.cfg";
        string generalConfigText = "";
        string scoreConfigText = "";
        string exercisesConfigText = "";
        if (generalConfigPath.Contains("://") || generalConfigPath.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(generalConfigPath);
            yield return www.SendWebRequest();
            generalConfigText = www.downloadHandler.text;
        }
        else
        {
            generalConfigText = File.ReadAllText(generalConfigPath);
        }

        if (scoreConfigPath.Contains("://") || scoreConfigPath.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(scoreConfigPath);
            yield return www.SendWebRequest();
            scoreConfigText = www.downloadHandler.text;
        }
        else
        {
            scoreConfigText = File.ReadAllText(scoreConfigPath);
        }

        if (exercisesConfigPath.Contains("://") || exercisesConfigPath.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(generalConfigPath);
            yield return www.SendWebRequest();
            exercisesConfigText = www.downloadHandler.text;
        }
        else
        {
            exercisesConfigText = File.ReadAllText(exercisesConfigPath);
        }

        //string json = JsonUtility.ToJson(settings, true);
        settings.generalSettings = JsonUtility.FromJson<GeneralSettings>(generalConfigText);
        settings.scoreSystem = JsonUtility.FromJson<ScoreSystem>(scoreConfigText);
        settings.exercisesGroups = JsonUtility.FromJson<ExerciseGroupsWrapper>(exercisesConfigText);
        

        numLevelsLeft = settings.generalSettings.numLevels;

        performanceMetrics = new PerformanceMetrics();
        performanceMetrics.singlebuttonHits = new Dictionary<Player, int>();

        
        DontDestroyOnLoad(stateCanvas);
        if (Globals.savedObjects == null)
        {
            Globals.InitGlobals();
        }
        Globals.savedObjects.Add(stateCanvas);


        for (int i = 0; i < settings.generalSettings.players.Count; i++)
        {
            GameObject playerUI = playerUIs[i];

            Player currPlayer = settings.generalSettings.players[i];
            currPlayer.Init(this, playerMarkerPrefab, playerMarkersContainer, playerUI, wordPanelsObject.transform.GetChild(i).gameObject, scorePanelsObject.transform.GetChild(i).gameObject, (i%2==0));
            currPlayer.GetWordPanel().transform.Find("panel/Layout").GetComponent<SpriteRenderer>().color = currPlayer.GetBackgroundColor();
            
            //set buttons for touch screen
            UnityEngine.UI.Button[] playerButtons = playerUI.GetComponentsInChildren<UnityEngine.UI.Button>();

            for (int buttonI = 0; buttonI < playerButtons.Length; buttonI++)
            {
                UnityEngine.UI.Button currButton = playerButtons[buttonI];
                if (buttonI < pointerPlaceholders.Count)
                {
                    currButton.GetComponent<Image>().color = currPlayer.GetButtonColor();
                    int innerButtonI = buttonI; //for corotine to save the iterated values
                    currButton.onClick.AddListener(delegate ()
                    {
                        //verify if button should be pressed
                        if (currPlayer.GetCurrNumPossibleActionsPerLevel() < 1)
                        {
                            return;
                        }
                        Globals.trackEffectsAudioManager.PlayClip("Audio/trackChange");
                        playerButtons[currPlayer.GetActivebuttonIndex()].GetComponent<Image>().color = currPlayer.GetButtonColor();
                        UpdateButtonOverlaps(currPlayer, innerButtonI);
                        currPlayer.SetActiveButton(innerButtonI, pointerPlaceholders[innerButtonI].transform.position);
                        currButton.GetComponent<Image>().color = new Color(1.0f, 0.8f, 0.6f);
                    });
                }
                else
                {
                    int j = buttonI - pointerPlaceholders.Count + 1;
                    Globals.KeyInteractionType iType = (Globals.KeyInteractionType)j;
                    EventTrigger trigger = currButton.gameObject.AddComponent<EventTrigger>();
                    EventTrigger.Entry pointerDown = new EventTrigger.Entry();
                    pointerDown.eventID = EventTriggerType.PointerDown;
                    pointerDown.callback.AddListener(delegate (BaseEventData eventData)
                    {
                        //verify if button should be pressed
                        if (currPlayer.GetCurrNumPossibleActionsPerLevel() < 1)
                        {
                            return;
                        }

                        Globals.trackEffectsAudioManager.PlayClip("Audio/clickDown");
                        currPlayer.SetActiveInteraction(iType);
                        if (iType == Globals.KeyInteractionType.GIVE) currPlayer.numGivePresses++;
                        else if (iType == Globals.KeyInteractionType.TAKE) currPlayer.numTakePresses++;

                        bool playerOverlappedAndPressing = false;
                        foreach (Player player in settings.generalSettings.players)
                        {
                            if(player!=currPlayer && player.IsPressingButton())
                            {
                                playerOverlappedAndPressing = true;
                                break;
                            }
                        }
                        if (isButtonOverlap && playerOverlappedAndPressing)
                        {
                            currButton.GetComponent<Image>().color = Color.red;
                        }
                        else
                        {
                            currButton.GetComponent<Image>().color = new Color(1.0f, 0.8f, 0.6f);
                        }

                        foreach (Player player in settings.generalSettings.players)
                        {
                            if (player != currPlayer && player.IsPressingButton() && player.GetActivebuttonIndex() == currPlayer.GetActivebuttonIndex())
                            {
                                return;
                            }
                        }
                        currPlayer.PressGameButton();
                    });
                    trigger.triggers.Add(pointerDown);
                    EventTrigger.Entry pointerUp = new EventTrigger.Entry();
                    pointerUp.eventID = EventTriggerType.PointerUp;
                    pointerUp.callback.AddListener(delegate (BaseEventData eventData)
                    {
                        Globals.trackEffectsAudioManager.PlayClip("Audio/clickUp");
                        //verify if button should be pressed
                        if (currPlayer.GetCurrNumPossibleActionsPerLevel() > 0)
                        {
                            currButton.GetComponent<Image>().color = currPlayer.GetButtonColor();
                        }
                        currPlayer.SetActiveInteraction(Globals.KeyInteractionType.NONE);
                        currPlayer.ReleaseGameButton();
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
                settings.generalSettings.players[inputFields.IndexOf(input)].SetName(input.text);
            });

            performanceMetrics.singlebuttonHits.Add(currPlayer, 0);
            List<KeyCode> keys = currPlayer.GetMyKeys();
            
            currPlayer.SetScore(0, 0, 0);
        }
        
        //UpdateButtonOverlaps(settings.generalSettings.players[0], 0);

        performanceMetrics.multiplayerButtonHits = 0;
        isGameplayStarted = true;

        exerciseGroupIndex = UnityEngine.Random.Range(0, settings.exercisesGroups.exerciseGroups.Count);
        countdownText.gameObject.SetActive(false);
        Shuffle<ExercisesListWrapper>(settings.exercisesGroups.exerciseGroups);

        Globals.backgroundAudioManager.StopCurrentClip();
        Globals.backgroundAudioManager.PlayInfinitClip("Audio/backgroundLoop", "Audio/backgroundLoop");

        StartCoroutine(ChangeLevel(false));

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
        foreach (Player player in settings.generalSettings.players)
        {
            StartCoroutine(logManager.WriteToLog("behavioralchangingcrossantlogs", "logs", new Dictionary<string, string>() {
                { "sessionId", Globals.gameId.ToString() },
                { "gameId", Globals.currLevelId.ToString() },
                { "playerId", player.GetId().ToString() },
                { "color", player.GetButtonColor().ToString() },
                { "currWord", player.GetCurrExercise().targetWord },
                { "scoreSystem", scoreSystemName },
                { "score", player.GetScore().ToString() },
                { "numberOfGivePresses", player.numGivePresses.ToString() },
                { "numberOfTakePresses", player.numTakePresses.ToString() }
            }));
        }
        
    }

    IEnumerator ChangeLevel(bool areWordsUnfinished)
    {
        if (numLevelsLeft > 0) { //<= 0 tells the game it is an infinite game (tutorial purposes)
            numLevelsLeft--;
            if (numLevelsLeft < 1) //quit on max num levels reached
            {
                Globals.gameSceneManager.EndGame();
            }
        }

        foreach (LetterSpawner spawner in letterSpawners)
        {
            spawner.UpdateCurrStarredWord("");
            spawner.StopSpawning();
        }
        ChangeTargetWords();


        if (areWordsUnfinished)
        {
            //Globals.effectsAudioManager.PlayClip("Audio/wordChangeBad");
            emoji.GetComponent<Animator>().Play("Sad");
        }
        else
        {
            Globals.effectsAudioManager.PlayClip("Audio/wordChangeGood");
            emoji.GetComponent<Animator>().Play("Smiling");
        }
        emoji.GetComponent<Animator>().speed = 0;

        foreach (Player player in settings.generalSettings.players)
        {
            player.SetNumPossibleActions(0);
            player.GetUI().GetComponentInChildren<Button>().onClick.Invoke(); //set track positions
            foreach (Button button in player.GetUI().GetComponentsInChildren<Button>())
            {
                button.GetComponent<Image>().color = Color.red;
                player.GetUI().GetComponentInChildren<Button>().onClick.Invoke(); //set track positions
                //button.interactable = false;
            }

        }

        int i = 5;
        countdownText.text = i.ToString();
        yield return new WaitForSeconds(2.0f);
        countdownText.gameObject.SetActive(true);
        for (i = 5; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        countdownText.gameObject.SetActive(false);

        emoji.GetComponent<Animator>().speed = 1;
        Globals.effectsAudioManager.PlayClip("Audio/snap");
        foreach (LetterSpawner spawner in letterSpawners)
        {
            spawner.BeginSpawning();
        }
        
        foreach(Player player in settings.generalSettings.players)
        {
            foreach (Button button in player.GetUI().GetComponentsInChildren<Button>())
            {
                button.GetComponent<Image>().color = player.GetButtonColor();
                button.onClick.Invoke();
            }
            player.ResetNumPossibleActions();
            player.GetUI().GetComponentInChildren<Button>().onClick.Invoke(); //set track positions
        }
        RecordMetrics();

        Globals.currLevelId++;
    }
    
    
    
    private void ChangeTargetWords()
    {
        List<Exercise> selectedExerciseGroup = new List<Exercise>(settings.exercisesGroups.exerciseGroups[exerciseGroupIndex++ % settings.exercisesGroups.exerciseGroups.Count].exercises);
        //if (selectedExerciseGroup.Count <= 0)
        //{
        //    selectedExerciseGroup = new List<Exercise>(settings.exercisesGroups.exerciseGroups[exerciseGroupIndex++ % settings.exercisesGroups.exerciseGroups.Count].exercises);
        //}

        int random = UnityEngine.Random.Range(0, selectedExerciseGroup.Count);
        Exercise newExercise = selectedExerciseGroup[random];
        selectedExerciseGroup.RemoveAt(random);


        int i = UnityEngine.Random.Range(0,1);
        foreach (Player player in settings.generalSettings.players)
        {
            //displayPanel.GetComponent<DisplayPanel>().SetTargetImage(newExercise.targetWord);
            player.SetCurrExercise(newExercise.playerExercises[(i++) % newExercise.playerExercises.Count]);
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
        bool usefulForMe = (currWordState.Length <= currTargetWord.Length && !currWordState.Contains(letterText) && currTargetWord.Contains(letterText));


        if (execute && usefulForMe)
        {
            int changeIndex = currTargetWord.IndexOf(letterText);
            while(true)
            {
                if (changeIndex == -1)
                {
                    return false;
                }else if(currWordState[changeIndex] == ' ')
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

        //verify if button should be pressed
        Globals.effectsAudioManager.PlayClip("Audio/badMove");
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
                foreach (Player usefulTargetPlayer in settings.generalSettings.players)
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
                foreach (Player usefulTargetPlayer in settings.generalSettings.players)
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
            Globals.effectsAudioManager.PlayClip("Audio/snap");
            emoji.GetComponent<Animator>().Play("Nice");
        }

        float otherPlayersCompletionMean = 0;
        int otherPlayersCount = settings.generalSettings.players.Count() - 1;
        float currPlayerCompletion = currHitter.GetCurrWordState().Count();
        foreach (Player innerPlayer in settings.generalSettings.players)
        {
            if (innerPlayer == currHitter)
            {
                continue;
            }
            otherPlayersCompletionMean += (float)innerPlayer.GetCurrWordState().Count() / otherPlayersCount;
        }
        Globals.DiffLetters playerDiff = (currPlayerCompletion > otherPlayersCompletionMean) ? Globals.DiffLetters.HIGHER : (currPlayerCompletion == otherPlayersCompletionMean) ? Globals.DiffLetters.EQUAL : Globals.DiffLetters.LOWER;

        bool scoreOptionFound = false;
        foreach (ScoreValue score in scores)
        {
            if (score.usefulForMe == usefulForMe && score.usefulForOther == usefulForOther && playerDiff == (Globals.DiffLetters)Enum.Parse(typeof(Globals.DiffLetters), score.diffLetters))
            {
                currHitter.SetScore(currHitter.GetScore() + score.myValue, score.myValue, 1.3f);
                foreach (Player innerPlayer in settings.generalSettings.players)
                {
                    if (innerPlayer == currHitter)
                    {
                        continue;
                    }
                    innerPlayer.SetScore(innerPlayer.GetScore() + score.otherValue, score.otherValue, 1.3f);
                }
                scoreOptionFound = true;
                break;
            }
        }

        if (!scoreOptionFound)
        {
            Debug.Log("could not find score option for <usefulForMe: " + usefulForMe + ", usefulForOther: " + usefulForOther + ", playerDiff: " + playerDiff + ">");
        }


        if (currHitter.GetCurrNumPossibleActionsPerLevel() < 1)
        {
            foreach (Button button in currHitter.GetUI().GetComponentsInChildren<Button>())
            {
                button.GetComponent<Image>().color = Color.red;
            }
            currHitter.ReleaseGameButton();
        }


        bool areWordsUnfinished = false;
        bool arePlayersWithoutActions = true;
        foreach (Player player in settings.generalSettings.players)
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

                if (!player.currExerciseFinished)
                    player.currExerciseFinished = true;
            }
            else
            {
                if (!player.currExerciseFinished && currHitter.GetCurrNumPossibleActionsPerLevel() > -1)
                    player.SetScore(player.GetScore() + settings.scoreSystem.completeWordMyScore, settings.scoreSystem.completeWordMyScore, 1.3f);
                foreach (Player innerPlayer in settings.generalSettings.players)
                {
                    if (player == innerPlayer)
                    {
                        continue;
                    }
                    if (!innerPlayer.currExerciseFinished && currHitter.GetCurrNumPossibleActionsPerLevel() > -1)
                        innerPlayer.SetScore(innerPlayer.GetScore() + settings.scoreSystem.completeWordOtherScore, settings.scoreSystem.completeWordOtherScore, 1.3f);
                }
            }
        }
        if (!areWordsUnfinished || arePlayersWithoutActions)
        {
            StartCoroutine(ChangeLevel(areWordsUnfinished));
        }
        
    }

    public List<Player> GetPlayers()
    {
        return settings.generalSettings.players;
    }
}


