using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

    public GameObject gameManager;
    public string buttonCode;
    public string xboxCode;

    private bool clicked;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey(buttonCode) || Input.GetButton(xboxCode))
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
