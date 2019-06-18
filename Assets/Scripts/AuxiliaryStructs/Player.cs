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

    public Player(List<KeyCode> myKeys, List<string> myButtons, List<float> buttonRGB)
    {
        this.buttonColor = buttonColor = new Color(buttonRGB[0], buttonRGB[1], buttonRGB[2], 1.0f);

        this.myKeys = myKeys;
        this.myButtons = myButtons;
        
        this.name = "";
        this.score = 0;
    }

    public void Init() {
        this.buttonColor = buttonColor = new Color(buttonRGB[0], buttonRGB[1], buttonRGB[2], 1.0f);
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
        this.activeButtonIndex = activeButtonIndex;
    }

    public Color GetButtonColor()
    {
        return buttonColor;
    }

}