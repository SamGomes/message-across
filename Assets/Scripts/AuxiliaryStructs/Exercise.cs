using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Exercise
{
    public string displayMessage;
    public string targetWord;

    public Exercise(string displayMessage, string targetWord) : this()
    {
        this.displayMessage = displayMessage;
        this.targetWord = targetWord;
    }


}