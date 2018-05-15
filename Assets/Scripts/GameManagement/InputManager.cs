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

    Dictionary<KeyCode, KeyValuePair< ButtonPressType, CallBack> > keyBindings;
    Dictionary<string, KeyValuePair< ButtonPressType, CallBack> > buttonBindings;

    void Start()
    {
        keyBindings = new Dictionary<KeyCode, KeyValuePair<ButtonPressType, CallBack>>();
        buttonBindings = new Dictionary<string, KeyValuePair<ButtonPressType, CallBack>>();
        
        this.addKeyBinding(KeyCode.S, ButtonPressType.DOWN, delegate () { gameSceneManager.startAndPauseGame(Utilities.PlayerId.NONE); });
        //this.addKeyBinding(KeyCode.Space, ButtonPressType.DOWN, delegate () { startQuitScript.pauseGame(Utilities.PlayerId.NONE); });
        this.addKeyBinding(KeyCode.Q, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        this.addKeyBinding(KeyCode.W, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        this.addKeyBinding(KeyCode.O, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
        this.addKeyBinding(KeyCode.P, ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });

        this.addButtonBinding("Start", ButtonPressType.DOWN, delegate () { gameSceneManager.startAndPauseGame(Utilities.PlayerId.NONE); });
        //this.addButtonBinding("Pause", ButtonPressType.DOWN, delegate () { startQuitScript.startGame(Utilities.PlayerId.NONE); });
        this.addButtonBinding("YButtonJoy1", ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        this.addButtonBinding("BButtonJoy1", ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        this.addButtonBinding("YButtonJoy2", ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
        this.addButtonBinding("BButtonJoy2", ButtonPressType.ALL, delegate () { gameManager.gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(KeyCode key in keyBindings.Keys)
        {
            var pair = keyBindings[key];
            if ((pair.Key == ButtonPressType.ALL && Input.GetKey(key))
             || (pair.Key == ButtonPressType.DOWN && Input.GetKeyDown(key))
             || (pair.Key == ButtonPressType.UP && Input.GetKeyUp(key)))
            {
                pair.Value();
            }
        }

        foreach (string key in buttonBindings.Keys)
        {
            var pair = buttonBindings[key];
            if ((pair.Key == ButtonPressType.ALL && Input.GetButton(key))
             || (pair.Key == ButtonPressType.DOWN && Input.GetButtonDown(key))
             || (pair.Key == ButtonPressType.UP && Input.GetButtonUp(key)))
            {
                pair.Value();
            }
        }

    }

    public void addKeyBinding(KeyCode key, ButtonPressType pressType, CallBack callback)
    {
        keyBindings.Add(key, new KeyValuePair < ButtonPressType, CallBack >(pressType, callback));
    }

    public void addButtonBinding(string key, ButtonPressType pressType, CallBack callback)
    {
        buttonBindings.Add(key, new KeyValuePair<ButtonPressType, CallBack>(pressType, callback));
    }

}


