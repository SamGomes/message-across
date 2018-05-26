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

    Dictionary<KeyCode[], KeyValuePair< ButtonPressType, CallBack> > keyBindings = new Dictionary<KeyCode[], KeyValuePair<ButtonPressType, CallBack>>();
    Dictionary<string[], KeyValuePair< ButtonPressType, CallBack> > buttonBindings = new Dictionary<string[], KeyValuePair<ButtonPressType, CallBack>>();


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

    public void changeKeyBinding(KeyCode[] keys, ButtonPressType pressType, CallBack callback)
    {
        if (keyBindings.ContainsKey(keys))
        {
            keyBindings[keys] = new KeyValuePair<ButtonPressType, CallBack>(pressType, callback);
        }
            
    }
    public void changeButtonBinding(string[] keys, ButtonPressType pressType, CallBack callback)
    {
        if (buttonBindings.ContainsKey(keys))
        {
            buttonBindings[keys] = new KeyValuePair<ButtonPressType, CallBack>(pressType, callback);
        }
    }

    public void removeKeyBinding(KeyCode[] keys)
    {
        if (keyBindings.ContainsKey(keys))
        {
            keyBindings.Remove(keys);
        }

    }
    public void removeButtonBinding(string[] keys)
    {
        if (buttonBindings.ContainsKey(keys))
        {
            buttonBindings.Remove(keys);
        }
    }

    public void removeAllKeyBindings()
    {
        keyBindings = new Dictionary<KeyCode[], KeyValuePair<ButtonPressType, CallBack>>();
    }
    public void removeAllButtonBindings()
    {
        buttonBindings = new Dictionary<string[], KeyValuePair<ButtonPressType, CallBack>>();
    }

}


