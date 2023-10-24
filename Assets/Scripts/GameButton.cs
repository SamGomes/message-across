using AuxiliaryStructs;
using Mirror;
using UnityEngine;


public class GameButton : NetworkBehaviour {
    
    private bool keyPressed;
    private bool isClicked;

    private AudioManager buttonAudioManager;

    private bool isBeingPressed;
    
    public void RegisterButtonDown()
    {
        this.keyPressed = true;
        this.isBeingPressed = true;
    }

    public void RegisterButtonUp()
    {
        this.isBeingPressed = false;
    }
    
    public bool IsClicked()
    {
        return isClicked;
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
   
}
