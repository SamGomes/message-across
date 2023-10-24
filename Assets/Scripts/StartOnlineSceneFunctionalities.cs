using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartOnlineSceneFunctionalities : MonoBehaviour
{
    public Transform popupPositioner;
    public Camera worldCam;
    public InputField playerName;
    public InputField serverCode;
    public Button hostButton;
    public Button serverButton;
    public Button joinButton;


    void Start()
    {
        if (Globals.activeInfoPopups)
        {
            Popup popup = new Popup(false, worldCam, popupPositioner);
            popup.SetMessage("This is the online dashboard. On the left, you can select to host an online game. " +
                             "Your ip is presented when you host a game." +
                             "On the right, you can join a game given a host IP address.");
//        popup.AddButton("Sure!", delegate
//        {
//            popup.HidePopupPanel();
//            return 0;
//        });
//        popup.HasCloseButton(false);
            popup.DisplayPopup();
        }

        if (Globals.settings.networkSettings.currOnlineOption == "HOST" ||
            Globals.settings.networkSettings.currOnlineOption == "SERVER")
        {
            SceneManager.LoadScene("paramsSetup");
        }
        else if (Globals.settings.networkSettings.currOnlineOption == "CLIENT")
        {
            SceneManager.LoadScene("mainScene");
        }
        else
        {
            Globals.settings.networkSettings.currMultiplayerOption = "ONLINE";
            hostButton.onClick.AddListener(delegate()
            {
                Globals.settings.networkSettings.currOnlineOption = "HOST";
                SceneManager.LoadScene("paramsSetup");
            });
            serverButton.onClick.AddListener(delegate()
            {
                Globals.settings.networkSettings.currOnlineOption = "SERVER";
                SceneManager.LoadScene("paramsSetup");
            });

            joinButton.onClick.AddListener(delegate()
            {
                Globals.settings.networkSettings.currOnlineOption = "CLIENT";
                Globals.settings.networkSettings.serverCode = serverCode.text;
                SceneManager.LoadScene("mainScene");
            });
        }
    }
}
