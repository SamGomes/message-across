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

    public enum PlayerInputRestriction
    {
        BTN_0_ONLY,
        BTN_1_ONLY,
        NONE
        //ALL_BTNS
    }

    public enum PlayerInputMod
    {
        BTN_EXCHANGE,
        BTN_ALL_ACTIONS,
        NONE
    }

    public enum GlobalInputMod
    {
        BTN_MIXEDINPUT,
        NONE
    }

    public enum OutputRestriction
    {
        EAT,
        STARPOWER,
        NONE
    }

    public static PlayerInputRestriction[] playerInputRestrictions = (PlayerInputRestriction[]) Enum.GetValues(typeof(Utilities.PlayerInputRestriction));
    public static PlayerInputMod[] playerInputMods = (PlayerInputMod[]) Enum.GetValues(typeof(Utilities.PlayerInputMod));
    public static GlobalInputMod[] globalInputMods = (GlobalInputMod[]) Enum.GetValues(typeof(Utilities.GlobalInputMod));
    public static ButtonId[] buttonIds = (ButtonId[]) Enum.GetValues(typeof(Utilities.ButtonId));

    public static OutputRestriction[] outputRestrictions = (OutputRestriction[])Enum.GetValues(typeof(Utilities.OutputRestriction));

    public static int numPlayerInputRestrictions = playerInputRestrictions.Length;
    public static int numPlayerInputMods = playerInputMods.Length;
    public static int numGlobalInputMods = globalInputMods.Length;

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

    public GameObject playersPanel;

    public GameObject[] antSpawners;
    public GameObject[] letterSpawners;
    public List<Button> gameButtons;

    public string currWord;

    public float timeLeft;
    //public float timeLeft = -1.0f;


    public int lifes;
    private int score;
    private List<Exercise> exercises;

    private List<Player> players;
    private List<Utilities.PlayerId> lastPlayersToPressIndexes;

    private List<Utilities.OutputRestriction> prevAntOutputs;
    public Utilities.GlobalInputMod currGlobalInputMod;

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
        players.Add(new Player(Utilities.PlayerId.PLAYER_0));
        players.Add(new Player(Utilities.PlayerId.PLAYER_1));

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

        Application.targetFrameRate = 60;

        timeLeft = 100.0f;
        InvokeRepeating("decrementTimeLeft", 0.0f, 1.0f);

        changeTargetWord();
    
    }

    // Update is called once per frame
    void FixedUpdate()
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
            playersPanel.transform.GetComponentsInChildren<UnityEngine.UI.Text>()[i].text = "Score of player "+i+": " + players[i].score;
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
            Application.ExternalEval("window.open('https://docs.google.com/forms/d/e/1FAIpQLSeM3Xn5qDBdX7QCtyrPILLbqpYj3ueDcLa_-9CbxCPzxVsMzg/viewform?usp=pp_url&entry.2097900814=" + player.id + "&entry.159491668=" + (int)this.currGlobalInputMod + "&entry.1857287181=" + (int)player.inputRestriction + "&entry.978719613=" + (int)player.inputMod + "&entry.1620449534=" + (int)prevAntOutputs[players.IndexOf(player)] + "');"); //spawn questionaires
        }
    }

    void changeTargetWord()
    {

        //init restrictions for all players
        for (int i = 0; i < players.Count; i++) 
        {
            players[i].inputRestriction = Utilities.PlayerInputRestriction.NONE; //choosePlayerInputRestriction(); Utilities.PlayerInputRestriction.BTN_0_ONLY;
            players[i].inputMod = choosePlayerInputMod(); //Utilities.PlayerInputMod.NONE; 
        }
        currGlobalInputMod = chooseGlobalInputMod(); //Utilities.GlobalInputMod.BTN_MIXEDINPUT;
        
        prevAntOutputs = new List<Utilities.OutputRestriction>();


        int random = UnityEngine.Random.Range(0, exercises.Count);
        this.currExercise = exercises[random];

        string targetWord = this.currExercise.targetWord;
        track.GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("Textures/track", typeof(Sprite));

        //init restrictions for all players
        for (int i = 0; i < antSpawners.Length; i++)
        {
            Utilities.OutputRestriction currOutputRestriction = antSpawners[i].GetComponent<AntSpawner>().outputRestriction;
            prevAntOutputs.Add(currOutputRestriction);
            
            string starredWord = "";
            if (currOutputRestriction==Utilities.OutputRestriction.STARPOWER)
            {
                //change the track on star power
                track.GetComponent<SpriteRenderer>().sprite = (Sprite)Resources.Load("Textures/starTrack", typeof(Sprite));
                starredWord = targetWord;
            }
            letterSpawners[i%antSpawners.Length].GetComponent<LetterSpawner>().updateCurrStarredtWord(starredWord);

            antSpawners[i].GetComponent<AntSpawner>().outputRestriction = chooseOutputRestriction(); //Utilities.OutputRestriction.STARPOWER;
        }

        currWord = "";

        displayPanel.GetComponent<DisplayPanel>().setTargetImage(targetWord);

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
        }
    }


    private Utilities.PlayerInputRestriction choosePlayerInputRestriction()
    {
        int randomIndex = UnityEngine.Random.Range(0, Utilities.numPlayerInputRestrictions);
        return Utilities.playerInputRestrictions[randomIndex];

    }
    private Utilities.PlayerInputMod choosePlayerInputMod()
    {
        int randomIndex = UnityEngine.Random.Range(0, Utilities.numPlayerInputMods);
        return Utilities.playerInputMods[randomIndex];

    }

    private Utilities.GlobalInputMod chooseGlobalInputMod()
    {
        int randomIndex = UnityEngine.Random.Range(0, Utilities.numGlobalInputMods);
        return Utilities.globalInputMods[randomIndex];

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


