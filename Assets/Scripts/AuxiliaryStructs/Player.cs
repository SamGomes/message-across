using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Utilities.PlayerId id;
    public string name;
  
    public int score;

    public Player(Utilities.PlayerId id)
    {
        this.id = id;
        this.name = "";
        this.score = 0;
    }
}