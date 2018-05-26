using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager : MonoBehaviour {

    public enum ButtonPressType
    {
        ALL,
        DOWN,
        UP,
    }

    public GameManager gameManager;
    public GameSceneManager gameSceneManager;

    public delegate void CallBack();

    Dictionary<KeyCode[], KeyValuePair< ButtonPressType, CallBack> > keyBindings;
    Dictionary<string[], KeyValuePair< ButtonPressType, CallBack> > buttonBindings;

    void Start()
    {
        keyBindings = new Dictionary<KeyCode[], KeyValuePair<ButtonPressType, CallBack>>();
        buttonBindings = new Dictionary<string[], KeyValuePair<ButtonPressType, CallBack>>();
        
        this.addKeyBinding(new KeyCode[] { KeyCode.Space }, ButtonPressType.DOWN, delegate () { gameSceneManager.startAndPauseGame(Utilities.PlayerId.NONE); });
        //this.addKeyBinding(KeyCode.Space, ButtonPressType.DOWN, delegate () { startQuitScript.pauseGame(Utilities.PlayerId.NONE); });
        this.addKeyBinding(new KeyCode[] { KeyCode.Q }, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        this.addKeyBinding(new KeyCode[] { KeyCode.W }, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        this.addKeyBinding(new KeyCode[] { KeyCode.O }, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
        this.addKeyBinding(new KeyCode[] { KeyCode.P }, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });

        this.addButtonBinding(new string[] { "Start" }, ButtonPressType.DOWN, delegate () { gameSceneManager.startAndPauseGame(Utilities.PlayerId.NONE); });
        //this.addButtonBinding("Pause", ButtonPressType.DOWN, delegate () { startQuitScript.startGame(Utilities.PlayerId.NONE); });
        this.addButtonBinding(new string[] { "YButtonJoy1" }, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        this.addButtonBinding(new string[] { "BButtonJoy1" }, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        this.addButtonBinding(new string[] { "YButtonJoy2" }, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
        this.addButtonBinding(new string[] { "BButtonJoy2" }, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
    }

    // Update is called once per frame
    void Update()
    {
        foreach(KeyCode[] simultaneouskeys in keyBindings.Keys)
        {
            var pair = keyBindings[simultaneouskeys];

            bool allKeysPressed=true;
            foreach (KeyCode key in simultaneouskeys)
            {
                if (!((pair.Key == ButtonPressType.ALL && Input.GetKey(key))
                 || (pair.Key == ButtonPressType.DOWN && Input.GetKeyDown(key))
                 || (pair.Key == ButtonPressType.UP && Input.GetKeyUp(key))))
                {
                    allKeysPressed = false;
                }
            }
            if (allKeysPressed)
            {
                pair.Value();
            }
        }

        foreach (string[] simultaneouskeys in buttonBindings.Keys)
        {
            var pair = buttonBindings[simultaneouskeys];

            bool allKeysPressed = true;
            foreach (string key in simultaneouskeys)
            {
                if (!((pair.Key == ButtonPressType.ALL && Input.GetButton(key))
                 || (pair.Key == ButtonPressType.DOWN && Input.GetButtonDown(key))
                 || (pair.Key == ButtonPressType.UP && Input.GetButtonUp(key))))
                {
                    allKeysPressed = false;
                }
            }
            if (allKeysPressed)
            {
                pair.Value();
            }
        }

    }

    public void addKeyBinding(KeyCode[] keys, ButtonPressType pressType, CallBack callback)
    {
        keyBindings.Add(keys, new KeyValuePair < ButtonPressType, CallBack >(pressType, callback));
    }

    public void addButtonBinding(string[] keys, ButtonPressType pressType, CallBack callback)
    {
        buttonBindings.Add(keys, new KeyValuePair<ButtonPressType, CallBack>(pressType, callback));
    }

}


