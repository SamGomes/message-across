using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private string name;

    private List<KeyCode> myKeys;
    private List<string> myButtons;

    public int score;


    //performance metrics
    public Dictionary<List<Player>, int> buttonHits;

    public Player(List<KeyCode> myKeys, List<string> myButtons)
    {
        this.myKeys = myKeys;
        this.myButtons = myButtons;
        
        this.name = "";
        this.score = 0;

        this.buttonHits = new Dictionary<List<Player>, int>();
    }
    
    public void AddHitToStatistics(List<Player> players)
    {
        if (!buttonHits.ContainsKey(players))
        {
            buttonHits.Add(players, 1);
        }
        buttonHits[players]++;
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