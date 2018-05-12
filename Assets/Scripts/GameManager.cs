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
    public static Exercise currExercise { get; set; }

    public enum PlayerId
    {
        PLAYER_0,
        PLAYER_1
    }

    public enum ButtonId
    {
        BTN_0,
        BTN_1,
        NONE
    }

    public enum AntId
    {
        ANT_0,
        ANT_1
    }

    public static Dictionary<PlayerId, string[]> xboxInputKeyCodes = new Dictionary<PlayerId, string[]> {
        { PlayerId.PLAYER_0, new string[] { "YButtonJoy1", "BButtonJoy1" } },
        { PlayerId.PLAYER_1, new string[] { "YButtonJoy2", "BButtonJoy2" } }
    };

    public enum InputRestriction
    {
        BTN_0_ONLY,
        BTN_1_ONLY,
        NONE,
        ALL_BTNS
    }

    public enum InputMod
    {
        BTN_OPPOSITION,
        BTN_ALL_ACTIONS,
        NONE
    }


    public enum OutputRestriction
    {
        EAT,
        STARPOWER,
        NONE
    }

    public static InputRestriction[] inputRestrictions = (InputRestriction[]) Enum.GetValues(typeof(Utilities.InputRestriction));
    public static InputMod[] inputMods = (InputMod[]) Enum.GetValues(typeof(Utilities.InputMod));
    public static ButtonId[] buttonIds = (ButtonId[]) Enum.GetValues(typeof(Utilities.ButtonId));

    public static OutputRestriction[] outputRestrictions = (OutputRestriction[])Enum.GetValues(typeof(Utilities.OutputRestriction));

    public static int numInputRestrictions = inputRestrictions.Length;
    public static int numInputMods = inputMods.Length;

    public static int numOutputRestriction = outputRestrictions.Length;
}

public struct Exercise
{
    public string displayMessage;
    public string targetWord;
    private List<Utilities.InputRestriction> inputRestrictionsForEachPlayer;
    private List<Utilities.InputMod> inputModsForEachPlayer;
    private List<Utilities.OutputRestriction> outputRestrictionsForEachAnt;

    public Exercise(string displayMessage, string targetWord) : this()
    {
        this.displayMessage = displayMessage;
        this.targetWord = targetWord;

        inputRestrictionsForEachPlayer = new List<Utilities.InputRestriction>();
        inputModsForEachPlayer = new List<Utilities.InputMod>();

        outputRestrictionsForEachAnt = new List<Utilities.OutputRestriction>();

        //init restrictions for all players
        int numPlayers = 2;
        for (int i=0; i < numPlayers; i++)
        {
            inputRestrictionsForEachPlayer.Add(chooseInputRestriction());
            inputModsForEachPlayer.Add(chooseInputMod());

        }

        //init restrictions for all players
        int numAnts = 2;
        for (int i = 0; i < numAnts; i++)
        {
            outputRestrictionsForEachAnt.Add(chooseOutputRestriction());
        }

    }
    private Utilities.InputRestriction chooseInputRestriction()
    {
        int randomIndex = UnityEngine.Random.Range(0, Utilities.numInputRestrictions);
        return Utilities.inputRestrictions[randomIndex];

    }
    private Utilities.InputMod chooseInputMod()
    {
        int randomIndex = UnityEngine.Random.Range(0, Utilities.numInputMods);
        return Utilities.inputMods[randomIndex];

    }

    private Utilities.OutputRestriction chooseOutputRestriction()
    {
        int randomIndex = UnityEngine.Random.Range(0, Utilities.numOutputRestriction);
        return Utilities.outputRestrictions[randomIndex];
    }



    public List<Utilities.InputRestriction> getInputRestrictionsForEachPlayer()
    {
        return this.inputRestrictionsForEachPlayer;

    }
    public List<Utilities.InputMod> getInputModsForEachPlayer()
    {
        return this.inputModsForEachPlayer;

    }

    public List<Utilities.OutputRestriction> getOutputRestrictionsForEachAnt()
    {
        return this.outputRestrictionsForEachAnt;
    }

}

public class GameManager : MonoBehaviour
{
    
    public GameObject hpPanel;
    public GameObject displayPanel;
    public GameObject scorePanel;
    public GameObject timePanel;
    public GameObject reqPanel;
    
    public GameObject[] antSpawners;
    public GameObject[] letterSpawners;


    public string currWord;

    public float timeLeft;
    //public float timeLeft = -1.0f;


    public int lives;
    private int score;
    private List<Exercise> exercises;

    // Use this for initialization
    void Start()
    {
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

        lives = 5;

        InvokeRepeating("decrementTimeLeft", 0.0f, 1.0f);
        Application.targetFrameRate = 60;

        timeLeft = 100.0f;

        changeTargetWord();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        hpPanel.GetComponent<UnityEngine.UI.Text>().text = "Lifes: "+ lives;
        scorePanel.GetComponent<UnityEngine.UI.Text>().text = "Score: "+ score;
        timePanel.GetComponent<UnityEngine.UI.Text>().text = "Time: "+ timeLeft;

        //update curr display message
        int missingLength = Utilities.currExercise.targetWord.Length - currWord.Length;
        string[] substrings = Utilities.currExercise.displayMessage.Split('_');

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



        currWord = currWord.ToUpper();



        if (timeLeft == 0.0f)
        {
            changeTargetWord();
            timeLeft = 100.0f;
            if (lives < 1)
            {
                SceneManager.LoadScene("gameover");
            }
        }

        string currTargetWord = Utilities.currExercise.targetWord;
        //if(currTargetWord.Length > currWord.Length)
        //{
        //    currTargetWord = currTargetWord.Substring(0, currWord.Length);
        //}

        if (currWord == ""){
            return;
        }
        if (currTargetWord[currWord.Length-1]!=currWord[currWord.Length - 1])
        {
            hurt();
            currWord = currWord.Remove(currWord.Length - 1);
            return;
        }
        
        if (currWord.CompareTo(currTargetWord) == 0)
        {
            score += currTargetWord.Length;
            //timeLeft += currTargetWord.Length*4;
            timeLeft = 100.0f;
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
                changeTargetWord();
            }
            

        }
    }

    void decrementTimeLeft()
    {
        timeLeft--;
        //timeLeft = (timeLeft > 0) ? timeLeft-- : timeLeft;
    }

    void changeTargetWord()
    {
        int random = UnityEngine.Random.Range(0, exercises.Count);
        Utilities.currExercise = exercises[random];

        currWord = "";

        //int randomIndex = UnityEngine.Random.Range(0, 400);
        //UnityWebRequest currPoke = UnityWebRequest.Get("http://pokeapi.co/api/v2/pokemon/"+randomIndex+"/");
        //currExercise.targetWord = currPoke.name;
        //currExercise.displayMessage = "Word to match: "+ currPoke.name+ " \n Your Word: _";

        displayPanel.GetComponent<DisplayPanel>().setTargetImage(Utilities.currExercise.targetWord);
    }

    void hurt()
    {
        lives--;
    }
}
