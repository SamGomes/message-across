using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Utilities
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

    public static PlayersToPressButtonAlternative[] playersToPressButtonAlternatives = (PlayersToPressButtonAlternative[]) Enum.GetValues(typeof(Utilities.PlayersToPressButtonAlternative));

    public static ButtonId[] buttonIds = (ButtonId[]) Enum.GetValues(typeof(Utilities.ButtonId));
    

    public static int numPlayersToPressButtonAlternatives = playersToPressButtonAlternatives.Length;
    public static int simultaneousKeysToPress = 2;
}


public class GameManager : MonoBehaviour
{

    public GameSceneManager gameSceneManager;

    public bool isGameplayPaused;

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
    private int score;
    private List<Exercise> exercises;

    private List<Player> players;
    
    public Utilities.PlayersToPressButtonAlternative currNumPlayersCombo;

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

    // Use this for initialization
    void Start()
    {
        //Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSeM3Xn5qDBdX7QCtyrPILLbqpYj3ueDcLa_-9CbxCPzxVsMzg/viewform?usp=pp_url&entry.100873100=AAA&entry.631185473=" + currWordState + "&entry.159491668=" + currNumPlayersCombo + "&entry.1252688229=" + 20 + "&entry.1140424083=" + 30); //spawn questionaires



        isGameplayPaused = false;
        gameSceneManager.MainSceneLoadedNotification();

        players = new List<Player>();
        players.Add(new Player(new HashSet<KeyCode>(){ KeyCode.Q, KeyCode.W, KeyCode.E }, new HashSet<string> { "YButtonJoy1" , "BButtonJoy1" }));
        players.Add(new Player(new HashSet<KeyCode>() { KeyCode.I, KeyCode.O, KeyCode.P }, new HashSet<string> { "YButtonJoy2" , "BButtonJoy2" }));
        //players.Add(new Player(Utilities.PlayerId.PLAYER_2, new KeyCode[] { KeyCode.V, KeyCode.B, KeyCode.N }, new string[] { "YButtonJoy3" , "BButtonJoy3" }));


        exercises = new List<Exercise>();
        exercises.Add(new Exercise("Fck yourself:_", "HFESUIHFUESIGHUFISEHUFESIHFESI"));
        //exercises.Add(new Exercise("Word to match: CAKE \n Your Word:_", "CAKE"));
        //exercises.Add(new Exercise("Word to match: BANANA \n Your Word:_", "BANANA"));
        //exercises.Add(new Exercise("Word to match: PIE \n Your Word:_", "PIE"));
        //exercises.Add(new Exercise("Word to match: PIZZA \n Your Word:_", "PIZZA"));
        //exercises.Add(new Exercise("Word to match: CROISSANT \n Your Word:_", "CROISSANT"));
        //exercises.Add(new Exercise("Word to match: DONUT \n Your Word:_", "DONUT"));
        //exercises.Add(new Exercise("Word to match: CHERRY \n Your Word:_", "CHERRY"));
        //exercises.Add(new Exercise("Word to match: XMASCOOKIES \n Your Word:_", "XMASCOOKIES"));
        //exercises.Add(new Exercise("Word to match: KIWI \n Your Word:_", "KIWI"));
        //exercises.Add(new Exercise("Word to match: QUICHE \n Your Word:_", "QUICHE"));
        //exercises.Add(new Exercise("Word to match: MANGO \n Your Word:_", "MANGO"));
        //exercises.Add(new Exercise("Word to match: FISH \n Your Word:_", "FISH"));
        //exercises.Add(new Exercise("Word to match: VANILLA \n Your Word:_", "VANILLA"));
        //exercises.Add(new Exercise("Word to match: JELLY \n Your Word:_", "JELLY"));

        lifes = 50;

        //Application.targetFrameRate = 60;

        timeLeft = 100.0f;
        InvokeRepeating("DecrementTimeLeft", 0.0f, 1.0f);

        ChangeTargetWord();
        ChangeGameParametrizations(true);

        gameSceneManager.StartAndPauseGame(); //for the initial screen


        for (int i = 0; i < players.Count; i++)
        {
            Player currPlayer = players[i];
            GameObject newNameInput = Instantiate(playerNameInputFieldPrefabRef,namesInputLocation.transform);
            InputField input = newNameInput.GetComponent<InputField>();
            input.placeholder.GetComponent<Text>().text = OrderedFormOfNumber(i+1)+" player name";


            input.onValueChanged.AddListener(delegate
            {
                List<InputField> inputFields = new List<InputField>(namesInputLocation.GetComponentsInChildren<InputField>());
                players[inputFields.IndexOf(input)].SetName(input.text);
            });
        }
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
        scorePanel.GetComponent<UnityEngine.UI.Text>().text = "Team Score: "+ score;
        timePanel.GetComponent<UnityEngine.UI.Text>().text = "Time: "+ timeLeft;


        UnityEngine.UI.Text playerPanelText = playersPanel.transform.GetComponentInChildren<UnityEngine.UI.Text>();
        playerPanelText.text = "";
        for (int i=0; i < players.Count; i++)
        {
            playerPanelText.text += "\n Player "+players[i].GetName()+" Score: " + players[i].score;
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
        foreach (Player player in players)
        {
            int totalButtonHits = player.mybuttonHits + player.simultaneousButtonHits;
            float playerHitsPercentage = ((float) player.mybuttonHits /(float) totalButtonHits) * 100.0f;
            float simultaneousHitsPercentage = ((float)player.simultaneousButtonHits / (float)totalButtonHits) * 100.0f;
            Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSeM3Xn5qDBdX7QCtyrPILLbqpYj3ueDcLa_-9CbxCPzxVsMzg/viewform?usp=pp_url&entry.100873100="+player.GetName()+"&entry.631185473="+currWordState+"&entry.159491668="+ currNumPlayersCombo + "&entry.1252688229="+ playerHitsPercentage +"&entry.1140424083="+ simultaneousHitsPercentage); //spawn questionaires
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
        
        int numKeysToPress = Utilities.simultaneousKeysToPress;
        
        //choose num players combo
        int randomAltIndex = UnityEngine.Random.Range(0, Utilities.numPlayersToPressButtonAlternatives);
        Utilities.PlayersToPressButtonAlternative chosenAlternative = Utilities.playersToPressButtonAlternatives[randomAltIndex];

        
        foreach (Button button in this.gameButtons)
        {
            this.currNumPlayersCombo = firstTimeCall ? Utilities.PlayersToPressButtonAlternative.SINGLE_PLAYER : chosenAlternative;
            
            if (this.currNumPlayersCombo == Utilities.PlayersToPressButtonAlternative.SINGLE_PLAYER)
            {
                foreach (Player player in this.players)
                {
                    HashSet<KeyCode> generatedKeyCombo = GenerateKeyCombo(player.GetMyKeys(), numKeysToPress);
                    List<Player> selectedKeysPlayers = new List<Player>(){ player };
                    inputManager.AddKeyBinding(
                        generatedKeyCombo, InputManager.ButtonPressType.PRESSED, delegate ()
                        {
                            gameButtons[(int)button.buttonCode].RegisterUserButtonPress(selectedKeysPlayers);
                        });
                }
            }
            else if (this.currNumPlayersCombo == Utilities.PlayersToPressButtonAlternative.MULTIPLAYER)
            {
                List<Player> selectedKeysPlayers = new List<Player>();
                HashSet<KeyCode> possibleKeys = new HashSet<KeyCode>();
                for (int i = 0; i < numKeysToPress; i++)
                {
                    Player selectedPlayer = players[i % players.Count];
                    selectedKeysPlayers.Add(selectedPlayer);

                    int randomIndex = UnityEngine.Random.Range(0, selectedPlayer.GetMyKeys().Count);
                    possibleKeys.Add(selectedPlayer.GetMyKeys().ElementAt(randomIndex));
                }

                HashSet<KeyCode> generatedKeyCombo = GenerateKeyCombo(possibleKeys, numKeysToPress);
                inputManager.AddKeyBinding(
                    generatedKeyCombo, InputManager.ButtonPressType.PRESSED, delegate ()
                    {
                        gameButtons[(int)button.buttonCode].RegisterUserButtonPress(selectedKeysPlayers);
                    });
            }



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

        }

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
        int random = UnityEngine.Random.Range(0, exercises.Count);
        Exercise newExercise = exercises[random];

        currWordState = "";
        displayPanel.GetComponent<DisplayPanel>().SetTargetImage(newExercise.targetWord);

        this.currExercise = newExercise;
    }

    void Hurt()
    {
        lifes--;
    }

    public void RecordHit(List<Player> hitters, char letterText)
    {
        this.currWordState += letterText;
        this.currWordState = this.currWordState.ToUpper();
        string currTargetWord = this.currExercise.targetWord;


        if (currWordState.Length <= currTargetWord.Length && currTargetWord[currWordState.Length - 1] == currWordState[currWordState.Length - 1])
        {
            score += 100;
            foreach (Player player in hitters)
            {
                player.AddHitToStatistics(hitters);
                player.score += 50;
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
            foreach (LetterSpawner letterSpawner in letterSpawners)
            {
                letterSpawner.SetScore(score);
            }
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
        return this.players;
    }


}


