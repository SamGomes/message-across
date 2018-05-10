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

    public enum InputRestriction
    {
        BTN_0_ONLY,
        BTN_1_ONLY,
        NONE,
        ALL_BTNS,

        BTN_EXCHANGE,
        BTN_ALL_ACTIONS
    }


    public enum OutputRestriction
    {
        ANT1_ANIM1,
        ANT1_ANIM2,
        ANT1_ANIM3
    }

    public static InputRestriction[] inputRestrictions = (InputRestriction[]) Enum.GetValues(typeof(Utilities.InputRestriction));
    public static OutputRestriction[] outputRestrictions = (OutputRestriction[])Enum.GetValues(typeof(Utilities.OutputRestriction));

    public static int numInputRestrictions = inputRestrictions.Length;
    public static int numOutputRestriction = outputRestrictions.Length;
}

public struct Exercise
{
    public string displayMessage;
    public string targetWord;
    private List<Utilities.InputRestriction> inputRestrictionsForEachPlayer;
    private Utilities.OutputRestriction outputRestriction;

    public Exercise(string displayMessage, string targetWord) : this()
    {
        this.displayMessage = displayMessage;
        this.targetWord = targetWord;

        inputRestrictionsForEachPlayer = new List<Utilities.InputRestriction>();

        //init restrictions for all players
        int numPlayers = 2;
        for (int i=0; i < numPlayers; i++)
        {
            inputRestrictionsForEachPlayer.Add(Utilities.InputRestriction.BTN_0_ONLY);

        }
        this.outputRestriction = chooseOutputRestriction();
    }
    private Utilities.InputRestriction chooseInputRestriction()
    {
        int randomIndex = UnityEngine.Random.Range(0, Utilities.numInputRestrictions);
        return Utilities.inputRestrictions[randomIndex];

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

    private Utilities.OutputRestriction getOutputRestriction()
    {
        return this.outputRestriction;
    }

}

public class GameManager : MonoBehaviour
{
    
    public GameObject hpPanel;
    public GameObject displayPanel;
    public GameObject scorePanel;
    public GameObject timePanel;
    public GameObject reqPanel;
    
    public GameObject Spawner;
    public GameObject[] LetterSpawners;


    public string currWord;

    public float timeLeft = 30.0f;


    public int lives = 9000;
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


        InvokeRepeating("decrementTimeLeft", 0.0f, 1.0f);
        Application.targetFrameRate = 60;
        //timeLeft = 30.0f;

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



        if (timeLeft <= 0.0f || lives < 1)
        {
            SceneManager.LoadScene("gameover");
        }

        string currTargetWord = Utilities.currExercise.targetWord;
        //if(currTargetWord.Length > currWord.Length)
        //{
        //    currTargetWord = currTargetWord.Substring(0, currWord.Length);
        //}

        if (currWord == ""){
            return;
        }
        if (!currTargetWord.Contains(currWord))
        {
            hurt();
            return;
        }
        
        if (currWord.CompareTo(currTargetWord) == 0)
        {
            score += currTargetWord.Length;
            timeLeft += currTargetWord.Length*4;
            GameObject[] letters = GameObject.FindGameObjectsWithTag("letter");
            foreach (GameObject letter in letters)
            {
                Destroy(letter);
            }
            foreach (GameObject LetterSpawner in LetterSpawners)
            {
                LetterSpawner.GetComponent<LetterSpawner>().setScore(score);   
            }
            AntSpawner spawner = Spawner.GetComponent<AntSpawner>();
            Debug.Log(currTargetWord);
            spawner.spawnAnt(currTargetWord);
            changeTargetWord();

        }
    }

    void decrementTimeLeft()
    {
        timeLeft--;
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
        currWord = "";
    }
}
