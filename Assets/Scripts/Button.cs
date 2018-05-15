using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Button : MonoBehaviour {

    public GameManager gameManager;
    public Utilities.ButtonId buttonCode;

    private bool keyPressed;
    private bool clicked;

    private Utilities.PlayerId playerIndex;


    public void registerUserButtonPress(Utilities.PlayerId playerIndex) {

        List<Player> players = gameManager.getPlayers();
        //------------------ MODS ---------------------
        var inputModForThisPlayer = players[(int)playerIndex].inputMod;

        foreach(Button button in gameManager.gameButtons) {
            if(button.buttonCode == this.buttonCode)
            {
                if (inputModForThisPlayer != Utilities.InputMod.BTN_EXCHANGE)
                {
                    registerUserButtonPressed();
                }
            }
            else
            {
                if (inputModForThisPlayer == Utilities.InputMod.BTN_ALL_ACTIONS || inputModForThisPlayer == Utilities.InputMod.BTN_EXCHANGE)
                {
                    button.registerUserButtonPressed();
                }
            }
        }
    }

    public void registerUserButtonPressed()
    {
        this.keyPressed = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.keyPressed)
        {

            this.playerIndex = playerIndex;

            List<Utilities.ButtonId> pressedButtonCodes = new List<Utilities.ButtonId>();

            List<Player> players = gameManager.getPlayers();

            pressedButtonCodes.Add(buttonCode);

            this.clicked = false;
            this.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);


            //------------------ RESTRICTIONS ---------------------
            Utilities.InputRestriction iRestCurrPL = players[(int)playerIndex].inputRestriction;

            bool validateBtn0Input = (this.buttonCode == Utilities.ButtonId.BTN_0)
                //&&(iRestCurrPL != Utilities.InputRestriction.ALL_BTNS)
                && (iRestCurrPL != Utilities.InputRestriction.BTN_0_ONLY);

            bool validateBtn1Input = (this.buttonCode == Utilities.ButtonId.BTN_1)
                // && (iRestCurrPL != Utilities.InputRestriction.ALL_BTNS)
                && (iRestCurrPL != Utilities.InputRestriction.BTN_1_ONLY);

            //(uRestPL1 == Utilities.InputRestriction.BTN_EXCHANGE)

            if (validateBtn0Input || validateBtn1Input)
            {
                this.clicked = true;
                this.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            this.keyPressed = false;
        }
        else
        {
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

            gameManager.GetComponent<GameManager>().currWord+=otherObject.gameObject.GetComponent<Letter>().letterText;
            gameManager.GetComponent<GameManager>().setLastPlayerToPressIndex(this.playerIndex);

            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
