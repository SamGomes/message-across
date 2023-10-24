using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class ParamsSceneFunctionalities : MonoBehaviour
{
    public Transform popupPositioner;
    public Camera worldCam;
    
    public Button startButton;
    public Button buttonPrefab;
    public GameObject paramsButtonsObject;
    private string scoreConfigPath;

    void Start()
    {
        string selectedScoreName = Globals.settings.networkSettings.selectedScoreName;
        if (selectedScoreName.Length != 0)
        {
            scoreConfigPath = selectedScoreName;
            scoreConfigPath = Application.streamingAssetsPath + "/" + selectedScoreName + ".cfg";
            StartCoroutine(LoadScoreConfigAndStart());
        }
        if (Globals.activeInfoPopups)
        {
            Popup popup = new Popup(false, worldCam, popupPositioner);
            popup.SetMessage("Welcome to the version selection menu. " +
                             "Here the host can select one of the game versions, " +
                             "currently loaded into the game," +
                             " and start a game session (open the game room).");
            popup.DisplayPopup();
        }
        
        //
        // if (Globals.savedObjects == null)
        // {
        //     Globals.InitGlobals();
        // }
        startButton.onClick.AddListener(delegate ()
        {
            StartCoroutine(LoadScoreConfigAndStart());
        });

        List<ScoreSystemParam> scoreSystemParams = Globals.settings.generalSettings.scoreSystemParams;
        
        //generate tutorial button
        Button tbutton = Instantiate(buttonPrefab, paramsButtonsObject.transform);
        tbutton.GetComponentInChildren<Text>().text = "T";
        tbutton.onClick.AddListener(delegate () {
            scoreConfigPath =  Application.streamingAssetsPath + "/scoreSystemConfigTutorial.cfg";
            Globals.gameParam = Globals.ExercisesConfig.TUTORIAL;
            UpdateButtonColors(tbutton);
            //update button colors
            
        });
        
        //generate other buttons
        foreach (ScoreSystemParam param in scoreSystemParams)
        {
            Button button = Object.Instantiate(buttonPrefab, paramsButtonsObject.transform);
            button.GetComponentInChildren<Text>().text = param.prefix;
            
            button.onClick.AddListener(delegate () { 
                Globals.gameParam = Globals.ExercisesConfig.CUSTOM;
                string scoreConfigName = Globals.settings.generalSettings.scoreSystemParams[scoreSystemParams.FindIndex(a => a == param)].filename;;
                scoreConfigPath = Application.streamingAssetsPath + "/" + scoreConfigName + ".cfg";
                UpdateButtonColors(button);
            });
        }
        
        Globals.audioManagers[0].StopCurrentClip();
        Globals.audioManagers[0].PlayInfiniteClip(
            Globals.backgroundMusicPath,
            Globals.backgroundMusicPath);
    }
    
    private void UpdateButtonColors(Button selectedButton)
    {
        Button[] paramsButtons = paramsButtonsObject.GetComponentsInChildren<Button>();
        foreach (Button innerButton in paramsButtons)
        {
            if (selectedButton == innerButton)
            {
                continue;
            }
            innerButton.GetComponentInChildren<Image>().color = new Color(1.0f, 1.0f, 1.0f);
        }
        selectedButton.GetComponentInChildren<Image>().color = new Color(0.0f, 1.0f, 0.0f);
    }

    IEnumerator LoadScoreConfigAndStart()
    {
        string scoreConfigText = "";
        if (scoreConfigPath.Contains("://") || scoreConfigPath.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(scoreConfigPath);
            yield return www.SendWebRequest();
            scoreConfigText = www.downloadHandler.text;
        }
        else
        {
            scoreConfigText = File.ReadAllText(scoreConfigPath);
        }
        
        Globals.settings.scoreSystem = JsonUtility.FromJson<ScoreSystem>(scoreConfigText);
        Globals.settings.scoreSystem.path = scoreConfigPath;
        
        //start after waiting for data load
        SceneManager.LoadScene("mainScene");
    }

}
