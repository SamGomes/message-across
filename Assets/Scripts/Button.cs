using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Button : MonoBehaviour {

    public GameObject gameManager;
    public Utilities.ButtonId buttonCode;

    private bool clicked;

    private int playerIndex;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        playerIndex = -1;

        List<Utilities.ButtonId> pressedButtonCodes = new List<Utilities.ButtonId>();

        List<Player> players = gameManager.GetComponent<GameManager>().getPlayers();

        var xboxKeyCodesPL0 = players[0].xboxInputKeyCodes;
        var xboxKeyCodesPL1 = players[1].xboxInputKeyCodes;

        for (int i = 0; i < xboxKeyCodesPL0.Length; i++)
        {
            if (Input.GetButton(xboxKeyCodesPL0[i]))
            {
                playerIndex = 0;
                pressedButtonCodes.Add(Utilities.buttonIds[i]);
            }
        }

        for (int i = 0; i < xboxKeyCodesPL1.Length; i++)
        {
            if (Input.GetButton(xboxKeyCodesPL1[i]))
            {
                playerIndex = 1;
                pressedButtonCodes.Add(Utilities.buttonIds[i]);
            }
        }


        this.clicked = false;
        this.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);


        if (playerIndex == -1)
        {
            return;
        }

        //------------------ MODS ---------------------
        var inputModForThisPlayer = players[playerIndex].inputMod;
        
        bool buttonIsPressed = pressedButtonCodes.Contains(this.buttonCode);

        if (inputModForThisPlayer == Utilities.InputMod.BTN_ALL_ACTIONS)
        {
            buttonIsPressed = true;
        }

        Debug.Log("pressedButtons: " + pressedButtonCodes.ToArray().ToString());


        if (inputModForThisPlayer == Utilities.InputMod.BTN_OPPOSITION)
        {
            buttonIsPressed = !buttonIsPressed;
        }


        if (inputModForThisPlayer == Utilities.InputMod.BTN_EXCHANGE)
        {
            buttonIsPressed = !buttonIsPressed;
        }
        

        if (!buttonIsPressed)
        {
            return;
        }

        //------------------ RESTRICTIONS ---------------------
        Utilities.InputRestriction iRestCurrPL = players[playerIndex].inputRestriction;

        bool validateBtn0Input = (this.buttonCode == Utilities.ButtonId.BTN_0)
            //&&(iRestCurrPL != Utilities.InputRestriction.ALL_BTNS)
            && (iRestCurrPL != Utilities.InputRestriction.BTN_0_ONLY);

        bool validateBtn1Input = (this.buttonCode == Utilities.ButtonId.BTN_1)
           // && (iRestCurrPL != Utilities.InputRestriction.ALL_BTNS)
            && (iRestCurrPL != Utilities.InputRestriction.BTN_1_ONLY);

        //(uRestPL1 == Utilities.InputRestriction.BTN_EXCHANGE)

        if (validateBtn0Input || validateBtn1Input)
        {
            this.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            this.clicked = true;
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
            gameManager.GetComponent<GameManager>().incrementPlayerScore(this.playerIndex);

            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
