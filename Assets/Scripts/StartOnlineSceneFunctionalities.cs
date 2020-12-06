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
        Globals.settings.networkSettings.currMultiplayerOption = "ONLINE";
        hostButton.onClick.AddListener(delegate () {
            Globals.settings.networkSettings.currOnlineOption = "HOST";
            SceneManager.LoadScene("paramsSetup");
        });
        
        joinButton.onClick.AddListener(delegate () {
            Globals.settings.networkSettings.currOnlineOption = "CLIENT";
            SceneManager.LoadScene("paramsSetup");
        });
    }
}
