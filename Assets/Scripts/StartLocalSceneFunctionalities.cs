using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class StartLocalSceneFunctionalities : MonoBehaviour
{
    public InputField playerNames;
    public Button startButton;


    void Start()
    {
        startButton.onClick.AddListener(delegate () {
            Globals.bufferedPlayerIds = new List<string>(playerNames.text.Split(';'));
            SceneManager.LoadScene("paramsSetup");
        });
    }
}
