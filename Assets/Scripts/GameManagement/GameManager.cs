using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AuxiliaryStructs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Mirror;
using Object = System.Object;


public class GameManager : NetworkManager
{
    public List<Player> players;

    //for netcode
    private int currPlayerI;
    private bool isMyUIInitted;


    private int startingLevelDelayInSeconds;

    public Button quitButton;
    public Button resetButton;
    private int numLevelsLeft;

    private int exerciseGroupIndex;

    public GameObject canvas;
    public GameObject stateCanvas;

    public Text countdownText;

    public bool isGameplayPaused;
    public bool isGameplayStarted;

    public bool isButtonOverlap;

    public GameObject emoji;

    public LetterSpawner[] letterSpawners;

    //placeholders for track markers
    public List<GameObject> pointerPlaceholders;

    private float timeLeft;

    private string scoreSystemPath; //to be able to recover condition



    public void PauseGame()
    {
        foreach (LetterSpawner ls in letterSpawners)
        {
            foreach (Letter letter in ls.GetComponentsInChildren<Letter>())
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
        // isButtonOverlap = false;
        // foreach (Player player in Globals.settings.generalSettings.players)
        // {
        //     if (player != currPlayer && player.GetActivebuttonIndex() == potentialIndex)
        //     {
        //         isButtonOverlap = true;
        //         break;
        //     }
        // }
        //
        // foreach (Player player in Globals.settings.generalSettings.players)
        // {
        //     foreach(GameObject obj in player.GetMaskedHalf())
        //     {
        //         obj.SetActive(!isButtonOverlap);
        //     }
        //     if(!isButtonOverlap)
        //     {
        //         player.GetActiveHalf().AddRange(player.GetMaskedHalf());
        //     }
        //     player.UpdateActiveHalf(player.IsPressingButton());
        // }
    }

    public void Start()
    {
        players = new List<Player>();

        //setup net code
        autoCreatePlayer = false;
        currPlayerI = 0;
        isMyUIInitted = false;

        //check connection type
        if (!NetworkClient.active)
        {
            if (Globals.settings.networkSettings.currMultiplayerOption == "LOCAL")
            {
                OnStartServer();
                // setup players and level directly if local
                if (Globals.settings.networkSettings.currMultiplayerOption == "LOCAL")
                {
                    AddPlayer(true, 0);
                    AddPlayer(true, 1);
                    StartCoroutine(ChangeLevel(false, false));
                }
            }
            else if (Globals.settings.networkSettings.currMultiplayerOption == "ONLINE")
            {
                if (Globals.settings.networkSettings.currOnlineOption == "HOST")
                {
                    this.StartHost();
                }
                else if (Globals.settings.networkSettings.currOnlineOption == "CLIENT")
                {
                    this.StartClient();
                    this.networkAddress = GUILayout.TextField(Globals.settings.networkSettings.serverIP);
                }
            }
        }
        else
        {
            Debug.Log("Connecting to " + this.networkAddress + "...");
            if (GUILayout.Button("Cancel Connection Attempt"))
            {
                this.StopClient();
            }
        }

        // client ready
        if (NetworkClient.isConnected && !ClientScene.ready)
        {
            if (GUILayout.Button("Client Ready"))
            {
                ClientScene.Ready(NetworkClient.connection);

                if (ClientScene.localPlayer == null)
                {
                    ClientScene.AddPlayer(NetworkClient.connection);
                }
            }
        }

    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        scoreSystemPath = Globals.settings.scoreSystem.path;

        isGameplayStarted = false;

        startingLevelDelayInSeconds = 5;

        quitButton.onClick.AddListener(delegate() { EndGame(); });
        resetButton.onClick.AddListener(delegate() { ResetGame(); });

        if (Globals.savedObjects == null)
        {
            Globals.InitGlobals();
        }

        isGameplayPaused = false;

        if (Globals.gameParam == Globals.ExercisesConfig.TUTORIAL)
        {
            //special condition also removes the score
            foreach (Player player in players)
            {
                player.scoreText.gameObject.SetActive(false);
            }
        }

        switch (Globals.settings.generalSettings.logMode)
        {
            case "DEBUG":
                Globals.logManager = new DebugLogManager();
                break;

            case "MONGODB":
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


        isGameplayStarted = true;

        exerciseGroupIndex = UnityEngine.Random.Range(0, Globals.settings.exercisesGroups.exerciseGroups.Count);
        countdownText.gameObject.SetActive(false);
        Shuffle<ExercisesListWrapper>(Globals.settings.exercisesGroups.exerciseGroups);

        Globals.backgroundAudioManager.StopCurrentClip();
        Globals.backgroundAudioManager.PlayInfinitClip(Globals.backgroundMusicPath, Globals.backgroundMusicPath);

    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        //if all players are connected, init their them and start the first level
        if (currPlayerI == players.Count)
        {
            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];

                //clients do not have local players' representation created
                PlayerInfo currPlayerInfo = Globals.settings.generalSettings.playersParams[i];

                string bufferedPlayerId = "";
                if (i < Globals.bufferedPlayerIds.Count)
                {
                    bufferedPlayerId = Globals.bufferedPlayerIds[i];
                }
                else
                {
                    bufferedPlayerId = "NO_NAME_" + i;
                }

                currPlayerInfo.id = bufferedPlayerId;
                
                player.info = currPlayerInfo;
                player.info.numGives = 0;
                player.info.numTakes = 0;
                
                player.SetTopMask(i % 2 == 0);
                player.SetScore(0, 0, 0);
            }

            StartCoroutine(ChangeLevel(false, false));
        }
        else if (currPlayerI > players.Count)
        {
            currPlayerI--;
            Debug.Log("Unable To Join, game is full");
            return;
        }

    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        //if my ui is not initted init it and disallow interaction with the other
        AddPlayer(!isMyUIInitted, currPlayerI++);
        if (!isMyUIInitted)
        {
            for (int i = 0; i < currPlayerI; i++)
            {
                AddPlayer(isMyUIInitted, i);
            }

            isMyUIInitted = true;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        //when a player looses connection, the game ends immediately
        EndGame();
    }


    void AddPlayer(bool allowInteraction, int i)
    {

        // currPlayer.Init(allowInteraction, bufferedPlayerId, this, playerMarkerPrefab, playerMarkersContainer, playerUI,
        //     wordPanelsObject.transform.GetChild(i).gameObject, scorePanelsObject.transform.GetChild(i).gameObject,
        //     (i % 2 == 0));
        GameObject playerGameObject = Instantiate(playerPrefab);
        Player player = playerGameObject.GetComponent<Player>();

        players.Add(player);


        // GameObject playerUI = player.ui;
        // //set buttons for touch screen
        // UnityEngine.UI.Button[] playerButtons = playerUI.GetComponentsInChildren<UnityEngine.UI.Button>();
        // for (int buttonI = 0; buttonI < playerButtons.Length; buttonI++)
        // {
        //     UnityEngine.UI.Button currButton = playerButtons[buttonI];
        //     if (buttonI < pointerPlaceholders.Count)
        //     {
        //         currButton.GetComponent<Image>().color = player.GetButtonColor();
        //         int innerButtonI = buttonI; //for coroutine to save the iterated values
        //         currButton.onClick.AddListener(delegate ()
        //         {
        //             //verify if button should be pressed
        //             if (player.GetCurrNumPossibleActionsPerLevel() < 1)
        //             {
        //                 Globals.trackEffectsAudioManager.PlayClip("Audio/badMove");
        //                 return;
        //             }
        //             if (player.GetActivebuttonIndex() != innerButtonI)
        //             {
        //                 Globals.trackEffectsAudioManager.PlayClip("Audio/trackChange");
        //             }
        //
        //             playerButtons[player.GetActivebuttonIndex()].GetComponent<Image>().color =
        //                 player.GetButtonColor();
        //             UpdateButtonOverlaps(player, innerButtonI);
        //             player.SetActiveButton(innerButtonI, pointerPlaceholders[innerButtonI].transform.position);
        //             currButton.GetComponent<Image>().color = new Color(1.0f, 0.82f, 0.0f);
        //         });
        //     }
        //     else
        //     {
        //         int j = buttonI - pointerPlaceholders.Count + 1;
        //         Globals.KeyInteractionType iType = (Globals.KeyInteractionType)j;
        //         EventTrigger trigger = currButton.gameObject.AddComponent<EventTrigger>();
        //         EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        //         pointerDown.eventID = EventTriggerType.PointerDown;
        //         pointerDown.callback.AddListener(delegate (BaseEventData eventData)
        //         {
        //             currButton.GetComponent<Image>().color = new Color(1.0f, 0.82f, 0.0f);
        //
        //             //verify if button should be pressed
        //             bool playerOverlappedAndPressing = false;
        //             foreach (Player innerPlayer in players)
        //             {
        //                 if (innerPlayer != player && player.IsPressingButton())
        //                 {
        //                     playerOverlappedAndPressing = true;
        //                     break;
        //                 }
        //             }
        //
        //             if (player.GetCurrNumPossibleActionsPerLevel() < 1 ||
        //                 (isButtonOverlap && playerOverlappedAndPressing))
        //             {
        //                 Globals.trackEffectsAudioManager.PlayClip("Audio/badMove");
        //                 currButton.GetComponent<Image>().color = Color.red;
        //                 return;
        //             }
        //
        //             Globals.trackEffectsAudioManager.PlayClip("Audio/clickDown");
        //             player.SetActiveInteraction(iType);
        //
        //             foreach (Player innerPlayer in players)
        //             {
        //                 if (innerPlayer != player && player.IsPressingButton() &&
        //                     player.GetActivebuttonIndex() == player.GetActivebuttonIndex())
        //                 {
        //                     return;
        //                 }
        //             }
        //             player.PressGameButton();
        //         });
        //         trigger.triggers.Add(pointerDown);
        //         EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        //         pointerUp.eventID = EventTriggerType.PointerUp;
        //         pointerUp.callback.AddListener(delegate (BaseEventData eventData)
        //         {
        //             Globals.trackEffectsAudioManager.PlayClip("Audio/clickUp");
        //             //verify if button should be pressed
        //             if (player.GetCurrNumPossibleActionsPerLevel() > 0)
        //             {
        //                 currButton.GetComponent<Image>().color = player.GetButtonColor();
        //             }
        //             player.SetActiveInteraction(Globals.KeyInteractionType.NONE);
        //             player.ReleaseGameButton();
        //         });
        //         trigger.triggers.Add(pointerUp);
        //     }
        // }

    }


    // Update is called once per frame
    void Update()
    {
        if (isGameplayPaused || !isGameplayStarted)
        {
            return;
        }

    }

    [Server]
    private IEnumerator RecordMetrics()
    {
        //spawn questionnaires before changing word
        foreach (Player player in players)
        {
            yield return StartCoroutine(Globals.logManager.WriteToLog(Globals.settings.generalSettings.databaseName,
                "logs",
                new Dictionary<string, string>()
                {
                    {"gameId", Globals.gameId.ToString()},
                    {"levelId", Globals.currLevelId.ToString()},
                    {"playerId", player.GetId().ToString()},
                    {"color", player.GetButtonColor().ToString()},
                    {"levelWord", player.GetCurrExercise().targetWord},
                    {"wordState", player.GetCurrWordState()},
                    {"scoreSystem", scoreSystemPath},
                    {"score", player.GetScore().ToString()},
                    {"numberOfGives", player.info.numGives.ToString()},
                    {"numberOfTakes", player.info.numTakes.ToString()}
                }));

            player.info.numGives = 0;
            player.info.numTakes = 0;
        }

    }

    [Server]
    IEnumerator ChangeLevel(bool recordMetrics, bool areWordsUnfinished)
    {

        if (numLevelsLeft >= 0)
        {
            //<= 0 tells the game it is an infinite game (tutorial purposes)
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

        foreach (Player player in players)
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

        foreach (Player player in players)
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


        int i = UnityEngine.Random.Range(0, 1);
        foreach (Player player in players)
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
            while (true)
            {
                if (changeIndex == -1)
                {
                    return false;
                }
                else if (currWordState[changeIndex] == ' ')
                {
                    break;
                }

                changeIndex = currTargetWord.IndexOf(letterText, changeIndex + 1);
            }

            StringBuilder sb = new StringBuilder(currWordState);
            sb[changeIndex] = letterText;
            currWordState = sb.ToString();

            player.SetCurrWordState(currWordState);
            letter.GetComponent<Letter>().isTranslationEnabled = false;
            StartCoroutine(AnimateLetter(letter, player));
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
                foreach (Player usefulTargetPlayer in players)
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
                currHitter.info.numGives++;
                break;
            case Globals.KeyInteractionType.TAKE:
                usefulForMe = TestAndExecuteHit(true, letterText, letter, currHitter);
                if (usefulForMe)
                {
                    //letter.GetComponentInChildren<SpriteRenderer>().color = player.GetButtonColor();
                }

                foreach (Player usefulTargetPlayer in players)
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
                currHitter.info.numTakes++;
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
        int otherPlayersCount = players.Count() - 1;
        float currPlayerCompletion = currHitter.GetCurrWordState().Count();
        foreach (Player innerPlayer in players)
        {
            if (innerPlayer == currHitter)
            {
                continue;
            }

            otherPlayersCompletionMean += (float) innerPlayer.GetCurrWordState().Count() / otherPlayersCount;
        }

        Globals.DiffLetters playerDiff = (currPlayerCompletion > otherPlayersCompletionMean)
            ? Globals.DiffLetters.HIGHER
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
                foreach (Player innerPlayer in players)
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
        foreach (Player player in players)
        {
            if (player.GetCurrNumPossibleActionsPerLevel() > 0)
            {
                arePlayersWithoutActions = false;
            }

            string currWordState = player.GetCurrWordState();
            string currTargetWord = player.GetCurrExercise().targetWord;
            if (currWordState.CompareTo(currTargetWord) != 0)
            {
                if (!areWordsUnfinished)
                    areWordsUnfinished = true;

                if (!player.currExerciseFinished)
                    player.currExerciseFinished = true;
            }
            else
            {
                if (!player.currExerciseFinished && currHitter.GetCurrNumPossibleActionsPerLevel() > -1)
                    player.SetScore(player.GetScore() + Globals.settings.scoreSystem.completeWordMyScore,
                        Globals.settings.scoreSystem.completeWordMyScore, 1.3f);
                foreach (Player innerPlayer in players)
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
}


