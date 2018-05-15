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
        PLAYER_1,
        NONE
    }

    public enum ButtonId
    {
        BTN_0,
        BTN_1,
        NONE
    }

    public enum InputRestriction
    {
        BTN_0_ONLY,
        BTN_1_ONLY,
        NONE
        //ALL_BTNS
    }

    public enum InputMod
    {
        //BTN_OPPOSITION,
        BTN_EXCHANGE,
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


public class GameManager : MonoBehaviour
{

    public bool isGameplayPaused;

    public GameObject hpPanel;
    public GameObject displayPanel;
    public GameObject scorePanel;
    public GameObject timePanel;
    public GameObject reqPanel;

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
    private Utilities.PlayerId lastPlayerToPressIndex = Utilities.PlayerId.NONE;

    private List<Utilities.OutputRestriction> prevAntOutputs;

    public void pauseGame()
    {
        CancelInvoke();
        isGameplayPaused = true;
    }

    public void resumeGame()
    {
        InvokeRepeating("decrementTimeLeft", 0.0f, 1.0f);
        isGameplayPaused = false;
    }

    // Use this for initialization
    void Start()
    {

        isGameplayPaused = false;

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

        GameObject pauseMenu = GameObject.Find("Canvas/PauseCanvas");
        pauseMenu.SetActive(false);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isGameplayPaused)
        {
            return;
        }


        if (lifes < 1)
        {
            SceneManager.LoadScene("gameover");
        }

        hpPanel.GetComponent<UnityEngine.UI.Text>().text = "Lifes: "+ lifes;
        scorePanel.GetComponent<UnityEngine.UI.Text>().text = "Score: "+ score;
        timePanel.GetComponent<UnityEngine.UI.Text>().text = "Time: "+ timeLeft;

        for(int i=0; i < players.Count; i++)
        {
            playersPanel.transform.GetComponentsInChildren<UnityEngine.UI.Text>()[i].text = "Score of player "+i+": " + players[i].score;
        }

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
            hurt();
        }

        string currTargetWord = Utilities.currExercise.targetWord;

        if (currWord == ""){
            return;
        }
        if ((currWord.Length > currTargetWord.Length) || currTargetWord[currWord.Length-1]!=currWord[currWord.Length - 1])
        {
            hurt();
            currWord = currWord.Remove(currWord.Length - 1);
            return;
        }
        
        if (currWord.CompareTo(currTargetWord) == 0)
        {
            score += currTargetWord.Length;
            players[(int)lastPlayerToPressIndex].score += currTargetWord.Length;
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
            }

            //spawn questionnaires before changing word
            foreach (Player player in players)
            {
                Application.ExternalEval("window.open('https://docs.google.com/forms/d/e/1FAIpQLSeM3Xn5qDBdX7QCtyrPILLbqpYj3ueDcLa_-9CbxCPzxVsMzg/viewform?usp=pp_url&entry.2097900814=" + player.id+"&entry.159491668="+ (int) player.inputRestriction+"&entry.978719613="+(int) player.inputMod+"&entry.1620449534="+ (int) prevAntOutputs[players.IndexOf(player)]+"');"); //spawn questionaire
            }
            changeTargetWord();
        }

    }

    void decrementTimeLeft()
    {
        if(timeLeft > 0.0f){
            timeLeft--; 
        }
    }

    void changeTargetWord()
    {

        //init restrictions for all players
        for (int i = 0; i < players.Count; i++) 
        {
            players[i].inputRestriction = chooseInputRestriction();
            players[i].inputMod = chooseInputMod();
        }
        
        prevAntOutputs = new List<Utilities.OutputRestriction>();


        int random = UnityEngine.Random.Range(0, exercises.Count);
        Utilities.currExercise = exercises[random];

        string targetWord = Utilities.currExercise.targetWord;

        //init restrictions for all players
        for (int i = 0; i < antSpawners.Length; i++)
        {
            Utilities.OutputRestriction currOutputRestriction = antSpawners[i].GetComponent<AntSpawner>().outputRestriction;
            prevAntOutputs.Add(currOutputRestriction);
            
            string starredWord = "";
            if (currOutputRestriction==Utilities.OutputRestriction.STARPOWER)
            {
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



    public void setLastPlayerToPressIndex(Utilities.PlayerId playerIndex)
    {
       this.lastPlayerToPressIndex = playerIndex;
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


    public List<Player> getPlayers()
    {
        return this.players;
    }


}


