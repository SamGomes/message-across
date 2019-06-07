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

    public delegate void CallBack(List<KeyCode> triggeredKeys);

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
        //this.RemoveAllKeyBindings();
        //this.AddKeyBinding(new List<KeyCode> { KeyCode.A, KeyCode.B }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys) { Debug.Log("A"); }, true);
        ////this.AddKeyBinding(new List<KeyCode> { KeyCode.B, KeyCode.A }, InputManager.ButtonPressType.DOWN, delegate () { Debug.Log("B"); }, true);
        ////this.AddKeyBinding(new List<KeyCode> { KeyCode.C, KeyCode.D }, InputManager.ButtonPressType.PRESSED, delegate () { Debug.Log("c"); }, false);

        //this.AddKeyBinding(new List<KeyCode> { KeyCode.U, (KeyCode)(-1) }, InputManager.ButtonPressType.UP, delegate (List<KeyCode> triggeredKeys) { Debug.Log("allU"); }, false);
        //this.AddKeyBinding(new List<KeyCode> { KeyCode.D, (KeyCode)(-1) }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> triggeredKeys) { Debug.Log("allD"); }, false);
        //this.AddKeyBinding(new List<KeyCode> { KeyCode.P, (KeyCode)(-1) }, InputManager.ButtonPressType.PRESSED, delegate (List<KeyCode> triggeredKeys) { Debug.Log("allP"); }, false);

        //this.AddKeyBinding(
        //      new List<KeyCode>() { (KeyCode)(-1), (KeyCode)(-1) }, InputManager.ButtonPressType.DOWN, delegate (List<KeyCode> pressedKeys)
        //      {
        //          Debug.Log("AAA");
        //      }, false);
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
                    if (bufferMod.Count < simultaneouskeysList.Count)
                    {
                        continue;
                    }


                    if (!(simultaneouskeysList.TrueForAll(delegate(KeyCode key) {
                        return (bufferMod.Contains(key) || key == (KeyCode)(-1));
                    }) && keysDown))
                    {
                        continue;
                    }
                    else if (bufferMod.Count > 0 && pair.isOrdered && !ContainsSequence(simultaneouskeysList, bufferMod)){
                            continue;
                    }

                    pair.callback(bufferMod);
                    break;
                case ButtonPressType.UP:
                    if (bufferMod.Count < simultaneouskeysList.Count)
                    {
                        continue;
                    }


                    if (!(simultaneouskeysList.TrueForAll(delegate (KeyCode key) {
                        return (bufferMod.Contains(key) || key == (KeyCode)(-1));
                    }) && keysUp))
                    {
                        continue;
                    }
                    else if (bufferMod.Count > 0 && pair.isOrdered && !ContainsSequence(simultaneouskeysList, bufferMod))
                    {
                        continue;
                    }

                    pair.callback(bufferMod);
                    break;
                case ButtonPressType.PRESSED:
                    if (currPressedKeys.Count < simultaneouskeysList.Count)
                    {
                        continue;
                    }


                    if (!(simultaneouskeysList.TrueForAll(delegate (KeyCode key) {
                        return (currPressedKeys.Contains(key) || key == (KeyCode)(-1));
                    })))
                    {
                        continue;
                    }
                    else if (pair.isOrdered && !ContainsSequence(simultaneouskeysList, currPressedKeys))
                    {
                        continue;
                    }

                    pair.callback(currPressedKeys);
                    break;
            }
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


