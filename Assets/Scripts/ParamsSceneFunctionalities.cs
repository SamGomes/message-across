using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class ParamsSceneFunctionalities : MonoBehaviour
{
    public Button startButton;
    public Button buttonPrefab;
    public GameObject paramsButtonsObject;
    private string scoreConfigPath;


    void Start()
    {
        StartCoroutine(YieldedStart());
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

    IEnumerator LoadScoreConfig()
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
    }
    
    // Start is called before the first frame update
    IEnumerator YieldedStart()
    {
        scoreConfigPath =  Application.streamingAssetsPath + "/scoreSystemConfigTutorial.cfg";
        string generalConfigText = "";
        string exercisesConfigText = "";
        string networkConfigText = "";

        string generalConfigPath = Application.streamingAssetsPath + "/generalConfig.cfg";
        string exercisesConfigPath = Application.streamingAssetsPath + "/exercisesConfig.cfg";
        
        string networkConfigPath = Application.streamingAssetsPath + "/networkConfig.cfg";

        
        if (generalConfigPath.Contains("://") || generalConfigPath.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(generalConfigPath);
            yield return www.SendWebRequest();
            generalConfigText = www.downloadHandler.text;
        }
        else
        {
            try
            {
                generalConfigText = File.ReadAllText(generalConfigPath);
            }
            catch (FileNotFoundException e)
            {
                generalConfigText = "";
            }
        }

        
        if (exercisesConfigPath.Contains("://") || exercisesConfigPath.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(generalConfigPath);
            yield return www.SendWebRequest();
            exercisesConfigText = www.downloadHandler.text;
        }
        else
        {
            try
            {
                exercisesConfigText = File.ReadAllText(exercisesConfigPath);
            }
            catch (FileNotFoundException e)
            {
                exercisesConfigText = "";
            }
        }
        
        
        if (networkConfigPath.Contains("://") || networkConfigPath.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(generalConfigPath);
            yield return www.SendWebRequest();
            networkConfigText = www.downloadHandler.text;
        }
        else
        {
            try
            {
                networkConfigText = File.ReadAllText(networkConfigPath);
            }
            catch (FileNotFoundException e)
            {
                networkConfigText = "";
            }
        }

        if (generalConfigText == "" || exercisesConfigText == "")
        {
            Debug.LogError("Game cannot start without general or exercise configurations!");
            yield return null;
        }

        //string json = JsonUtility.ToJson(settings, true);
        Globals.settings.generalSettings = JsonUtility.FromJson<GeneralSettings>(generalConfigText);
        Globals.settings.exercisesGroups = JsonUtility.FromJson<ExerciseGroupsWrapper>(exercisesConfigText);

        if (networkConfigText != "")
        {
            //auto config
            Globals.settings.networkSettings = JsonUtility.FromJson<MANetworkSettings>(networkConfigText);
        }


        
        
        
        if (Globals.savedObjects == null)
        {
            Globals.InitGlobals();
        }
        startButton.onClick.AddListener(delegate ()
        {
            StartCoroutine(LoadScoreConfig());
            SceneManager.LoadScene("mainScene");
        });



        
        List<ScoreSystemParam> scoreSystemParams = Globals.settings.generalSettings.scoreSystemParams;
        
        //generate tutorial button
        Button tbutton = Object.Instantiate(buttonPrefab, paramsButtonsObject.transform);
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

}
