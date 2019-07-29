using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameButton : MonoBehaviour {

    private GameManager gameManager;

    private bool keyPressed;
    private bool isClicked;

    private AudioManager buttonAudioManager;

    private bool isBeingPressed;

    private Collider currCollidingLetter;

    private Player owner;

    public void RegisterButtonPress()
    {
        this.keyPressed = true;
        this.isBeingPressed = false;
    }
    public void RegisterButtonDown()
    {
        this.keyPressed = true;
        this.isBeingPressed = true;
    }
    public void RegisterButtonUp()
    {
        this.isBeingPressed = false;
    }

    void Start()
    {
        currCollidingLetter = null;
        buttonAudioManager = new AudioManager();
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.keyPressed) {
            this.isClicked = true;
            this.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            this.isClicked = false;
            this.gameObject.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        }
        if(!isBeingPressed)
            this.keyPressed = false;

        if (owner.GetCurrNumPossibleActionsPerLevel() < 1)
        {
            this.isBeingPressed = false;
            this.keyPressed = false;
            return;
        }

        if (this.isClicked && this.currCollidingLetter!=null)
        {
            buttonAudioManager.PlayClip("Audio/note");
            GameObject letter = currCollidingLetter.gameObject;
            letter.transform.localScale *= 1.2f;
            gameManager.RecordHit(currCollidingLetter.gameObject.GetComponent<Letter>().letterText, letter, owner);
            this.currCollidingLetter = null;
        }
    }


    void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.GetComponent<Letter>() == null){
            return;
        }
        this.currCollidingLetter = otherObject;
    }
    
    void OnTriggerExit(Collider otherObject)
    {
        this.currCollidingLetter = null;
    }

    public void SetOwner(Player player)
    {
        this.owner = player;
    }
}
