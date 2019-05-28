using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private string name;

    private HashSet<KeyCode> myKeys;
    private HashSet<string> myButtons;

    public int score;


    //performance metrics
    public int mybuttonHits;
    public int simultaneousButtonHits;

    public Player(HashSet<KeyCode> myKeys, HashSet<string> myButtons)
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
    public HashSet<KeyCode> GetMyKeys() {
        return myKeys;
    }
    public HashSet<string> GetMyButtons()
    {
        return myButtons;
    }

    public void SetName(string name)
    {
        this.name = name;
    }

}