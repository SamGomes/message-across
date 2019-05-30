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

    //performance metrics
    public int mybuttonHits;
    public int simultaneousButtonHits;

    public Player(List<KeyCode> myKeys, List<string> myButtons)
    {
        this.myKeys = myKeys;
        this.myButtons = myButtons;
        
        this.name = "";
        this.score = 0;

        this.mybuttonHits = 0;
        this.simultaneousButtonHits = 0;
    }
    
    public void AddHitToStatistics(List<Player> players)
    {
        if (players.Contains(this))
        {
            if(players.Count == 1)
            {
                mybuttonHits++;
            }
            else
            {
                simultaneousButtonHits++;
            }
        }
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

}