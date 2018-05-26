using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private Utilities.PlayerId id;
    private string name;

    private KeyCode[] myKeys;
    private string[] myButtons;

    public int score;

    public Player(Utilities.PlayerId id, KeyCode[] myKeys, string[] myButtons)
    {
        this.myKeys = myKeys;
        this.myButtons = myButtons;

        this.id = id;
        this.name = "";
        this.score = 0;
    }

    public Utilities.PlayerId getId()
    {
        return this.id;
    }
    public string getName()
    {
        return this.name;
    }
    public KeyCode[] getMyKeys() {
        return myKeys;
    }
    public string[] getMyButtons()
    {
        return myButtons;
    }

    public void setName(string name)
    {
        this.name = name;
    }

}