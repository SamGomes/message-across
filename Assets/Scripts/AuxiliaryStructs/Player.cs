using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Player
{
    [SerializeField]
    private List<float> buttonRGB;
    private Color backgroundColor;

    [SerializeField]
    private string id;

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
    private GameObject scoreUpdateUIup;
    private GameObject scoreUpdateUIdown;

    private Text possibleActionsText;
    private Text scoreText;

    private GameObject marker;
    private List<GameObject> maskedHalf;
    private List<GameObject> activeHalf;
    private GameButton gameButton;

    private GameManager gameManagerRef;
    private Color buttonColor;

    private IEnumerator currButtonLerp;

    private PlayerExercise currExercise;
    public bool currExerciseFinished;
    private string currWordState;

    //log stuff
    public int numGives;
    public int numTakes;

    private GameObject ui;

    public bool pressingButton;



    public Player(string id, List<KeyCode> myKeys, List<string> myButtons, List<float> buttonRGB, int numPossibleActionsPerLevel)
    {
        //if(this.id != "")
        //{
        //    this.id = id;
        //}

        this.buttonRGB = buttonRGB;

        this.myKeys = myKeys;
        this.myButtons = myButtons;

        this.numPossibleActionsPerLevel = numPossibleActionsPerLevel;
        this.currNumPossibleActionsPerLevel = numPossibleActionsPerLevel;
    }

    public void Init(string id, GameManager gameManagerRef, GameObject markerPrefab, GameObject canvas, GameObject ui, GameObject wordPanel, GameObject statePanel, bool isTopMask)
    {
        this.id = id;
        this.ui = ui;

        //this.buttonColor = new Color(buttonRGB[0], buttonRGB[1], buttonRGB[2], 1.0f);
        this.backgroundColor = new Color(buttonRGB[0], buttonRGB[1], buttonRGB[2], 0.8f);
        //marker.transform.position = activeButtonPos;
        this.gameManagerRef = gameManagerRef;

        marker = UnityEngine.Object.Instantiate(markerPrefab, canvas.transform);
        //marker.transform.Rotate(new Vector3(1, 0, 0), angle);
        foreach (SpriteRenderer image in marker.GetComponentsInChildren<SpriteRenderer>()) {
            image.color = this.backgroundColor;
        }

        maskedHalf = new List<GameObject>();
        maskedHalf.Add((isTopMask == true) ? marker.transform.Find("Button/BackgroundTH").gameObject : marker.transform.Find("Button/BackgroundBH").gameObject);
        maskedHalf.Add((isTopMask == true) ? marker.transform.Find("trackTH").gameObject : marker.transform.Find("trackBH").gameObject);


        activeHalf = new List<GameObject>();
        activeHalf.Add((isTopMask == false) ? marker.transform.Find("Button/BackgroundTH").gameObject : marker.transform.Find("Button/BackgroundBH").gameObject);
        activeHalf.Add((isTopMask == false) ? marker.transform.Find("trackTH").gameObject : marker.transform.Find("trackBH").gameObject); //activeHalf[1] is the one of the track
        activeHalf.Add((isTopMask == true) ? marker.transform.Find("Button/BackgroundTH").gameObject : marker.transform.Find("Button/BackgroundBH").gameObject);
        activeHalf.Add((isTopMask == true) ? marker.transform.Find("trackTH").gameObject : marker.transform.Find("trackBH").gameObject);

        activeHalf[1].SetActive(false); //lets init it to hidden

        MeshRenderer[] meshes = marker.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes) {
            mesh.material.color = backgroundColor;
        }

        this.wordPanel = wordPanel;
        this.statePanel = statePanel;

        scoreUpdateUIup = statePanel.transform.Find("scoreUpdateUI/up").gameObject;
        scoreUpdateUIup.SetActive(false);
        scoreUpdateUIdown = statePanel.transform.Find("scoreUpdateUI/down").gameObject;
        scoreUpdateUIdown.SetActive(false);

        this.possibleActionsText = statePanel.transform.Find("possibleActionsText").GetComponent<Text>();
        this.scoreText = statePanel.transform.Find("scoreText").GetComponent<Text>();

        this.buttonColor = SetColor(this.backgroundColor);
        
        this.score = -1;

        ResetNumPossibleActions();

        this.gameButton = marker.GetComponentInChildren<GameButton>();
        this.gameButton.SetOwner(this);

        this.pressingButton = false;
    }

    private Color SetColor(Color newColor)
    {
        statePanel.GetComponentInChildren<Image>().color = newColor;
        ui.GetComponentInChildren<Image>().color = newColor;

        float g = 1.0f - newColor.grayscale*0.8f;
        List<Image> imgs = new List<Image>(ui.GetComponentsInChildren<Image>());
        imgs.RemoveAt(0);
        
        Color dualColor = new Color(g, g, g);

        //for the state display
        scoreText.color = dualColor;
        possibleActionsText.color = dualColor;

        //for player buttons
        foreach (Image img in imgs)
        {
            img.color = dualColor;
        }
        return dualColor;

    }


    public string GetId()
    {
        return id;
    }

    public void SetCurrExercise(PlayerExercise newExercise)
    {
        currExercise = newExercise;
    }
    public PlayerExercise GetCurrExercise()
    {
        return currExercise;
    }

    public void InitCurrWordState()
    {
        currWordState = "";
        int missingLength = currExercise.targetWord.Length;
        for (int i = 0; i < missingLength; i++) {
            currWordState += ' ';
        }

        //Update UI
        TextMesh[] playerDisplayTexts = wordPanel.GetComponentsInChildren<TextMesh>();
        playerDisplayTexts[0].text = currExercise.targetWord;
        playerDisplayTexts[1].text = currWordState;
    }
    public void SetCurrWordState(string newCurrWordState)
    {
        currWordState = newCurrWordState;

        //Update UI
        TextMesh[] playerDisplayTexts = wordPanel.GetComponentsInChildren<TextMesh>();
        playerDisplayTexts[0].text = currExercise.targetWord;
        playerDisplayTexts[1].text = currWordState;
    }
    public string GetCurrWordState()
    {
        return currWordState;
    }

    private IEnumerator TimedSetColor(float timeAmount, Color newColor)
    {
        SetColor(newColor);
        yield return new WaitForSeconds(timeAmount);
        SetColor(this.backgroundColor);
    }

    public IEnumerator DelayedScoreDisplay(float score, float delay)
    {
        yield return new WaitForSeconds(delay);
        scoreText.text = "Score: " + score;
    }

    public void SetScore(int score, int increase, float delay)
    {
        if(this.score != score)
        {
            this.score = score;

            //update UI
            gameManagerRef.StartCoroutine(DelayedScoreDisplay(score, delay));
            statePanel.GetComponent<Animator>().Play(0);
            Color newColor = this.backgroundColor;
            if(increase > 0)
            {
                scoreUpdateUIup.GetComponentInChildren<Text>().text = "+" + increase;
                scoreUpdateUIup.SetActive(false);
                scoreUpdateUIup.SetActive(true);
                //newColor = new Color(0.0f, 1.0f, 0.0f);
            }
            else if(increase < 0)
            {
                scoreUpdateUIdown.GetComponentInChildren<Text>().text = "-" + Math.Abs(increase);
                scoreUpdateUIdown.SetActive(false);
                scoreUpdateUIdown.SetActive(true);
                //newColor = new Color(1.0f, 0.0f, 0.0f);
            }
            //gameManagerRef.StartCoroutine(TimedSetColor(0.5f, newColor));
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

    public List<KeyCode> GetMyKeys() {
        return myKeys;
    }
    public List<string> GetMyButtons()
    {
        return myButtons;
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
    public List<GameObject> GetActiveHalf()
    {
        return this.activeHalf;
    }
    public void SetActiveHalf(List<GameObject> activeHalf)
    {
        this.activeHalf = activeHalf;
    }
    public void UpdateActiveHalf(bool visible)
    {
        for (int i = 0; i < activeHalf.Count; i++)
        {
            if (i % 2 == 0)
            {
                continue;
            }
            GameObject currObj = activeHalf[i];
            currObj.SetActive(visible);
        }
    }

    public Color GetBackgroundColor()
    {
        return this.backgroundColor;
    }

    public void SetActiveInteraction(Globals.KeyInteractionType activeInteraction)
    {
        this.activeInteraction = activeInteraction;
    }
    public Globals.KeyInteractionType GetActiveInteraction()
    {
        return activeInteraction;
    }
    public int GetActivebuttonIndex()
    {
        return this.activeButtonIndex;
    }


    public Color GetButtonColor()
    {
        return buttonColor;
    }

    public GameObject GetMarker()
    {
        return this.marker;
    }

    public void SetNumPossibleActions(int currNumPossibleActionsPerLevel)
    {
        this.currNumPossibleActionsPerLevel = currNumPossibleActionsPerLevel;
        //update UI
        possibleActionsText.text = "Actions: " + currNumPossibleActionsPerLevel;
        statePanel.GetComponent<Animator>().Play(0);
    }
    public void ResetNumPossibleActions()
    {
        SetNumPossibleActions(numPossibleActionsPerLevel);
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

    public void PressGameButton()
    {
        this.pressingButton = true;
        this.gameButton.RegisterButtonDown();
        UpdateActiveHalf(true);
    }

    public void ReleaseGameButton()
    {
        this.pressingButton = false;
        this.gameButton.RegisterButtonUp();
        UpdateActiveHalf(false);

    }

    public GameObject GetUI()
    {
        return this.ui;
    }

    public bool IsPressingButton()
    {
        return pressingButton;
    }

    public GameObject GetStatePanel()
    {
        return this.statePanel;
    }
}