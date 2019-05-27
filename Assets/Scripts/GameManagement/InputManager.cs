using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class InputManager : MonoBehaviour {

    public enum ButtonPressType
    {
        PRESSED,
        DOWN,
        UP,
    }

    public GameManager gameManager;
    public GameSceneManager gameSceneManager;

    public delegate void CallBack();

    Dictionary<List<KeyCode>, KeyValuePair< ButtonPressType, CallBack> > keyBindings;
    Dictionary<List<string>, KeyValuePair< ButtonPressType, CallBack> > buttonBindings;

    List<KeyCode> currPressedKeys = new List<KeyCode>();
    
    public void InitKeys()
    {
        this.RemoveAllKeyBindings();
        this.AddKeyBinding(new List<KeyCode> { KeyCode.Space, KeyCode.A }, InputManager.ButtonPressType.DOWN, delegate () { gameSceneManager.StartAndPauseGame(); });
        this.AddKeyBinding(new List<KeyCode> { KeyCode.Space, KeyCode.B }, InputManager.ButtonPressType.UP, delegate () { gameSceneManager.StartAndPauseGame(); });
        this.AddButtonBinding(new List<string> { "Start" }, InputManager.ButtonPressType.DOWN, delegate () { gameSceneManager.StartAndPauseGame(); });
        
        //    inputManager.addKeyBinding(new KeyCode[] { KeyCode.Q }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        //    inputManager.addKeyBinding(new KeyCode[] { KeyCode.W }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        //    inputManager.addKeyBinding(new KeyCode[] { KeyCode.O }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
        //    inputManager.addKeyBinding(new KeyCode[] { KeyCode.P }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });

        //    inputManager.addButtonBinding(new string[] { "YButtonJoy1" }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        //    inputManager.addButtonBinding(new string[] { "BButtonJoy1" }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_0); });
        //    inputManager.addButtonBinding(new string[] { "YButtonJoy2" }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_0].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
        //    inputManager.addButtonBinding(new string[] { "BButtonJoy2" }, InputManager.ButtonPressType.ALL, delegate () { gameButtons[(int)Utilities.ButtonId.BTN_1].registerUserButtonPress(Utilities.PlayerId.PLAYER_1); });
        //
    }

    void Awake() //before any start init stuff
    {
        keyBindings = new Dictionary<List<KeyCode>, KeyValuePair<ButtonPressType, CallBack>>();
        buttonBindings = new Dictionary<List<string>, KeyValuePair<ButtonPressType, CallBack>>();
        InitKeys();
    }
  

    // Update is called once per frame
    void Update()
    {
        List<KeyCode> bufferMod = new List<KeyCode>();
        List<KeyCode> oldPressedKeys = currPressedKeys;
        currPressedKeys = new List<KeyCode>();
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(key))
            {
                currPressedKeys.Add(key);
            }
        }

        bool keysDown = !currPressedKeys.TrueForAll(oldPressedKeys.Contains);
        bool keysUp = !oldPressedKeys.TrueForAll(currPressedKeys.Contains) || currPressedKeys.Count == 0;
        if (currPressedKeys.Count > 0 && (keysDown || keysUp))
        {
            List<KeyCode> allKeysPressed = oldPressedKeys;
            allKeysPressed.AddRange(currPressedKeys);
            bufferMod = allKeysPressed.Distinct().ToList();
            //bufferMod = bufferMod.Distinct().ToList();
        }

        foreach (List<KeyCode> simultaneouskeysList in keyBindings.Keys)
        {
            var pair = keyBindings[simultaneouskeysList];
            switch (pair.Key)
            {
                case ButtonPressType.DOWN:
                    if (!(simultaneouskeysList.TrueForAll(bufferMod.Contains) && keysDown))
                    {
                        continue;
                    }
                    break;
                case ButtonPressType.UP:
                    if (!(simultaneouskeysList.TrueForAll(bufferMod.Contains) && keysUp))
                    {
                        continue;
                    }
                    break;
                case ButtonPressType.PRESSED:
                    if (!simultaneouskeysList.TrueForAll(currPressedKeys.Contains))
                    {
                        continue;
                    }
                    break;
            }
            pair.Value();
        }

        
    }

    public void AddKeyBinding(List<KeyCode> keys, ButtonPressType pressType, CallBack callback)
    {
        keyBindings.Add(keys, new KeyValuePair < ButtonPressType, CallBack >(pressType, callback));
    }
    public void AddButtonBinding(List<string> keys, ButtonPressType pressType, CallBack callback)
    {
        buttonBindings.Add(keys, new KeyValuePair<ButtonPressType, CallBack>(pressType, callback));
    }

    public void ChangeKeyBinding(List<KeyCode> keys, ButtonPressType pressType, CallBack callback)
    {
        if (keyBindings.ContainsKey(keys))
        {
            keyBindings[keys] = new KeyValuePair<ButtonPressType, CallBack>(pressType, callback);
        }
            
    }
    public void ChangeButtonBinding(List<string> keys, ButtonPressType pressType, CallBack callback)
    {
        if (buttonBindings.ContainsKey(keys))
        {
            buttonBindings[keys] = new KeyValuePair<ButtonPressType, CallBack>(pressType, callback);
        }
    }

    public void RemoveKeyBinding(List<KeyCode> keys)
    {
        if (keyBindings.ContainsKey(keys))
        {
            keyBindings.Remove(keys);
        }

    }
    public void RemoveButtonBinding(List<string> keys)
    {
        if (buttonBindings.ContainsKey(keys))
        {
            buttonBindings.Remove(keys);
        }
    }

    public void RemoveAllKeyBindings()
    {
        keyBindings = new Dictionary<List<KeyCode>, KeyValuePair<ButtonPressType, CallBack>>();
    }
    public void RemoveAllButtonBindings()
    {
        buttonBindings = new Dictionary<List<string>, KeyValuePair<ButtonPressType, CallBack>>();
    }

}


