using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class InputManager : MonoBehaviour {

    public struct KeyBindingData
    {
        public List<KeyCode> keyCodes;
        public ButtonPressType pressType;
        public CallBack callback;
        public bool isOrdered;
    }


    public enum ButtonPressType
    {
        PRESSED,
        DOWN,
        UP,
    }

    public GameManager gameManager;
    public GameSceneManager gameSceneManager;

    public delegate void CallBack();

    private Dictionary<List<KeyCode>, KeyBindingData> keyBindings;

    private List<KeyCode> currPressedKeys = new List<KeyCode>();
    private List<KeyCode> bufferMod;

    private KeyCode lastPressedKey;

    public List<KeyCode> GetCurrPressedKeys()
    {
        return bufferMod;
    }
    public List<KeyCode> GetBufferMod()
    {
        return bufferMod;
    }


    public void InitKeys()
    {
        this.RemoveAllKeyBindings();
        this.AddKeyBinding(new List<KeyCode> { KeyCode.Space }, InputManager.ButtonPressType.DOWN, delegate () { gameSceneManager.StartAndPauseGame(); }, false);
        this.AddKeyBinding(new List<KeyCode> { KeyCode.A, KeyCode.B }, InputManager.ButtonPressType.DOWN, delegate () { Debug.Log("A"); }, true);
        this.AddKeyBinding(new List<KeyCode> { KeyCode.B, KeyCode.A }, InputManager.ButtonPressType.DOWN, delegate () { Debug.Log("B"); }, true);
        this.AddKeyBinding(new List<KeyCode> { KeyCode.C, KeyCode.D }, InputManager.ButtonPressType.PRESSED, delegate () { Debug.Log("c"); }, false);
        //this.AddKeyBinding(new HashSet<KeyCode> { KeyCode.Space, KeyCode.A }, InputManager.ButtonPressType.DOWN, delegate () { gameSceneManager.StartAndPauseGame(); });
        //this.AddKeyBinding(new HashSet<KeyCode> { KeyCode.Space, KeyCode.B }, InputManager.ButtonPressType.UP, delegate () { gameSceneManager.StartAndPauseGame(); });
        //this.AddButtonBinding(new HashSet<string> { "Start" }, InputManager.ButtonPressType.DOWN, delegate () { gameSceneManager.StartAndPauseGame(); });

        
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
        keyBindings = new Dictionary<List<KeyCode>, KeyBindingData>();
        InitKeys();
    }

    public bool ContainsSequence<T>(IEnumerable<T> source, IEnumerable<T> other)
    {
        int count = other.Count();

        while (source.Any())
        {
            if (source.Take(count).SequenceEqual(other))
                return true;
            source = source.Skip(1);
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        bufferMod = new List<KeyCode>();
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
            lastPressedKey = bufferMod[bufferMod.Count - 1];
            //bufferMod = bufferMod.Distinct().ToList();
        }


        foreach (List<KeyCode> simultaneouskeysList in keyBindings.Keys)
        {
            if(bufferMod.Count > simultaneouskeysList.Count || currPressedKeys.Count > simultaneouskeysList.Count)
            {
                continue;
            }

            var pair = keyBindings[simultaneouskeysList];
            
            switch (pair.pressType)
            {
                case ButtonPressType.DOWN:
                    if (!(simultaneouskeysList.TrueForAll(bufferMod.Contains) && keysDown))
                    {
                        continue;
                    }
                    else
                    {
                        if (bufferMod.Count > 0 && pair.isOrdered && !ContainsSequence(simultaneouskeysList, bufferMod))
                        {
                            continue;
                        }
                    }
                    break;
                case ButtonPressType.UP:
                    if (!(simultaneouskeysList.TrueForAll(bufferMod.Contains) && keysUp))
                    {
                        continue;
                    }
                    else
                    {
                        if (bufferMod.Count > 0 && pair.isOrdered && !ContainsSequence(simultaneouskeysList, bufferMod))
                        {
                            continue;
                        }
                    }
                    break;
                case ButtonPressType.PRESSED:
                    if (!(simultaneouskeysList.TrueForAll(currPressedKeys.Contains)))
                    {
                        continue;
                    }
                    else
                    {
                        if (pair.isOrdered && !ContainsSequence(simultaneouskeysList, currPressedKeys))
                        {
                            continue;
                        }
                    }
                    break;
            }
            pair.callback();
            break;
        }

        
    }

    public void AddKeyBinding(List<KeyCode> keys, ButtonPressType pressType, CallBack callback, bool isUnordered)
    {
        keyBindings.Add(keys, new KeyBindingData() { pressType = pressType, callback = callback, isOrdered = isUnordered});
    }
    
    public void ChangeKeyBinding(List<KeyCode> keys, ButtonPressType pressType, CallBack callback, bool isUnordered)
    {
        if (keyBindings.ContainsKey(keys))
        {
            keyBindings[keys] = new KeyBindingData() { pressType = pressType, callback = callback, isOrdered = isUnordered };
        }
            
    }
    
    public void RemoveKeyBinding(List<KeyCode> keys)
    {
        if (keyBindings.ContainsKey(keys))
        {
            keyBindings.Remove(keys);
        }

    }

    public void RemoveAllKeyBindings()
    {
        keyBindings.Clear();
    }
    

    public KeyCode GetLastPressedKey()
    {
        return this.lastPressedKey;
    }

    internal bool ContainsKeyBinding(List<KeyCode> generatedKeyCombo)
    {
        foreach(List<KeyCode> combo in keyBindings.Keys)
        {
            bool comboContains = true;
            foreach(KeyCode key in generatedKeyCombo)
            {
                if (!combo.Contains(key))
                {
                    comboContains = false;
                }
            }
            if (comboContains)
            {
                return true;
            }
        }
        return false;
    }
}


