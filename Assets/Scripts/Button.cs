using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Button : MonoBehaviour {

    public GameManager gameManager;
    public Utilities.ButtonId buttonCode;

    private bool keyPressed;
    private bool clicked;

    private List<Utilities.PlayerId> playersPressingThisButton;

    public void registerUserButtonPress(Utilities.PlayerId[] playersPressingThisButton) {
        this.playersPressingThisButton = new List<Utilities.PlayerId>(playersPressingThisButton);
        List<Player> players = gameManager.getPlayers();
        this.keyPressed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.keyPressed)
        {

            this.clicked = false;
            this.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            
            bool validateBtn0Input = (this.buttonCode == Utilities.ButtonId.BTN_0);
            bool validateBtn1Input = (this.buttonCode == Utilities.ButtonId.BTN_1);

            if (validateBtn0Input || validateBtn1Input)
            {
                this.clicked = true;
                this.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
        }
        else
        {
            this.playersPressingThisButton = new List<Utilities.PlayerId>(2);
            this.clicked = false;
            this.gameObject.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }
        this.keyPressed = false;

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
