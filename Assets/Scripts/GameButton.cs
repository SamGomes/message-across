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

    private Collider currCollidingLetterCollider;

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
        currCollidingLetterCollider = null;
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

        
        if (this.isClicked && this.currCollidingLetterCollider!=null)
        {
            buttonAudioManager.PlayClip("Audio/note");
            GameObject currCollidingLetterObject = currCollidingLetterCollider.gameObject;
            currCollidingLetterObject.transform.localScale *= 1.2f;

            Letter theActualLetter = currCollidingLetterCollider.gameObject.GetComponent<Letter>();
            if (!theActualLetter.IsLocked())
            {
                gameManager.RecordHit(theActualLetter.letterText, currCollidingLetterObject, owner);
            }
            else
            {
                theActualLetter.Lock();
            }
            this.currCollidingLetterCollider = null;
        }
    }

    public bool IsButtonBeingPressed()
    {
        return isBeingPressed;
    }

    void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.GetComponent<Letter>() == null){
            return;
        }
        this.currCollidingLetterCollider = otherObject;
    }
    
    void OnTriggerExit(Collider otherObject)
    {
        this.currCollidingLetterCollider = null;
    }

    public void SetOwner(Player player)
    {
        this.owner = player;
    }
}
