using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Button : MonoBehaviour {

    public GameManager gameManager;
    public Utilities.ButtonId buttonCode;

    private bool keyPressed;
    private bool clicked;

    private List<Utilities.PlayerId> playersPressingThisButton;


    public void registerUserButtonPress(Utilities.PlayerId playerIndex) {

        if (!this.playersPressingThisButton.Contains(playerIndex))
        {
            this.playersPressingThisButton.Add(playerIndex);
        }

        List<Player> players = gameManager.getPlayers();

        //------------------ MODS ---------------------
        Utilities.PlayerInputMod inputModForThisPlayer = players[(int)playerIndex].inputMod;

        foreach(Button button in gameManager.gameButtons) {
            if(button.buttonCode == this.buttonCode)
            {
                if (inputModForThisPlayer != Utilities.PlayerInputMod.BTN_EXCHANGE)
                {
                    registerUserButtonPressed(playerIndex);
                }
            }
            else
            {
                if (inputModForThisPlayer == Utilities.PlayerInputMod.BTN_ALL_ACTIONS || inputModForThisPlayer == Utilities.PlayerInputMod.BTN_EXCHANGE)
                {
                    button.registerUserButtonPressed(playerIndex);
                }
            }
        }
    }

    public void registerUserButtonPressed(Utilities.PlayerId playerIndex)
    {
        if (!this.playersPressingThisButton.Contains(playerIndex))
        {
            this.playersPressingThisButton.Add(playerIndex);
        }
        this.keyPressed = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.keyPressed)
        {
            List<Player> players = gameManager.getPlayers();

            this.clicked = false;
            this.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);


            Utilities.GlobalInputMod currGlobalInputMod = gameManager.currGlobalInputMod;

            //------------------ RESTRICTIONS (AND GLOBAL INPUT MODS)---------------------
            bool buttonWasValidByAll = true;
            foreach (Utilities.PlayerId playerIndex in playersPressingThisButton)
            {
                Utilities.PlayerInputRestriction iRestCurrPL = players[(int)playerIndex].inputRestriction;

                bool validateBtn0Input = (this.buttonCode == Utilities.ButtonId.BTN_0)
                    //&&(iRestCurrPL != Utilities.InputRestriction.ALL_BTNS)
                    && (iRestCurrPL != Utilities.PlayerInputRestriction.BTN_0_ONLY);

                bool validateBtn1Input = (this.buttonCode == Utilities.ButtonId.BTN_1)
                    // && (iRestCurrPL != Utilities.InputRestriction.ALL_BTNS)
                    && (iRestCurrPL != Utilities.PlayerInputRestriction.BTN_1_ONLY);
                

                if (!(validateBtn0Input || validateBtn1Input))
                {
                    buttonWasValidByAll = false;
                }
            }

            if (buttonWasValidByAll)
            {
                if ((currGlobalInputMod != Utilities.GlobalInputMod.BTN_MIXEDINPUT) || 
                   (playersPressingThisButton.Count==2 && (currGlobalInputMod==Utilities.GlobalInputMod.BTN_MIXEDINPUT)))
                {
                    this.clicked = true;
                    this.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }

                //if no mixed input register only first player to press
                if (playersPressingThisButton.Count > 1 && currGlobalInputMod != Utilities.GlobalInputMod.BTN_MIXEDINPUT)
                {
                    List<Utilities.PlayerId> auxList = new List<Utilities.PlayerId>();
                    auxList.Add(this.playersPressingThisButton[0]);
                    this.playersPressingThisButton = auxList;
                }
            }

            this.keyPressed = false;
        }
        else
        {
            this.playersPressingThisButton = new List<Utilities.PlayerId>(2);
            this.clicked = false;
            this.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }

    }
    

    void OnTriggerEnter(Collider otherObject)
    {
        if (otherObject.GetComponent<Letter>()== null){
            return;
        }
        if (this.clicked)
        {
            otherObject.gameObject.transform.localScale = new Vector3(0.3f, 0.3f, 1.0f);
            otherObject.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;

            gameManager.recordHit(this.playersPressingThisButton, otherObject.gameObject.GetComponent<Letter>().letterText);

            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
