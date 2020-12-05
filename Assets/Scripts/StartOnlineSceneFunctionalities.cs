using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartOnlineSceneFunctionalities : MonoBehaviour
{
    public InputField playerName;
    public Button hostButton;
    public Button joinButton;
    public InputField joinIP;


    void Start()
    {
        hostButton.onClick.AddListener(delegate () {
            SceneManager.LoadScene("paramsSetup");
        });
        
        joinButton.onClick.AddListener(delegate () {
            SceneManager.LoadScene("paramsSetup");
        });
    }
}
