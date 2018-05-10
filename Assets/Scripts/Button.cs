using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonId
{
    BTN_0,
    BTN_1
}

public class Button : MonoBehaviour {

    public GameObject gameManager;
    public ButtonId buttonCode;
    private string xboxCodeJoy1;
    private string xboxCodeJoy2;

    private bool clicked;

	// Use this for initialization
	void Start () {
        switch (buttonCode)
        {
            case ButtonId.BTN_0:
                xboxCodeJoy1 = "YButtonJoy1";
                xboxCodeJoy2 = "YButtonJoy2";
                break;

            case ButtonId.BTN_1:
                xboxCodeJoy1 = "BButtonJoy1";
                xboxCodeJoy2 = "BButtonJoy2";
                break;
        }
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        List<Utilities.InputRestriction> currIRestr = Utilities.currExercise.getInputRestrictionsForEachPlayer();
        Utilities.InputRestriction iRestPL1 = currIRestr[0];
        Utilities.InputRestriction iRestPL2 = currIRestr[1];

        bool validateJoy1Input = (Input.GetButton(xboxCodeJoy1)
            && (iRestPL1 != Utilities.InputRestriction.ALL_BTNS)
            && (buttonCode==ButtonId.BTN_0 && iRestPL1!=Utilities.InputRestriction.BTN_0_ONLY));

        bool validateJoy2Input = (Input.GetButton(xboxCodeJoy2)
            && (iRestPL2 != Utilities.InputRestriction.ALL_BTNS)
            && (buttonCode == ButtonId.BTN_1 && iRestPL2 != Utilities.InputRestriction.BTN_1_ONLY));

        //(uRestPL1 == Utilities.InputRestriction.BTN_EXCHANGE)

        if (validateJoy1Input && validateJoy1Input)
        {
            this.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            this.clicked = true;
        }else
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

            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
