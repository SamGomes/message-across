using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public static class Utilities
{
    public enum PlayerId
    {
        PLAYER_0,
        PLAYER_1,
        NONE
    }

    public enum ButtonId
    {
        BTN_0,
        BTN_1,
        NONE
    }

    public enum PlayersToPressButtonAlternative
    {
        SINGLE_PLAYER,
        MULTIPLAYER_COMBO
    }

    public enum MultiplayerKeysAlternative
    {
        K_2 = 2
        //,K_3 = 3

    }
    public enum OutputRestriction
    {
        EAT,
        STARPOWER,
        NONE
    }

    public static PlayersToPressButtonAlternative[] playersToPressButtonAlternatives = (PlayersToPressButtonAlternative[]) Enum.GetValues(typeof(Utilities.PlayersToPressButtonAlternative));
    public static MultiplayerKeysAlternative[] multiplayerKeysAlternatives = (MultiplayerKeysAlternative[]) Enum.GetValues(typeof(Utilities.MultiplayerKeysAlternative));

    public static ButtonId[] buttonIds = (ButtonId[]) Enum.GetValues(typeof(Utilities.ButtonId));

    public static OutputRestriction[] outputRestrictions = (OutputRestriction[])Enum.GetValues(typeof(Utilities.OutputRestriction));

    public static int numPlayersToPressButtonAlternatives = playersToPressButtonAlternatives.Length;
    public static int numMultiplayerKeysAlternatives = multiplayerKeysAlternatives.Length;

    public static int numOutputRestriction = outputRestrictions.Length;
}


public class GameManager : MonoBehaviour
{

    public GameSceneManager gameSceneManager;

    public bool isGameplayPaused;

    public GameObject hpPanel;
    public GameObject displayPanel;
    public GameObject scorePanel;
    public GameObject timePanel;
    public GameObject reqPanel;
    public GameObject track;

    public InputManager inputManager;


    public GameObject playersPanel;
    public GameObject[] antSpawners;
    public GameObject[] letterSpawners;
    public List<Button> gameButtons;

    public string currWord;

    public float timeLeft;

    public int lifes;
    private int score;
    private List<Exercise> exercises;

    private List<Player> players;
    private List<Utilities.PlayerId> lastPlayersToPressIndexes;

    private List<Utilities.OutputRestriction> prevAntOutputs;
    public Utilities.PlayersToPressButtonAlternative currNumPlayersCombo;

    public Exercise currExercise { get; set; }

    public void pauseGame()
    {
        Time.timeScale = 0;
        isGameplayPaused = true;
    }

    public void resumeGame()
    {
        Time.timeScale = 1;
        isGameplayPaused = false;
    }


    public void PL1NameBoxTextChanged(string newText)
    {
        players[0].setName(newText);
    }
    public void PL2NameBoxTextChanged(string newText)
    {
        players[1].setName(newText);
    }

    public void initKeys()
    {
        inputManager.addKeyBinding(new KeyCode[] { KeyCode.Space }, InputManager.ButtonPressType.DOWN, delegate () { gameSceneManager.startAndPauseGame(Utilities.PlayerId.NONE); });
        inputManager.addButtonBinding(new string[] { "Start" }, InputManager.ButtonPressType.DOWN, delegate () { gameSceneManager.startAndPauseGame(Utilities.PlayerId.NONE); });



        //    inputManager.addKeyBinding(new KeyCode[] { KeyCode.Q }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        //    inputManager.addKeyBinding(new KeyCode[] { KeyCode.W }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        //    inputManager.addKeyBinding(new KeyCode[] { KeyCode.O }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
        //    inputManager.addKeyBinding(new KeyCode[] { KeyCode.P }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });

        //    inputManager.addButtonBinding(new string[] { "YButtonJoy1" }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        //    inputManager.addButtonBinding(new string[] { "BButtonJoy1" }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        //    inputManager.addButtonBinding(new string[] { "YButtonJoy2" }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
        //    inputManager.addButtonBinding(new string[] { "BButtonJoy2" }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
        //
    }

