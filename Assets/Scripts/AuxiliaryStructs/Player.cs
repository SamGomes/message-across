using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Player(List<KeyCode> myKeys, List<string> myButtons, List<float> buttonRGB)
    {
        this.buttonColor = buttonColor = new Color(buttonRGB[0], buttonRGB[1], buttonRGB[2], 1.0f);

        this.myKeys = myKeys;
        this.myButtons = myButtons;
        
        this.name = "";
        this.score = 0;
    }

    public void Init(GameObject markerPrefab, GameObject canvas)
    {
        marker = UnityEngine.Object.Instantiate(markerPrefab, canvas.transform);
        this.buttonColor = buttonColor = new Color(buttonRGB[0], buttonRGB[1], buttonRGB[2], 1.0f);
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

    public void SetActiveButtonIndex(int activeButtonIndex)
    {
        //Vector3.Lerp(marker.transform.position, , 20 * Time.deltaTime); 
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

}