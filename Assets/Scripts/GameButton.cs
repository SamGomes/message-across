using AuxiliaryStructs;
using Mirror;
using UnityEngine;


public class GameButton : NetworkBehaviour {
    
    private bool keyPressed;
    private bool isClicked;

    private AudioManager buttonAudioManager;

    private bool isBeingPressed;

    private GameObject currCollidingLetter;

    private Player owner;

    [ClientRpc]
    public void RegisterButtonDown()
    {
        this.keyPressed = true;
        this.isBeingPressed = true;
    }
    [ClientRpc]
    public void RegisterButtonUp()
    {
        this.isBeingPressed = false;
    }
    
    public bool IsClicked()
    {
        return isClicked;
    }

    public GameObject GetCollidingLetter()
    {
        return currCollidingLetter;
    }
    
    [ClientRpc]
    public void SetCollidingLetter(GameObject otherObject)
    {
        currCollidingLetter = otherObject;
    }
    [ClientRpc]
    public void ResetCollidingLetter()
    {
        currCollidingLetter = null;
    }

    
    void Start()
    {
        currCollidingLetter = null;
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
    
    [Server]
    void OnTriggerEnter(Collider letterCollider)
    {
        if (letterCollider.GetComponent<Letter>() == null){
            return;
        }
        SetCollidingLetter(letterCollider.gameObject);
    }
    
    [Server]
    void OnTriggerExit(Collider otherObject)
    {
        this.currCollidingLetter = null;
    }

   
}