    // Use this for initialization
    void Start()
    {

        lastPlayersToPressIndexes = new List<Utilities.PlayerId>();

        isGameplayPaused = false;
        gameSceneManager.mainSceneLoadedNotification();

        prevAntOutputs = new List<Utilities.OutputRestriction>(2);
        prevAntOutputs.Add(Utilities.OutputRestriction.NONE);
        prevAntOutputs.Add(Utilities.OutputRestriction.NONE);

        players = new List<Player>();
        players.Add(new Player(Utilities.PlayerId.PLAYER_0, new KeyCode[] { KeyCode.Q, KeyCode.W }, new string[] { "YButtonJoy1" , "BButtonJoy1" }));
        players.Add(new Player(Utilities.PlayerId.PLAYER_1, new KeyCode[] { KeyCode.O, KeyCode.P }, new string[] { "YButtonJoy2" , "BButtonJoy2" }));


        exercises = new List<Exercise>();
        exercises.Add(new Exercise("Word to match: CAKE \n Your Word:_", "CAKE"));
        exercises.Add(new Exercise("Word to match: BANANA \n Your Word:_", "BANANA"));
        exercises.Add(new Exercise("Word to match: PIE \n Your Word:_", "PIE"));
        exercises.Add(new Exercise("Word to match: PIZZA \n Your Word:_", "PIZZA"));
        exercises.Add(new Exercise("Word to match: CROISSANT \n Your Word:_", "CROISSANT"));
        exercises.Add(new Exercise("Word to match: DONUT \n Your Word:_", "DONUT"));
        exercises.Add(new Exercise("Word to match: CHERRY \n Your Word:_", "CHERRY"));
        exercises.Add(new Exercise("Word to match: XMASCOOKIES \n Your Word:_", "XMASCOOKIES"));
        exercises.Add(new Exercise("Word to match: KIWI \n Your Word:_", "KIWI"));
        exercises.Add(new Exercise("Word to match: QUICHE \n Your Word:_", "QUICHE"));
        exercises.Add(new Exercise("Word to match: MANGO \n Your Word:_", "MANGO"));
        exercises.Add(new Exercise("Word to match: FISH \n Your Word:_", "FISH"));
        exercises.Add(new Exercise("Word to match: VANILLA \n Your Word:_", "VANILLA"));
        exercises.Add(new Exercise("Word to match: JELLY \n Your Word:_", "JELLY"));

        lifes = 50;

        //Application.targetFrameRate = 60;

        timeLeft = 100.0f;
        InvokeRepeating("decrementTimeLeft", 0.0f, 1.0f);

        changeTargetWord();
        changeGameParametrizations();

        gameSceneManager.startAndPauseGame(Utilities.PlayerId.PLAYER_0); //for the initial screen
    
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameplayPaused)
        {
            return;
        }

        //if no lifes end game immediately
        if (lifes < 1)
        {
            gameSceneManager.endGame();
        }

        //if time's up change word
        if (timeLeft == 0.0f)
        {
            poppupQuestionnaires();
            changeTargetWord();
            timeLeft = 100.0f;
            hurt();
        }

        hpPanel.GetComponent<UnityEngine.UI.Text>().text = "Lifes: "+ lifes;
        scorePanel.GetComponent<UnityEngine.UI.Text>().text = "Team Score: "+ score;
        timePanel.GetComponent<UnityEngine.UI.Text>().text = "Time: "+ timeLeft;

        for(int i=0; i < players.Count; i++)
        {
            playersPanel.transform.GetComponentsInChildren<UnityEngine.UI.Text>()[i].text = "Player "+players[i].getName()+" Score: " + players[i].score;
        }

