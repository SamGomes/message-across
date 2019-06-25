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
    private Color buttonColor;

    [SerializeField]
    private string name;

    [SerializeField]
    private List<KeyCode> myKeys;

    [SerializeField]
    private List<string> myButtons;

    public string currWordState;
    public int score;
    private int activeButtonIndex;

    private Globals.KeyInteractionType activeInteraction;

    private GameObject wordPanel;
    private GameObject scorePanel;

    private GameObject marker;

    private GameManager gameManagerRef;

    private IEnumerator currButtonLerp;

    public Player(List<KeyCode> myKeys, List<string> myButtons, List<float> buttonRGB)
    {
        this.buttonRGB = buttonRGB;

        this.myKeys = myKeys;
        this.myButtons = myButtons;
        
        this.name = "";
        this.score = 0;
    }

    public void Init(GameManager gameManagerRef, GameObject markerPrefab, GameObject canvas)
    {
        this.buttonColor = new Color(buttonRGB[0], buttonRGB[1], buttonRGB[2], 0.5f);
        //marker.transform.position = activeButtonPos;
        this.gameManagerRef = gameManagerRef;

        marker = UnityEngine.Object.Instantiate(markerPrefab, canvas.transform);
        foreach (Image image in marker.GetComponentsInChildren<Image>()) {
            image.color = this.buttonColor;
        }
    }

    public void SetScorePanel(GameObject scorePanel)
    {
        this.scorePanel = scorePanel;
    }
    public void SetWordPanel(GameObject wordPanel)
    {
        this.wordPanel = wordPanel;
    }
    public GameObject GetScorePanel()
    {
        return this.scorePanel;
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

        currButtonLerp = Globals.LerpAnimation(marker, activeButtonPos, 3.5f);
        gameManagerRef.StartCoroutine(currButtonLerp);

        this.activeButtonIndex = activeButtonIndex;
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

}