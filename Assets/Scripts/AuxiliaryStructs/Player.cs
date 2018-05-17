﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Utilities.PlayerId id;
    public Utilities.PlayerInputMod inputMod;
    public Utilities.PlayerInputRestriction inputRestriction;
  
    public int score;

    public Player(Utilities.PlayerId id)
    {
        this.id = id;
        this.inputMod = Utilities.PlayerInputMod.NONE;
        this.inputRestriction = Utilities.PlayerInputRestriction.NONE;
        this.score = 0;
    }
}