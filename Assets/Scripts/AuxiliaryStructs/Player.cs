using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Utilities.PlayerId id;
    public Utilities.InputMod inputMod;
    public Utilities.InputRestriction inputRestriction;
  
    public int score;

    public Player(Utilities.PlayerId id)
    {
        this.id = id;
        this.inputMod = Utilities.InputMod.NONE;
        this.inputRestriction = Utilities.InputRestriction.NONE;
        this.score = 0;
    }
}