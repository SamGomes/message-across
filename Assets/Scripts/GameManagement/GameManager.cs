using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using AuxiliaryStructs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Mirror;
using NUnit.Framework.Constraints;
using Telepathy;
using Object = System.Object;
using Random = UnityEngine.Random;

public class PlayerServerState
{
    public int orderNum;
    public int currNumPossibleActionsPerLevel;
    public int score;
        
    public PlayerExercise currExercise;
    public bool currExerciseFinished;
    public string currWordState;

} 

//mainly implements the server
public class GameManager : NetworkManager
{
    public List<Player> players;
    public List<GameObject> playerGameObjects;
    public List<PlayerServerState> playerServerStates;

    private int startingLevelDelayInSeconds;

    public GameObject serverDebugUI;
    public GameObject gameCodeUI;
    
    public Button quitButton;
    public Button resetButton;
    public Button pauseButton;
    private int numLevelsLeft;

    private int exerciseGroupIndex;


    public bool isGameplayPaused;
    public bool isGameplayStarted;

    public bool isLaneOverlap;


    public LetterSpawner[] letterSpawners;


    private float timeLeft;

    private string scoreSystemPath; //to be able to recover condition
    
    
    private List<char> letters = new List<char>(){ 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    private List<List<char>> letterPools;

    float playersLettersSpawnP;


    public ClientMainGameElements cmge;

    public GameObject letterPit;

    private bool inLobby;

    private ScoreSystem currScoreSystem;
//    public Dictionary<string, string> codesAndIPs; 


    private bool isGameReady;
        
    [Server]
    public void PauseGame()
    {
        foreach (LetterSpawner ls in letterSpawners)
        {
            foreach (Letter letter in ls.GetComponentsInChildren<Letter>())
            {
                letter.Lock();
            }

            ls.enabled = false;
        }

        isGameplayPaused = true;
    }

    [Server]
    public void ResumeGame()
    {
        foreach (LetterSpawner ls in letterSpawners)
        {
            foreach (Letter letter in ls.GetComponentsInChildren<Letter>())
            {
                letter.Unlock();
            }
            ls.enabled = true;
        }

        isGameplayPaused = false;
    }
    
    [Server]
    public void ResetGame()
    {
        cmge.EndGameInAllClients();
        foreach (var obj in Globals.savedObjects)
        {
            Destroy(obj);
        }

        Globals.InitGlobals();
        SceneManager.LoadScene("paramsSetup");
    }
    
    [Server]
    public void EndGame()
    {
        cmge.EndGameInAllClients();
        foreach (var obj in Globals.savedObjects)
        {
            Destroy(obj);
        }
        Globals.InitGlobals();
        isGameplayStarted = false;
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


    [Server]
    public string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[UnityEngine.Random.Range(0, s.Length)]).ToArray());
    }
    

    public override void Start()
    {
        inLobby = true;
        players = new List<Player>();
        playerServerStates = new List<PlayerServerState>();
        
        //setup net code
//        codesAndIPs = new Dictionary<string, string>();

        //check connection type
        if (!NetworkClient.active)
        {
            if (Globals.settings.networkSettings.currMultiplayerOption == "LOCAL")
            {
                OnStartServer();
                // setup players and level directly if local
                if (Globals.settings.networkSettings.currMultiplayerOption == "LOCAL")
                {
//                    CreatePlayer(null);
//                    CreatePlayer(null);
//                    InitPlayer(null,0);
//                    InitPlayer(null,1);
                    StartCoroutine(ChangeLevel(false, false));
                }
            }
            else if (Globals.settings.networkSettings.currMultiplayerOption == "ONLINE")
            {
                if (Globals.activeInfoPopups)
                {
                    Popup popup = new Popup(false);
                    popup.SetMessage("Welcome to the wait lobby. This is where you wait for the all players to join." +
                                     "The host IP is included at the top left of the screen. " +
                                     "The wait lobby spawns some letters for you to train." +
                                     "When you are ready to begin," +
                                     " simply click on the \"Ready\" button. ");
                    popup.DisplayPopup();
                }

                
                if (Globals.settings.networkSettings.currOnlineOption == "HOST")
                {
                    StartHost();
//                    string randomStr = RandomString(5);
//                    while (codesAndIPs.ContainsValue(randomStr))
//                    {
//                        randomStr = RandomString(5);
//                    }
//                    codesAndIPs.Add(networkAddress, randomStr);

                    foreach(NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if(ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        {
                            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                            {
                                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    //do what you want with the IP here... add it to a list, just get the first and break out. Whatever.
//                                    Debug.Log(ip.Address.ToString());
                                    Globals.settings.networkSettings.serverCode = ip.Address.ToString();
                                }
                            }
                        }  
                    }


                }
                else if (Globals.settings.networkSettings.currOnlineOption == "CLIENT")
                {
                    if (Globals.settings.networkSettings.serverCode == "")
                    {
                        this.networkAddress = "localhost";
                    }
                    else
                    {
                        this.networkAddress = Globals.settings.networkSettings.serverCode;
                    }
                    this.StartClient();
                    serverDebugUI.SetActive(false);
                }
                gameCodeUI.GetComponentInChildren<Text>().text = "Host IP: " + 
                                                                 Globals.settings.networkSettings.serverCode;
            }
        }
        else
        {
            Debug.Log("Connecting to " + this.networkAddress + "...");
            // if (GUILayout.Button("Cancel Connection Attempt"))
            // {
            //     this.StopClient();
            // }
        }

        // client ready
        // if (NetworkClient.isConnected && !ClientScene.ready)
        // {
        //     if (GUILayout.Button("Client Ready"))
        //     {
        //         ClientScene.Ready(NetworkClient.connection);
        //
        //         if (ClientScene.localPlayer == null)
        //         {
        //             ClientScene.AddPlayer(NetworkClient.connection);
        //         }
        //     }
        // }

    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        isGameReady = false;
        
        scoreSystemPath = Globals.settings.scoreSystem.path;

        isGameplayStarted = false;
        isGameplayPaused = false;

        startingLevelDelayInSeconds = 5;

        serverDebugUI.SetActive(true);
        quitButton.onClick.AddListener(delegate() { EndGame(); });
        resetButton.onClick.AddListener(delegate() { ResetGame(); });
        pauseButton.onClick.AddListener(delegate() {
            if (isGameplayPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        });

        if (Globals.savedObjects == null)
        {
            Globals.InitGlobals();
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

        // DontDestroyOnLoad(stateCanvas);
        // Globals.savedObjects.Add(stateCanvas);

        isGameplayStarted = true;

        exerciseGroupIndex = Random.Range(0, Globals.settings.exercisesGroups.exerciseGroups.Count);
        Shuffle(Globals.settings.exercisesGroups.exerciseGroups);

        cmge.StopCurrentAudioClip(0);
        cmge.PlayInfiniteAudioClip(0, Globals.backgroundMusicPath, Globals.backgroundMusicPath);

    }


    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        StopClient();
        Popup popup = new Popup(false);
        popup.SetMessage("Disconnected by host or game not found. Returning to Menu...");
        popup.AddButton("OK", delegate { 
            SceneManager.LoadScene("startOnline");
            return 0; 
        });
        popup.DisplayPopup();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //check if room is full
        if (numPlayers == Globals.settings.generalSettings.playersParams.Count)
        {
            conn.Disconnect();
            return;
        }
        
        
        CreatePlayer(conn);

        cmge.DisplayCountdownText(false, ""); //disable countdown text in lobby
        for(int i=0; i< players.Count; i++)
        {
            InitPlayer(conn, i);
        }

        //create lobby levels on first player entry
        if (numPlayers == 1)
        {
            currScoreSystem = Globals.settings.lobbyScoreSystem;
            
            //init letter spawner stuff
            int spawnerId = 0;
            letterPools = new List<List<char>>();
            playersLettersSpawnP = Globals.settings.generalSettings.playersLettersSpawnP;

            foreach (LetterSpawner spawner in letterSpawners)
            {
                spawner.SetId(spawnerId++);
                letterPools.Add(new List<char>());
            
                //start spawners update cycle
                StartCoroutine(UpdateSpawner(spawner));
                spawner.StartSpawning();
            }
            StartCoroutine(ChangeLobbyLevel());
        }
        
        if (numPlayers == Globals.settings.generalSettings.playersParams.Count)
        {
            isGameReady = true;
        }
    }


    IEnumerator StartAfterInit()
    {
        yield return new WaitForSeconds(1.0f);
        
        currScoreSystem = Globals.settings.scoreSystem;
        //special condition removes the score
        if (Globals.gameParam == Globals.ExercisesConfig.CUSTOM)
        {
            foreach (Player player in players)
            {
                //special condition also removes the score
                player.ShowScoreText();
            }
        }
        
        //start spawners
        foreach (LetterSpawner spawner in letterSpawners)
        {
            //update cycle started in lobby
            spawner.StopSpawning();
        }
        
        //start player buttons
        foreach (PlayerServerState pss in playerServerStates)
        {
            Player player = players[pss.orderNum];
            FinishPlayerAction(pss, player.GetPressedButtonIndex());
            //player.ChangeAllButtonsColor(Color.red); done internally to increase performance
            player.DisableAllButtons(Color.red);
        }
        cmge.DisplayCountdownText(true, "Get Ready...");

        //start first level
        StartCoroutine(ChangeLevel(false,false));
    }
    

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        //when a player looses connection, the game ends immediately
        EndGame();
    }

    void InitPlayer(NetworkConnection conn, int orderNum)
    {
        Player player = players[orderNum];
        GameObject playerInstance = playerGameObjects[orderNum];
        
        //setup player after instantiation
        //clients do not have local players' info created
        PlayerInfo currPlayerInfo = Globals.settings.generalSettings.playersParams[orderNum];

        string bufferedPlayerId = "";
        if (numPlayers < Globals.bufferedPlayerIds.Count)
        {
            bufferedPlayerId = Globals.bufferedPlayerIds[orderNum];
        }
        else
        {
            bufferedPlayerId = "NO_NAME_" + orderNum;
        }

        currPlayerInfo.id = bufferedPlayerId;

        //all these methods are broadcasted to each client
        player.SetActiveLayout(orderNum % 2 == 0);
        player.SetTopMask(orderNum % 2 == 0);
        player.Init(currPlayerInfo, playerInstance, orderNum);

        player.SetScore(0, 0, 0);
        
        //reset player actions in server (maintains order implicitly)
        PlayerServerState newPlayerState = new PlayerServerState();
        newPlayerState.currNumPossibleActionsPerLevel = currPlayerInfo.numPossibleActionsPerLevel;
        newPlayerState.score = -1;
        newPlayerState.currExercise = new PlayerExercise();
        newPlayerState.currExerciseFinished = false;
        newPlayerState.currWordState = "";
        newPlayerState.orderNum = orderNum;
        playerServerStates.Add(newPlayerState);
        player.UpdateNumPossibleActions(playerServerStates[orderNum].currNumPossibleActionsPerLevel);

        player.HideScoreText();
    }

   

    void CreatePlayer(NetworkConnection conn)
    {
        GameObject playerGameObject = Instantiate(playerPrefab);
        //instantiates playerGameObject in all clients automatically
        NetworkServer.AddPlayerForConnection(conn, playerGameObject);
        Player player = playerGameObject.GetComponent<Player>();
        players.Add(player);
        playerGameObjects.Add(playerGameObject);
    }


    // Update is called once per frame
    void Update()
    {
        if (isGameplayPaused || !isGameplayStarted)
        {
            return;
        }

//        if (inLobby)
//        {
//            bool allReady = true;
//            foreach (Player player in players)
//            {
//                if (!player.IsReady())
//                {
//                    allReady = false;
//                    break;
//                }
//            }
//
//            if (allReady)
//            {
//                inLobby = false;
//                //TODO: receive acknowledgements instead of waiting a bit
//                StartCoroutine(StartAfterInit());
//            }
//        }
        
        PlayerTrackChanges();
        CheckMarkerCollisions();

        PlayerActionChanges();
    }

    
    
    
    //--------------------- player movement ------------------------
    [Server]
    public void PlayerTrackChanges()
    {
        foreach (Player player in players)
        {
            if (player != null && player.ChangedLane())
            {
                ChangeLane(player);
                UpdateButtonOverlaps(player, player.GetPressedButtonIndex());
                player.AckChangedLane();
            }
        }
    }
    
    [Server]
    public void UpdateButtonOverlaps(Player currPlayer, int potentialIndex)
    {
        isLaneOverlap = false;
        foreach (Player player in players)
        {
            if (player != currPlayer && player.GetActiveButtonIndex() == potentialIndex)
            {
                isLaneOverlap = true;
                break;
            }
        }
        
        foreach (Player player in players)
        {
            if(isLaneOverlap)
            {
                player.HideHalfMarker();
            }
            else
            {
                player.ShowHalfMarker();
            }
            player.UpdateActiveHalf(player.IsPressingButton());
        }
    }

    [Server]
    void ChangeLane(Player player)
    {
        int activeButtonI = player.GetActiveButtonIndex();
        int pressedButtonI = player.GetPressedButtonIndex();
        if (activeButtonI != pressedButtonI)
        {
            cmge.PlayAudioClip(2, "Audio/trackChange");
        }

        //update player state and marker in track
        player.SetActiveTrackButton(pressedButtonI, player.markerPlaceholders.GetChild(pressedButtonI).transform.position);
        
        //update client player UIs after commands are executed on client
        player.ChangeAllButtonsColor(player.GetButtonColor());
        player.ChangeButtonColor(pressedButtonI, new Color(1.0f, 0.82f, 0.0f));

    }
    

    
    
    
    
    
    
    //--------------------- spawners stuff ------------------------
    [Server]
    IEnumerator UpdateSpawner(LetterSpawner spawner)
    { 
        while (spawner.IsStopped())
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(spawner.minIntervalRange, spawner.maxIntervalRange));
        
        //pick new letter
        if (letterPools[spawner.GetId()].Count == 0)
        {
            ResetPool(spawner);
        }
        List<char> letterPool = letterPools[spawner.GetId()];
        
        int random = Random.Range(0, letterPool.Count - 1);
        char currLetter = letterPool[random];
        letterPool.RemoveAt(random);
        
        letterPools[spawner.GetId()] = letterPool;
        
        spawner.SpawnLetter(currLetter); //spawns the picked letter in each client
        
        StartCoroutine(UpdateSpawner(spawner));
    }
    
    [Server]
    private void ResetPool(LetterSpawner spawner)
    {

        List<char> currWordsLetters = new List<char>();
        List<char> allLetters = new List<char>();
        foreach (Player player in players)
        {
            //verify if the player has a word. If not, push random leters
            string currWord = playerServerStates[player.GetOrderNum()].currExercise.targetWord;
            if (currWord != null)
            {
                currWordsLetters = currWordsLetters.Union(currWord.ToCharArray()).ToList();
            }
            else
            {
                currWordsLetters.Add(letters[Random.Range(0, letters.Count)]);
            }
        }

        float total = currWordsLetters.Count / playersLettersSpawnP;
        List<char> letterPool = letterPools[spawner.GetId()];
        letterPool.AddRange(currWordsLetters);
        while (letterPool.Count < total)
        {
            if (allLetters.Count == 0)
            {
                allLetters.AddRange(letters);
            }

            char newLetter = allLetters[Random.Range(0, allLetters.Count - 1)];
            if (!currWordsLetters.Contains(newLetter))
            {
                letterPool.Add(newLetter);
            }
            allLetters.Remove(newLetter);
        }
        
        letterPools[spawner.GetId()] = letterPool;
    }

    [Server]
    private void ClearPool(LetterSpawner spawner)
    {
        letterPools[spawner.GetId()].Clear();
    }
    
    
    
    
    
    
    
    //--------------------- level change ------------------------
    
    [Server]
    private IEnumerator RecordMetrics()
    {
        //spawn questionnaires before changing word
        foreach (PlayerServerState pss in playerServerStates)
        {
            Player player = players[pss.orderNum];
            yield return StartCoroutine(Globals.logManager.WriteToLog(Globals.settings.generalSettings.databaseName,
                "logs",
                new Dictionary<string, string>()
                {
                    {"gameId", Globals.gameId.ToString()},
                    {"levelId", Globals.currLevelId.ToString()},
                    {"playerId", player.GetId().ToString()},
                    {"levelWord", pss.currExercise.targetWord},
                    {"wordState", pss.currWordState},
                    {"scoreSystem", scoreSystemPath},
                    {"score", pss.score.ToString()},
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

        cmge.PlayAudioClip(1, "Audio/wordChange");

        if (recordMetrics)
        {
            yield return RecordMetrics();
        }

        ChangeTargetWords();
        foreach (LetterSpawner spawner in letterSpawners)
        {
            ClearPool(spawner); //risky-> exercise may not have been synced yet
            spawner.UpdateCurrStarredWord("");
        }
        

        if (areWordsUnfinished)
        {
            cmge.SetEmojiAnim("Sad");
        }
        else
        {
            cmge.SetEmojiAnim("Smiling");
        }
        cmge.StopEmojiAnim();

        
        int i = startingLevelDelayInSeconds;
        yield return new WaitForSeconds(2.0f);
        for (i = startingLevelDelayInSeconds; i > 0; i--)
        {
            cmge.DisplayCountdownText(true, i.ToString());
            yield return new WaitForSeconds(1.0f);
        }

        cmge.DisplayCountdownText(false, "");

        cmge.StartEmojiAnim();
        cmge.PlayAudioClip(1, "Audio/snap");
        foreach (LetterSpawner spawner in letterSpawners)
        {
            spawner.StartSpawning();
        }

        foreach (PlayerServerState pss in playerServerStates)
        {
            Player player = players[pss.orderNum];
            foreach (Button button in player.GetUI().GetComponentsInChildren<Button>())
            {
  //            player.ChangeAllButtonsColor(player.GetColor()); done internally to increase performance
                player.ResetButtonStates();
                player.EnableAllButtons();
            }

            
            //reset player actions in server
            pss.currNumPossibleActionsPerLevel = player.info.numPossibleActionsPerLevel;
            player.UpdateNumPossibleActions(pss.currNumPossibleActionsPerLevel);
            
            player.GetUI().GetComponentInChildren<Button>().onClick.Invoke(); //set track positions
        }

        Globals.currLevelId++;
    }
    
    
    
    [Server]
    private void ChangeTargetWords()
    {
        Exercise newExercise = new Exercise();
        
        List<Exercise> selectedExerciseGroup = new List<Exercise>(Globals.settings.exercisesGroups
            .exerciseGroups[exerciseGroupIndex++ % Globals.settings.exercisesGroups.exerciseGroups.Count]
            .exercises);
        if (selectedExerciseGroup.Count <= 0)
        {
            Debug.Log("No exercises available");
        }

        int random = Random.Range(0, selectedExerciseGroup.Count);
        newExercise = selectedExerciseGroup[random];
        selectedExerciseGroup.RemoveAt(random);
    
        int i = Random.Range(0, 1);
        foreach (PlayerServerState pss in playerServerStates)
        {
            Player player = players[pss.orderNum];
            pss.currExercise = newExercise.playerExercises[(i++) % newExercise.playerExercises.Count];
            player.InitCurrWordState("", pss.currExercise);
        }
    }

    
    
    
    
    
    
    [Server]
    private IEnumerator ChangeLobbyLevel()
    {
        foreach (LetterSpawner spawner in letterSpawners)
        {
            spawner.StopSpawning();
        }
        
        ChangeLobbyTargetWords();
        foreach (LetterSpawner spawner in letterSpawners)
        {
            ClearPool(spawner); //risky-> exercise may not have been synced yet
            spawner.UpdateCurrStarredWord("");
        }
        
        cmge.DisplayCountdownText(false, "");

        yield return new WaitForSeconds(5);
        
        foreach (LetterSpawner spawner in letterSpawners)
        {
            spawner.StartSpawning();
        }

        foreach (PlayerServerState pss in playerServerStates)
        {
            Player player = players[pss.orderNum];
            foreach (Button button in player.GetUI().GetComponentsInChildren<Button>())
            {
                //            player.ChangeAllButtonsColor(player.GetColor()); done internally to increase performance
                player.ResetButtonStates();
                player.EnableAllButtons();
            }
            
            //reset player actions in server
            pss.currNumPossibleActionsPerLevel = player.info.numPossibleActionsPerLevel;
            player.UpdateNumPossibleActions(pss.currNumPossibleActionsPerLevel);
            
            player.GetUI().GetComponentInChildren<Button>().onClick.Invoke(); //set track positions
        }
    }

    [Server]
    private void ChangeLobbyTargetWords()
    {
        Exercise newExercise = new Exercise();
        
        List<Exercise> selectedExerciseGroup = new List<Exercise>(Globals.settings.lobbyExercisesGroups
            .exerciseGroups[exerciseGroupIndex++ % Globals.settings.lobbyExercisesGroups.exerciseGroups.Count]
            .exercises);
        if (selectedExerciseGroup.Count <= 0)
        {
            Debug.Log("No exercises available");
        }

        int random = Random.Range(0, selectedExerciseGroup.Count);
        newExercise = selectedExerciseGroup[random];
    
        int i = Random.Range(0, 1);
        foreach (PlayerServerState pss in playerServerStates)
        {
            Player player = players[pss.orderNum];
            pss.currExercise = newExercise.playerExercises[(i++) % newExercise.playerExercises.Count];
            player.InitCurrWordState("", pss.currExercise);
        }
    }
    
    
    
    
    //--------------------- player actions ------------------------
    public void PlayerActionChanges()
    {
        foreach (PlayerServerState pss in playerServerStates)
        {
            Player player = players[pss.orderNum];
            if (player.ActionStarted() || player.ActionFinished())
            {
                int pressedButtonI = player.GetPressedButtonIndex();
                if (!player.IsButtonEnabled(pressedButtonI))
                {
                    continue;
                }

                if (player.ActionStarted())
                {
                    StartPlayerAction(pss, pressedButtonI);
                    player.AckActionStarted();
                }

                if (player.ActionFinished())
                {
                    FinishPlayerAction(pss, pressedButtonI);
                    player.AckActionFinished();
                }
            }
        }
    }
    
    [Server]
    public void FinishPlayerAction(PlayerServerState pss, int pressedButtonI)
    {
        Player player = players[pss.orderNum];
        cmge.PlayAudioClip(2, "Audio/clickUp");

        player.SetActiveInteraction(Globals.KeyInteractionType.NONE);
        player.ReleaseGameButton();
        
        player.ChangeButtonColor(pressedButtonI, player.GetButtonColor());
        player.UpdateActiveHalf(false);
    }


    [Server]
    public void StartPlayerAction(PlayerServerState pss, int pressedButtonI)
    {
        Player player = players[pss.orderNum];
        player.ChangeButtonColor(pressedButtonI, new Color(1.0f, 0.82f, 0.0f));
                
        //verify if button should be pressed
        bool playerOverlappedAndPressing = false;
        if (isLaneOverlap)
        {
            foreach (Player innerPlayer in players)
            {
                if (innerPlayer != player && player.IsPressingButton())
                {
                    playerOverlappedAndPressing = true;
                    break;
                }
            }
        }
        
        if (pss.currNumPossibleActionsPerLevel == 0 ||
            (isLaneOverlap && playerOverlappedAndPressing))
        {
            cmge.PlayAudioClip(2, "Audio/badMove");
            player.ChangeButtonColor(pressedButtonI, Color.red);
            
            return;
        }

        int j = pressedButtonI - 2;
        Globals.KeyInteractionType iType = (Globals.KeyInteractionType) j;
        
        cmge.PlayAudioClip(2, "Audio/clickDown");
        player.SetActiveInteraction(iType);
                
        player.PressGameButton();
        player.UpdateActiveHalf(true);
    }
    
    public GameObject bufferedFirstLetter; //so that server syncs clients properly
                                           //(avoids removing the same letter multiple times)
    [Server]
    public void CheckMarkerCollisions()
    {
        //verify collision letter-marker and letter-pit
        foreach (PlayerServerState pss in playerServerStates)
        {
            Player player = players[pss.orderNum];
            GameButton playerButton = player.GetGameButton();
            foreach (LetterSpawner spawner in letterSpawners)
            {
                GameObject firstLetter = spawner.GetCurrSpawnedLetterObjects().FirstOrDefault();
                if(firstLetter) 
                {
                    if (firstLetter!=bufferedFirstLetter && 
                        letterPit.GetComponent<Collider>().bounds.Intersects(
                        firstLetter.GetComponent<Collider>().bounds
                    ))
                    {
                        bufferedFirstLetter = firstLetter;
                        spawner.DestroyFirstLetter();
                    }else
                    if (firstLetter!=bufferedFirstLetter && 
                        playerButton.IsClicked() && 
                        playerButton.GetComponent<Collider>().bounds.Intersects(
                            firstLetter.GetComponent<Collider>().bounds
                    ))
                    {
                        bufferedFirstLetter = firstLetter;
                        RecordHit(firstLetter, pss);
                        spawner.DestroyFirstLetter();

                    }
                }
            }
        }

    }
    
     
    
    
    
    [Server]
    private bool TestAndExecuteHit(bool execute, char letterText, GameObject letterObj, PlayerServerState pss)
    {
        Player player = players[pss.orderNum];
        string currWordState = pss.currWordState;
        string currTargetWord = pss.currExercise.targetWord;

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

            pss.currWordState = currWordState;
//            letter.Lock();
//            letter.AnimateAndDestroy(player.GetWordPanel().transform.position);
        }

        return usefulForMe;
    }

   
    [Server]
    public void RecordHit(GameObject letterObj, PlayerServerState currHitterSS)
    {
        Player currHitter = players[currHitterSS.orderNum];
        
        Letter letter = letterObj.gameObject.GetComponent<Letter>();
        if (letter.IsLocked())
        {
            return;
        }

//        letter.Lock();
        char letterText = letter.letterText;
        letterObj.transform.localScale *= 1.2f;
        
        currHitterSS.currNumPossibleActionsPerLevel--;
        currHitter.UpdateNumPossibleActions(currHitterSS.currNumPossibleActionsPerLevel);
            
        currHitter.HideReadyButton();
        if (inLobby && currHitterSS.currNumPossibleActionsPerLevel == 0)
        {
            currHitter.ShowReadyButton();
        }
        
        //different rewards in different utility conditions
        bool usefulForMe = false;
        bool usefulForOther = false;

        Globals.KeyInteractionType playerIT = currHitter.GetActiveInteraction();
        List<ScoreValue> scores = new List<ScoreValue>();
        switch (playerIT)
        {
            case Globals.KeyInteractionType.GIVE:
                usefulForMe = TestAndExecuteHit(false, letterText, letterObj, currHitterSS);
                foreach (PlayerServerState usefulTargetPlayerSS in playerServerStates)
                {
                    if (usefulTargetPlayerSS.orderNum == currHitterSS.orderNum)
                    {
                        continue;
                    }

                    usefulForOther = TestAndExecuteHit(true, letterText, letterObj, usefulTargetPlayerSS);
                    if (usefulForOther)
                    {
                        break;
                    }

                }
                
                scores = currScoreSystem.giveScores;
                currHitter.info.numGives++;
                break;
            case Globals.KeyInteractionType.TAKE:
                usefulForMe = TestAndExecuteHit(true, letterText, letterObj, currHitterSS);

                foreach (PlayerServerState usefulTargetPlayerSS in playerServerStates)
                {
                    if (usefulTargetPlayerSS.orderNum == currHitterSS.orderNum)
                    {
                        continue;
                    }

                    if (usefulForOther)
                    {
                        break;
                    }

                    usefulForOther = TestAndExecuteHit(false, letterText, letterObj, usefulTargetPlayerSS);
                }

                scores = currScoreSystem.takeScores;
                currHitter.info.numTakes++;
                break;
        }

        if (usefulForMe || usefulForOther)
        {
            cmge.PlayAudioClip(1, "Audio/snap");
            cmge.SetEmojiAnim("Nice");
        }
        else
        {
            cmge.PlayAudioClip(1, "Audio/badMove");
        }

        float otherPlayersCompletionMean = 0;
        int otherPlayersCount = players.Count() - 1;
        float currPlayerCompletion = currHitterSS.currWordState.Count();
        foreach (PlayerServerState innerPlayerSS in playerServerStates)
        {
            if (innerPlayerSS.orderNum == currHitterSS.orderNum)
            {
                continue;
            }

            otherPlayersCompletionMean += (float) innerPlayerSS.currWordState.Count() / otherPlayersCount;
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
                currHitterSS.score += score.myValue;
                currHitter.SetScore(currHitterSS.score, score.myValue, 1.3f);
                foreach (PlayerServerState innerPlayerSS in playerServerStates)
                {
                    if (innerPlayerSS.orderNum == currHitterSS.orderNum)
                    {
                        continue;
                    }
                    innerPlayerSS.score += score.otherValue;
                    currHitter.SetScore(innerPlayerSS.score, score.otherValue, 1.3f);
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


//        if (currHitter.GetCurrNumPossibleActionsPerLevel() < 1)
//        {
//            foreach (Button button in currHitter.GetUI().GetComponentsInChildren<Button>())
//            {
//                button.GetComponent<Image>().color = Color.red;
//            }
//
//            currHitter.ReleaseGameButton();
//        }


        bool areWordsUnfinished = false;
        bool arePlayersWithoutActions = true;
        foreach (PlayerServerState pss in playerServerStates)
        {
            Player player = players[pss.orderNum];
            if (pss.currNumPossibleActionsPerLevel > 0)
            {
                arePlayersWithoutActions = false;
            }
            else
            {
                FinishPlayerAction(pss, player.GetPressedButtonIndex());
                //player.ChangeAllButtonsColor(Color.red); done internally to increase performance
                player.DisableAllButtons(Color.red);
            }

            string currWordState = pss.currWordState;
            string currTargetWord = pss.currExercise.targetWord;
            if (currWordState.CompareTo(currTargetWord) != 0)
            {
                if (!areWordsUnfinished)
                    areWordsUnfinished = true;

                if (!pss.currExerciseFinished)
                    pss.currExerciseFinished = true;
            }
            else
            {
                if (!pss.currExerciseFinished && pss.currNumPossibleActionsPerLevel >= 0)
                {
                    pss.score += currScoreSystem.completeWordMyScore;
                    player.SetScore(pss.score, currScoreSystem.completeWordMyScore, 1.3f);
                }
                foreach (PlayerServerState innerPlayerSS in playerServerStates)
                {
                    if (pss.orderNum == innerPlayerSS.orderNum)
                    {
                        continue;
                    }

                    if (!innerPlayerSS.currExerciseFinished && innerPlayerSS.currNumPossibleActionsPerLevel >= 0)
                    {
                        innerPlayerSS.score += currScoreSystem.completeWordOtherScore;
                        player.SetScore(innerPlayerSS.score, currScoreSystem.completeWordOtherScore, 1.3f);
                    }
                }
            }
        }

        if (!areWordsUnfinished || arePlayersWithoutActions)
        {
            if (inLobby)
            {
                StartCoroutine(ChangeLobbyLevel());
            }
            else
            {
                StartCoroutine(ChangeLevel(true, areWordsUnfinished));
            }
        }

    }
}


