using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    private int startingLevelDelayInSeconds;

    public Button quitButton;
    public Button resetButton;
    private int numLevelsLeft;

    private int exerciseGroupIndex;

    public GameObject canvas;
    public GameObject stateCanvas;

    public Text countdownText;


    public GameObject playerMarkersContainer;
    public GameObject playerMarkerPrefab;
    
    public bool isGameplayPaused;
    public bool isGameplayStarted;

    public bool isButtonOverlap;

    public GameObject wordPanelsObject;
    public GameObject scorePanelsObject;

    public GameObject emoji;

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

    public void EndGame()
    {
        Globals.backgroundAudioManager.PlayClip("Audio/backgroundLoop");
        SceneManager.LoadScene("gameover");
    }
    public void ResetGame()
    {
        EndGame();
        foreach (var obj in Globals.savedObjects)
        {
            Destroy(obj);
        }
        Globals.InitGlobals();
        SceneManager.LoadScene("paramsSetup");
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
        foreach (Player player in Globals.settings.generalSettings.players)
        {
            if (player != currPlayer && player.GetActivebuttonIndex() == potentialIndex)
            {
                isButtonOverlap = true;
                break;
            }
        }

        foreach (Player player in Globals.settings.generalSettings.players)
        {
            foreach(GameObject obj in player.GetMaskedHalf())
            {
                obj.SetActive(!isButtonOverlap);
            }
            if(!isButtonOverlap)
            {
                player.GetActiveHalf().AddRange(player.GetMaskedHalf());
            }
            player.UpdateActiveHalf(player.IsPressingButton());

            
        }
    }

    

    void Start()
    {
        isGameplayStarted = false;
        
        startingLevelDelayInSeconds = 5;

        quitButton.onClick.AddListener(delegate(){
            EndGame();
        });
        resetButton.onClick.AddListener(delegate () {
            ResetGame();

        });

        if (Globals.savedObjects == null)
        {
            Globals.InitGlobals();
        }
        isGameplayPaused = false;

        if (Globals.gameParam == Globals.ExercisesConfig.TUTORIAL)
        {
            //special condition also removes the score
            scorePanelsObject.transform.GetChild(0).GetComponentInChildren<Text>().gameObject.SetActive(false);
            scorePanelsObject.transform.GetChild(1).GetComponentInChildren<Text>().gameObject.SetActive(false);
        }
        
        switch(Globals.settings.generalSettings.logMode)
        {
            case "DEBUG":
                Globals.logManager = new DebugLogManager();
                break;
            
            case "MLAB":
                Globals.logManager = new MongoDBLogManager();
                break;
            
            default:
                Globals.logManager = new DebugLogManager();
                break;
        }
        Globals.logManager.InitLogs(this);

        numLevelsLeft = Globals.settings.generalSettings.numLevels;
        
        DontDestroyOnLoad(stateCanvas);
        Globals.savedObjects.Add(stateCanvas);


        for (int i = 0; i < Globals.settings.generalSettings.players.Count; i++)
        {
            GameObject playerUI = playerUIs[i];
            Player currPlayer = Globals.settings.generalSettings.players[i];

            string bufferedPlayerIds = "";
            if(i < Globals.bufferedPlayerIds.Count){
                bufferedPlayerIds = Globals.bufferedPlayerIds[i];
            }
            else
            {
                bufferedPlayerIds = "NO_NAME_" + i;
            }

            currPlayer.Init(bufferedPlayerIds, this, playerMarkerPrefab, playerMarkersContainer, playerUI,
                wordPanelsObject.transform.GetChild(i).gameObject, scorePanelsObject.transform.GetChild(i).gameObject,
                (i % 2 == 0));
            currPlayer.GetWordPanel().transform.Find("panel/Layout").GetComponent<SpriteRenderer>().color =
                currPlayer.GetBackgroundColor();
            
            //set buttons for touch screen
            UnityEngine.UI.Button[] playerButtons = playerUI.GetComponentsInChildren<UnityEngine.UI.Button>();

            for (int buttonI = 0; buttonI < playerButtons.Length; buttonI++)
            {
                UnityEngine.UI.Button currButton = playerButtons[buttonI];
                if (buttonI < pointerPlaceholders.Count)
                {
                    currButton.GetComponent<Image>().color = currPlayer.GetButtonColor();
                    int innerButtonI = buttonI; //for coroutine to save the iterated values
                    currButton.onClick.AddListener(delegate ()
                    {
                        //verify if button should be pressed
                        if (currPlayer.GetCurrNumPossibleActionsPerLevel() < 1)
                        {
                            Globals.trackEffectsAudioManager.PlayClip("Audio/badMove");
                            return;
                        }
                        if (currPlayer.GetActivebuttonIndex() != innerButtonI)
                        {
                            Globals.trackEffectsAudioManager.PlayClip("Audio/trackChange");
                        }

                        playerButtons[currPlayer.GetActivebuttonIndex()].GetComponent<Image>().color =
                            currPlayer.GetButtonColor();
                        UpdateButtonOverlaps(currPlayer, innerButtonI);
                        currPlayer.SetActiveButton(innerButtonI, pointerPlaceholders[innerButtonI].transform.position);
                        currButton.GetComponent<Image>().color = new Color(1.0f, 0.82f, 0.0f);
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
                        currButton.GetComponent<Image>().color = new Color(1.0f, 0.82f, 0.0f);

                        //verify if button should be pressed
                        bool playerOverlappedAndPressing = false;
                        foreach (Player player in Globals.settings.generalSettings.players)
                        {
                            if (player != currPlayer && player.IsPressingButton())
                            {
                                playerOverlappedAndPressing = true;
                                break;
                            }
                        }

                        if (currPlayer.GetCurrNumPossibleActionsPerLevel() < 1 ||
                            (isButtonOverlap && playerOverlappedAndPressing))
                        {
                            Globals.trackEffectsAudioManager.PlayClip("Audio/badMove");
                            currButton.GetComponent<Image>().color = Color.red;
                            return;
                        }

                        Globals.trackEffectsAudioManager.PlayClip("Audio/clickDown");
                        currPlayer.SetActiveInteraction(iType);

                        foreach (Player player in Globals.settings.generalSettings.players)
                        {
                            if (player != currPlayer && player.IsPressingButton() &&
                                player.GetActivebuttonIndex() == currPlayer.GetActivebuttonIndex())
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

            List<KeyCode> keys = currPlayer.GetMyKeys();
            currPlayer.SetScore(0, 0, 0);
        }
        
        isGameplayStarted = true;

        exerciseGroupIndex = UnityEngine.Random.Range(0, Globals.settings.exercisesGroups.exerciseGroups.Count);
        countdownText.gameObject.SetActive(false);
        Shuffle<ExercisesListWrapper>(Globals.settings.exercisesGroups.exerciseGroups);

        Globals.backgroundAudioManager.StopCurrentClip();
        Globals.backgroundAudioManager.PlayInfinitClip(Globals.backgroundMusicPath, Globals.backgroundMusicPath);

        StartCoroutine(ChangeLevel(false, false));
    }


    // Update is called once per frame
    void Update()
    {
        if (isGameplayPaused || !isGameplayStarted)
        {
            return;
        }
        
    }

    private IEnumerator RecordMetrics()
    {
        //spawn questionnaires before changing word
        foreach (Player player in Globals.settings.generalSettings.players)
        {
            yield return StartCoroutine(Globals.logManager.WriteToLog(Globals.settings.generalSettings.databaseName, "logs",
                new Dictionary<string, string>()
                {
                    {"gameId", Globals.gameId.ToString()},
                    {"levelId", Globals.currLevelId.ToString()},
                    {"playerId", player.GetId().ToString()},
                    {"color", player.GetButtonColor().ToString()},
                    {"levelWord", player.GetCurrExercise().targetWord},
                    {"wordState", player.GetCurrWordState()},
                    {"scoreSystem", scoreSystemName},
                    {"score", player.GetScore().ToString()},
                    {"numberOfGives", player.numGives.ToString()},
                    {"numberOfTakes", player.numTakes.ToString()}
                }));

            player.numGives = 0;
            player.numTakes = 0;
        }
        
    }

    IEnumerator ChangeLevel(bool recordMetrics, bool areWordsUnfinished)
    {
        
        if (numLevelsLeft >= 0) { //<= 0 tells the game it is an infinite game (tutorial purposes)
            if (numLevelsLeft < 1) //quit on max num levels reached
            {
                yield return RecordMetrics();
                EndGame();
                yield return null;
            }
            numLevelsLeft--;
        }

        foreach (LetterSpawner spawner in letterSpawners)
        {
            spawner.StopSpawning();
        }
        Globals.effectsAudioManager.PlayClip("Audio/wordChange");

        if (recordMetrics)
        {
            yield return RecordMetrics();
        }

        foreach (LetterSpawner spawner in letterSpawners)
        {
            spawner.UpdateCurrStarredWord("");
        }
        ChangeTargetWords();


        if (areWordsUnfinished)
        {
            emoji.GetComponent<Animator>().Play("Sad");
        }
        else
        {
            emoji.GetComponent<Animator>().Play("Smiling");
        }
        emoji.GetComponent<Animator>().speed = 0;

        foreach (Player player in Globals.settings.generalSettings.players)
        {
            player.GetUI().GetComponentInChildren<Button>().onClick.Invoke(); //set track positions
            foreach (Button button in player.GetUI().GetComponentsInChildren<Button>())
            {
                button.GetComponent<Image>().color = Color.red;
            }
        }


        int i = startingLevelDelayInSeconds;
        countdownText.text = i.ToString();
        yield return new WaitForSeconds(2.0f);
        countdownText.gameObject.SetActive(true);
        for (i = startingLevelDelayInSeconds; i > 0; i--)
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
        
        foreach(Player player in Globals.settings.generalSettings.players)
        {
            foreach (Button button in player.GetUI().GetComponentsInChildren<Button>())
            {
                button.GetComponent<Image>().color = player.GetButtonColor();
            }
            player.ResetNumPossibleActions();
            player.GetUI().GetComponentInChildren<Button>().onClick.Invoke(); //set track positions
        }
        
        Globals.currLevelId++;
    }
    
    
    
    private void ChangeTargetWords()
    {
        List<Exercise> selectedExerciseGroup = new List<Exercise>(Globals.settings.exercisesGroups
            .exerciseGroups[exerciseGroupIndex++ % Globals.settings.exercisesGroups.exerciseGroups.Count].exercises);
        if (selectedExerciseGroup.Count <= 0)
        {
            Debug.Log("No exercises available");
        }

        int random = UnityEngine.Random.Range(0, selectedExerciseGroup.Count);
        Exercise newExercise = selectedExerciseGroup[random];
        selectedExerciseGroup.RemoveAt(random);


        int i = UnityEngine.Random.Range(0,1);
        foreach (Player player in Globals.settings.generalSettings.players)
        {
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
        bool usefulForMe = (currWordState.Length <= currTargetWord.Length && currTargetWord.Contains(letterText));


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

    public void RecordHit(GameObject letter, Player currHitter)
    {
        
        Letter theActualLetter = letter.gameObject.GetComponent<Letter>();
        if (theActualLetter.IsLocked())
        {
            return;
        }
        theActualLetter.Lock();
        char letterText = theActualLetter.letterText;
        letter.transform.localScale *= 1.2f;
        
        
        //verify if button should be pressed
        currHitter.DecreasePossibleActionsPerLevel();

        
        
        //different rewards in different utility conditions
        bool usefulForMe = false;
        bool usefulForOther = false;
        
        Globals.KeyInteractionType playerIT = currHitter.GetActiveInteraction();
        List<ScoreValue> scores = new List<ScoreValue>();
        switch (playerIT)
        {
            case Globals.KeyInteractionType.GIVE:
                usefulForMe = TestAndExecuteHit(false, letterText, letter, currHitter);
                foreach (Player usefulTargetPlayer in Globals.settings.generalSettings.players)
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
                scores = Globals.settings.scoreSystem.giveScores;
                currHitter.numGives++;
                break;
            case Globals.KeyInteractionType.TAKE:
                usefulForMe = TestAndExecuteHit(true, letterText, letter, currHitter);
                if (usefulForMe)
                {
                    //letter.GetComponentInChildren<SpriteRenderer>().color = player.GetButtonColor();
                }
                foreach (Player usefulTargetPlayer in Globals.settings.generalSettings.players)
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
                scores = Globals.settings.scoreSystem.takeScores;
                currHitter.numTakes++;
                break;
        }

        if (usefulForMe || usefulForOther)
        {
            Globals.effectsAudioManager.PlayClip("Audio/snap");
            emoji.GetComponent<Animator>().Play("Nice");
        }
        else
        {
            Globals.effectsAudioManager.PlayClip("Audio/badMove");
        }

        float otherPlayersCompletionMean = 0;
        int otherPlayersCount = Globals.settings.generalSettings.players.Count() - 1;
        float currPlayerCompletion = currHitter.GetCurrWordState().Count();
        foreach (Player innerPlayer in Globals.settings.generalSettings.players)
        {
            if (innerPlayer == currHitter)
            {
                continue;
            }
            otherPlayersCompletionMean += (float)innerPlayer.GetCurrWordState().Count() / otherPlayersCount;
        }

        Globals.DiffLetters playerDiff = (currPlayerCompletion > otherPlayersCompletionMean)
            ?
            Globals.DiffLetters.HIGHER
            : (currPlayerCompletion == otherPlayersCompletionMean)
                ? Globals.DiffLetters.EQUAL
                : Globals.DiffLetters.LOWER;

        bool scoreOptionFound = false;
        foreach (ScoreValue score in scores)
        {
            if (score.usefulForMe == usefulForMe && score.usefulForOther == usefulForOther && playerDiff ==
                (Globals.DiffLetters) Enum.Parse(typeof(Globals.DiffLetters), score.diffLetters))
            {
                currHitter.SetScore(currHitter.GetScore() + score.myValue, score.myValue, 1.3f);
                foreach (Player innerPlayer in Globals.settings.generalSettings.players)
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
            Debug.Log("could not find score option for <usefulForMe: " + usefulForMe + ", usefulForOther: " +
                      usefulForOther + ", playerDiff: " + playerDiff + ">");
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
        foreach (Player player in Globals.settings.generalSettings.players)
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
                    player.SetScore(player.GetScore() + Globals.settings.scoreSystem.completeWordMyScore,
                        Globals.settings.scoreSystem.completeWordMyScore, 1.3f);
                foreach (Player innerPlayer in Globals.settings.generalSettings.players)
                {
                    if (player == innerPlayer)
                    {
                        continue;
                    }

                    if (!innerPlayer.currExerciseFinished && currHitter.GetCurrNumPossibleActionsPerLevel() > -1)
                        innerPlayer.SetScore(
                            innerPlayer.GetScore() + Globals.settings.scoreSystem.completeWordOtherScore,
                            Globals.settings.scoreSystem.completeWordOtherScore, 1.3f);
                }
            }
        }
        if (!areWordsUnfinished || arePlayersWithoutActions)
        {
            StartCoroutine(ChangeLevel(true, areWordsUnfinished));
        }
        
    }

    public List<Player> GetPlayers()
    {
        return Globals.settings.generalSettings.players;
    }
}