        //update curr display message
        int missingLength = this.currExercise.targetWord.Length - currWord.Length;
        string[] substrings = this.currExercise.displayMessage.Split('_');

        string displayString = "";
        if (substrings.Length > 0)
        {
            displayString = substrings[0];
            displayString += currWord;
            for (int i = 0; i < missingLength; i++)
            {
                displayString += "_";
            }
            if (substrings.Length > 1)
            {
                displayString += substrings[1];
            }
        }
        reqPanel.GetComponent<reqScript>().updateRequirement(displayString);

    }

    void decrementTimeLeft()
    {
        if(timeLeft > 0.0f){
            timeLeft--; 
        }
    }

    void poppupQuestionnaires()
    {
        gameSceneManager.pauseForQuestionnaires(Utilities.PlayerId.NONE);
        //spawn questionnaires before changing word
        foreach (Player player in players)
        {
            //Application.ExternalEval("window.open('https://docs.google.com/forms/d/e/1FAIpQLSeM3Xn5qDBdX7QCtyrPILLbqpYj3ueDcLa_-9CbxCPzxVsMzg/viewform?usp=pp_url&entry.100873100="+ player.name + "&entry.2097900814=" + player.id + "&entry.631185473=" + currExercise.targetWord + "&entry.159491668=" + (int)this.currNumPlayersCombo + "&entry.978719613=" + (int)player.inputMod + "&entry.1620449534=" + (int)prevAntOutputs[players.IndexOf(player)] + "');"); //spawn questionaires
        }
    }

    void changeGameParametrizations()
    {
        this.currNumPlayersCombo = chooseNumPlayersCombo();
        Debug.Log("currNumPlayersCombo: " + currNumPlayersCombo);

        //could be more efficient
        inputManager.removeAllKeyBindings();
        initKeys();

        if (this.currNumPlayersCombo == Utilities.PlayersToPressButtonAlternative.SINGLE_PLAYER)
        {
            foreach (Player player in this.players)
            {
                List<KeyCode> unassignedPlayerKeys = new List<KeyCode>(player.getMyKeys());
                foreach (Button button in this.gameButtons)
                {
                    int randomIndex = UnityEngine.Random.Range(0, unassignedPlayerKeys.Count);
                    KeyCode currCode = unassignedPlayerKeys[randomIndex];
                    unassignedPlayerKeys.RemoveAt(randomIndex);
                    inputManager.addKeyBinding(new KeyCode[] { currCode }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)button.buttonCode].registerUserButtonPress(new Utilities.PlayerId[] { player.getId() }); });
                }
            }
        }
        else if (this.currNumPlayersCombo == Utilities.PlayersToPressButtonAlternative.MULTIPLAYER_COMBO)
        {
            foreach (Button button in this.gameButtons)
            {
                int randomNumKeysToPressIndex = UnityEngine.Random.Range(0, Utilities.numMultiplayerKeysAlternatives);
                Utilities.MultiplayerKeysAlternative numKeysToPress = Utilities.multiplayerKeysAlternatives[randomNumKeysToPressIndex];

                List<KeyCode> buttonKeyCombo = new List<KeyCode>();
                List<List< KeyCode >> buttonKeyCombos = new List<List<KeyCode>>();
                List<Utilities.PlayerId> playersPressingThisButton = new List<Utilities.PlayerId>();

                int currPlayerIndex = 0;
                while (buttonKeyCombo.Count < (int)numKeysToPress)
                {
                    KeyCode currCode = KeyCode.A;
                    bool codeAlreadyExists = true;
                    while (codeAlreadyExists)
                    {
                        Player currPlayer = this.players[currPlayerIndex % this.players.Count];
                        playersPressingThisButton.Add(currPlayer.getId());
                        KeyCode[] playerKeys = currPlayer.getMyKeys();
                        int randomIndex = UnityEngine.Random.Range(0, playerKeys.Length);
                        currCode = playerKeys[randomIndex];
                        codeAlreadyExists = buttonKeyCombos.Contains(buttonKeyCombo);
                    }
                    buttonKeyCombo.Add(currCode);
                    currPlayerIndex++;
                }
                inputManager.addKeyBinding(buttonKeyCombo.ToArray(), InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)button.buttonCode].registerUserButtonPress(playersPressingThisButton.ToArray());  });
            }
        }
       

        prevAntOutputs = new List<Utilities.OutputRestriction>();
        track.GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("Textures/track", typeof(Sprite));

        string targetWord = this.currExercise.targetWord;
        //init restrictions for all players
        for (int i = 0; i < antSpawners.Length; i++)
        {
            Utilities.OutputRestriction currOutputRestriction = antSpawners[i].GetComponent<AntSpawner>().outputRestriction;
            prevAntOutputs.Add(currOutputRestriction);

            string starredWord = "";
            if (currOutputRestriction == Utilities.OutputRestriction.STARPOWER)
            {
                //change the track on star power
                track.GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("Textures/starTrack", typeof(Sprite));
                starredWord = targetWord;
            }
            letterSpawners[i % antSpawners.Length].GetComponent<LetterSpawner>().updateCurrStarredtWord(starredWord);

            antSpawners[i].GetComponent<AntSpawner>().outputRestriction = Utilities.OutputRestriction.STARPOWER; // chooseOutputRestriction(); //Utilities.OutputRestriction.STARPOWER;
        }
    }

    void changeTargetWord()
    {
        int random = UnityEngine.Random.Range(0, exercises.Count);
        Exercise newExercise = exercises[random];

        currWord = "";
        displayPanel.GetComponent<DisplayPanel>().setTargetImage(newExercise.targetWord);

        this.currExercise = newExercise;
    }

    void hurt()
    {
        lifes--;
    }

    public void recordHit(List<Utilities.PlayerId> hitters, char letterText)
    {
        this.lastPlayersToPressIndexes = hitters;

        this.currWord += letterText;
        this.currWord = this.currWord.ToUpper();
        string currTargetWord = this.currExercise.targetWord;


        if (currWord.Length <= currTargetWord.Length && currTargetWord[currWord.Length - 1] == currWord[currWord.Length - 1])
        {
            score += 100;
            foreach (Utilities.PlayerId playerId in lastPlayersToPressIndexes)
            {
                players[(int)playerId].score += 50;
            }
        }
        else
        {
            hurt();
            currWord = currWord.Remove(currWord.Length - 1);
            return;
        }
        

        if (currWord.CompareTo(currTargetWord) == 0)
        {
            //timeLeft += currTargetWord.Length*4;
            timeLeft = 100.0f;

            //init track and play ant anims
            GameObject[] letters = GameObject.FindGameObjectsWithTag("letter");
            foreach (GameObject letter in letters)
            {
                Destroy(letter);
            }
            foreach (GameObject letterSpawner in letterSpawners)
            {
                letterSpawner.GetComponent<LetterSpawner>().setScore(score);
            }
            foreach (GameObject antSpawner in antSpawners)
            {
                AntSpawner spawner = antSpawner.GetComponent<AntSpawner>();
                spawner.spawnAnt(currTargetWord);
            }

            poppupQuestionnaires();
            changeTargetWord();
            changeGameParametrizations();

        }
    }


    private Utilities.PlayersToPressButtonAlternative chooseNumPlayersCombo()
    {
        int randomIndex = UnityEngine.Random.Range(0, Utilities.numPlayersToPressButtonAlternatives);
        return Utilities.playersToPressButtonAlternatives[randomIndex];

    }
    private Utilities.OutputRestriction chooseOutputRestriction()
    {
        int randomIndex = UnityEngine.Random.Range(0, Utilities.numOutputRestriction);
        return Utilities.outputRestrictions[randomIndex];
    }


    public List<Player> getPlayers()
    {
        return this.players;
    }


}


