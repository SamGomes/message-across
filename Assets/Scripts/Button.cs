using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Button : MonoBehaviour {

    public GameObject gameManager;
    public Utilities.ButtonId buttonCode;
    private string xboxCodeJoy1;

    private bool clicked;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        int playerIndex = -1;
        List<Utilities.ButtonId> pressedButtonCodes = new List<Utilities.ButtonId>();


        List<Utilities.InputRestriction> currIRestr = Utilities.currExercise.getInputRestrictionsForEachPlayer();

        var xboxKeyCodesPL0 = Utilities.xboxInputKeyCodes[Utilities.PlayerId.PLAYER_0];
        var xboxKeyCodesPL1 = Utilities.xboxInputKeyCodes[Utilities.PlayerId.PLAYER_1];

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

        Debug.Log(playerIndex);

        this.clicked = false;
        this.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);


        if (playerIndex == -1)
        {
            return;
        }

        //------------------ MODS ---------------------
        if (Utilities.currExercise.getInputModsForEachPlayer()[playerIndex] == Utilities.InputMod.BTN_ALL_ACTIONS)
        {
            pressedButtonCodes = new List<Utilities.ButtonId>();
            pressedButtonCodes.AddRange(Utilities.buttonIds);
        }
        bool buttonIsPressed = pressedButtonCodes.Contains(this.buttonCode);

        if (Utilities.currExercise.getInputModsForEachPlayer()[playerIndex] == Utilities.InputMod.BTN_OPPOSITION)
        {
            buttonIsPressed = !buttonIsPressed;
        }
        

        if (!buttonIsPressed)
        {
            return;
        }

        //------------------ RESTRICTIONS ---------------------
        Utilities.InputRestriction iRestCurrPL = currIRestr[playerIndex];
        
        bool validateBtn0Input = (this.buttonCode == Utilities.ButtonId.BTN_0)
            &&(iRestCurrPL != Utilities.InputRestriction.ALL_BTNS)
            && (iRestCurrPL != Utilities.InputRestriction.BTN_0_ONLY);

        bool validateBtn1Input = (this.buttonCode == Utilities.ButtonId.BTN_1)
            && (iRestCurrPL != Utilities.InputRestriction.ALL_BTNS)
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

            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
