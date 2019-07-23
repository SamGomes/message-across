using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameButton : MonoBehaviour {

    private GameManager gameManager;

    private bool keyPressed;
    private bool isClicked;

    private AudioManager buttonAudioManager;

    private HashSet<Player> currHitters;

    private bool isBeingPressed;
    
    public void RegisterButtonPress(Player hitter) {
        currHitters.Add(hitter);
        this.keyPressed = true;
    }
    public void RegisterButtonDown(Player hitter) {
        currHitters.Add(hitter);
        this.keyPressed = true;
        this.isBeingPressed = true;
    }
    public void RegisterButtonUp(Player hitter) {
        currHitters.Remove(hitter);
        this.isBeingPressed = false;
    }

    void Start()
    {
        buttonAudioManager = new AudioManager();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        currHitters = new HashSet<Player>();  
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
    }


    void OnTriggerEnter(Collider otherObject)
    {

        if (otherObject.GetComponent<Letter>() == null){
            return;
        }
        if (this.isClicked)
        {
            buttonAudioManager.PlayClip("Audio/note");
            
            GameObject letter = otherObject.gameObject;
            
            letter.transform.localScale *= 1.2f;

            gameManager.RecordHit(otherObject.gameObject.GetComponent<Letter>().letterText, letter, currHitters);

            currHitters.Clear();
        }
    }
}
