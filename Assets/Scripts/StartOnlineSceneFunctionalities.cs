using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartOnlineSceneFunctionalities : MonoBehaviour
{
    public InputField playerName;
    public InputField serverCode;
    public Button hostButton;
    public Button joinButton;


    void Start()
    {
        if (Globals.activeInfoPopups)
        {
            Popup popup = new Popup(false);
            popup.SetMessage("This is the online dashboard. On the left, you can select to host an online game. " +
                             "A game code is presented when you host a game." +
                             "On the right, you can join a game given a host IP address.");
//        popup.AddButton("Sure!", delegate
//        {
//            popup.HidePopupPanel();
//            return 0;
//        });
//        popup.HasCloseButton(false);
            popup.DisplayPopup();
        }

        Globals.settings.networkSettings.currMultiplayerOption = "ONLINE";
        hostButton.onClick.AddListener(delegate () {
            Globals.settings.networkSettings.currOnlineOption = "HOST";
            SceneManager.LoadScene("paramsSetup");
        });
        
        joinButton.onClick.AddListener(delegate () {
            Globals.settings.networkSettings.currOnlineOption = "CLIENT";
            Globals.settings.networkSettings.serverCode = serverCode.text;
            SceneManager.LoadScene("mainScene");
        });
    }
}
