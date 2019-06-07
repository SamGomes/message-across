using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    [SerializeField]
    private string name;

    [SerializeField]
    private List<KeyCode> myKeys;

    [SerializeField]
    private List<string> myButtons;

    public int score;

    private int activeButtonIndex;

    public Player(List<KeyCode> myKeys, List<string> myButtons)
    {
        this.myKeys = myKeys;
        this.myButtons = myButtons;
        
        this.name = "";
        this.score = 0;
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

    internal int GetActivebuttonIndex()
    {
        return activeButtonIndex;
    }

    internal void SetActiveButtonIndex(int activeButtonIndex)
    {
        this.activeButtonIndex = activeButtonIndex;
    }
}