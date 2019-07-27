﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Player
{
    [SerializeField]
    private List<float> buttonRGB;
    private Color buttonColor;

    [SerializeField]
    private string name;

    [SerializeField]
    private List<KeyCode> myKeys;

    [SerializeField]
    private List<string> myButtons;

    [SerializeField]
    private int numPossibleActionsPerLevel;

    private int currNumPossibleActionsPerLevel;

    private int score;
    private int activeButtonIndex;

    private Globals.KeyInteractionType activeInteraction;

    private GameObject wordPanel;
    private GameObject statePanel;
    private Text possibleActionsText;
    private Text scoreText;
    private Text nameText;

    private GameObject marker;
    private List<GameObject> maskedHalf;

    private GameManager gameManagerRef;

    private IEnumerator currButtonLerp;


    private Exercise currExercise;
    public bool isCurrExerciseFinished;
    private string currWordState;

    //log stuff
    public int numGivePresses;
    public int numTakePresses;

    public Player(List<KeyCode> myKeys, List<string> myButtons, List<float> buttonRGB, int numPossibleActionsPerLevel)
    {
        this.buttonRGB = buttonRGB;

        this.myKeys = myKeys;
        this.myButtons = myButtons;

        this.numPossibleActionsPerLevel = numPossibleActionsPerLevel;
        this.currNumPossibleActionsPerLevel = numPossibleActionsPerLevel;
    }

    public void Init(GameManager gameManagerRef, GameObject markerPrefab, GameObject canvas, GameObject wordPanel, GameObject statePanel, bool isTopMask)
    {
        //this.buttonColor = new Color(buttonRGB[0], buttonRGB[1], buttonRGB[2], 1.0f);
        this.buttonColor = new Color(buttonRGB[0], buttonRGB[1], buttonRGB[2], 0.8f);
        //marker.transform.position = activeButtonPos;
        this.gameManagerRef = gameManagerRef;

        marker = UnityEngine.Object.Instantiate(markerPrefab, canvas.transform);
        //marker.transform.Rotate(new Vector3(1, 0, 0), angle);
        foreach (SpriteRenderer image in marker.GetComponentsInChildren<SpriteRenderer>()) {
            image.color = this.buttonColor;
        }

        maskedHalf = new List<GameObject>();
        maskedHalf.Add((isTopMask == true) ? marker.transform.Find("Button/BackgroundTH").gameObject : marker.transform.Find("Button/BackgroundBH").gameObject);
        maskedHalf.Add((isTopMask == true) ? marker.transform.Find("trackTH").gameObject : marker.transform.Find("trackBH").gameObject);

        this.wordPanel = wordPanel;
        this.statePanel = statePanel;
        this.possibleActionsText = statePanel.transform.Find("possibleActionsText").GetComponent<Text>();
        this.scoreText = statePanel.transform.Find("scoreText").GetComponent<Text>();
        this.nameText = statePanel.transform.Find("nameText").GetComponent<Text>();


        statePanel.GetComponentInChildren<Image>().color = this.buttonColor;

        this.name = "";
        this.score = -1;

        ResetNumPossibleActions();
    }
    
    public void SetCurrExercise(Exercise newExercise)
    {
        //currWordState = "";
        currExercise = newExercise;
    }
    public Exercise GetCurrExercise()
    {
        return currExercise;
    }

    public void SetCurrWordState(string newCurrWordState)
    {
        currWordState = newCurrWordState;

        //update UI
        string displayString = "";
        int missingLength = currExercise.targetWord.Length - currWordState.Length;
        string[] substrings = currExercise.displayMessage.Split('_');

        if (substrings.Length > 0)
        {
            displayString += substrings[0];
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
        displayString += "\n";

        TextMesh playerDisplayText = wordPanel.GetComponentInChildren<TextMesh>();
        playerDisplayText.text = displayString;
    }
    public string GetCurrWordState()
    {
        return currWordState;
    }

    public void SetScore(int score)
    {
        if(this.score != score)
        {
            this.score = score;

            //update UI
            scoreText.text = "Score: " + score;
            statePanel.GetComponent<Animator>().Play(0);
        }

    }
    public int GetScore()
    {
        return this.score;
    }
    
    public void SetWordPanel(GameObject wordPanel)
    {
        this.wordPanel = wordPanel;
    }
    public GameObject GetWordPanel()
    {
        return this.wordPanel;
    }

    public string GetName()
    {
        return this.name;
    }
    public List<KeyCode> GetMyKeys() {
        return myKeys;
    }
    public List<string> GetMyButtons()
    {
        return myButtons;
    }

    public void SetName(string name)
    {
        this.name = name;

        //update UI
        nameText.text = "Player " + name;
    }

    public int GetActivebuttonIndex()
    {
        return activeButtonIndex;
    }

    public void SetActiveButton(int activeButtonIndex, Vector3 activeButtonPos)
    {
        if (currButtonLerp != null)
        {
            gameManagerRef.StopCoroutine(currButtonLerp);
        }

        currButtonLerp = Globals.LerpAnimation(marker, activeButtonPos, 10.0f);
        gameManagerRef.StartCoroutine(currButtonLerp);

        this.activeButtonIndex = activeButtonIndex;
    }

    public List<GameObject> GetMaskedHalf()
    {
        return this.maskedHalf;
    }

    public void SetActiveInteraction(Globals.KeyInteractionType activeInteraction)
    {
        this.activeInteraction = activeInteraction;
    }
    public Globals.KeyInteractionType GetActiveInteraction()
    {
        return activeInteraction;
    }


    public Color GetButtonColor()
    {
        return buttonColor;
    }

    public GameObject GetMarker()
    {
        return this.marker;
    }

    public void ResetNumPossibleActions()
    {
        this.currNumPossibleActionsPerLevel = numPossibleActionsPerLevel;
        //update UI
        possibleActionsText.text = "Actions: " + numPossibleActionsPerLevel;
        statePanel.GetComponent<Animator>().Play(0);
    }

    public int GetCurrNumPossibleActionsPerLevel()
    {
        return this.currNumPossibleActionsPerLevel;
    }

    public void DecreasePossibleActionsPerLevel()
    {
        currNumPossibleActionsPerLevel--;
        //update UI
        possibleActionsText.text = "Actions: " + currNumPossibleActionsPerLevel;
        statePanel.GetComponent<Animator>().Play(0);
    }
}