using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utilities
{
    public enum Scope
    {
        [Description("Individual Exercise!")]
        INDIVIDUAL,
        [Description("Group Exercise!")]
        COOPERATION
    }

    public static string getDescription(Scope enumerationValue)
    {
        Type type = enumerationValue.GetType();
        if (!type.IsEnum)
        {
            throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
        }

        //Tries to find a DescriptionAttribute for a potential friendly name
        //for the enum
        MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
        if (memberInfo != null && memberInfo.Length > 0)
        {
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs != null && attrs.Length > 0)
            {
                //Pull out the description value
                return ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        //If we have no description attribute, just return the ToString of the enum
        return enumerationValue.ToString();
    }
}

public struct Exercise
{
    public string displayMessage;
    public string targetWord;
    public Utilities.Scope scope;

    public Exercise(string displayMessage, string targetWord, Utilities.Scope scope)
    {
        this.displayMessage = displayMessage;
        this.targetWord = targetWord;
        this.scope = scope;
    }
}

public class GameManager : MonoBehaviour
{
    
    public GameObject hpPanel;
    public GameObject displayPanel;
    public GameObject scorePanel;
    public GameObject timePanel;
    public GameObject reqPanel;

    public GameObject scopePanel;

    public GameObject Spawner;
    public GameObject[] LetterSpawners;


    public string currWord;
    public Exercise currExercise;

    public float timeLeft = 30.0f;


    public int lives = 4;
    private int score;
    private Exercise[] exercises = { new Exercise("I got some _ for you", "CAKE", Utilities.Scope.INDIVIDUAL),
                                     new Exercise("23 + 34 = _", "57", Utilities.Scope.COOPERATION)};

    // Use this for initialization
    void Start()
    {

        //exercises[0] = new Exercise("I got some _ for you", "CAKE", Utilities.Scope.INDIVIDUAL);
        //exercises[1] = new Exercise("23 + 34", "57", Utilities.Scope.COOPERATION);


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
        int missingLength = currExercise.targetWord.Length - currWord.Length;
        string[] substrings = currExercise.displayMessage.Split('_');

        string displayString = substrings[0];
        displayString += currWord;
        for (int i = 0; i < missingLength; i++)
        {
            displayString += "_";
        }
        displayString += substrings[1];

        reqPanel.GetComponent<reqScript>().updateRequirement(displayString);



        currWord = currWord.ToUpper();



        if (timeLeft <= 0.0f || lives < 1)
        {
            SceneManager.LoadScene("gameover");
        }

        string currTargetWord = currExercise.targetWord;
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
        int random = UnityEngine.Random.Range(0, exercises.Length);
        currExercise = exercises[random];
        currWord = "";

        displayPanel.GetComponent<DisplayPanel>().setCurrWord(currExercise.targetWord);
        scopePanel.GetComponent<UnityEngine.UI.Text>().text = Utilities.getDescription(currExercise.scope);
    }

    void hurt()
    {
        lives--;
        currWord = "";
    }
}
