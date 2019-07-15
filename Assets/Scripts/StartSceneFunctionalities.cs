using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneFunctionalities : MonoBehaviour
{
    public InputManager inputManager;
    // Start is called before the first frame update
    void Start()
    {
        inputManager.AddKeyBinding(
                    new List<KeyCode>() { KeyCode.Space }, InputManager.ButtonPressType.PRESSED, delegate (List<KeyCode> triggeredKeys)
                    {
                        SceneManager.LoadScene("mainScene");
                    }, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
