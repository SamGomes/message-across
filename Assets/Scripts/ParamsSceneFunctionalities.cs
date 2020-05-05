using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class ParamsSceneFunctionalities : MonoBehaviour
{
    public Button startButton;
    public Button buttonPrefab;
    public GameObject paramsButtons;


    void Start()
    {
        StartCoroutine(YieldedStart());
    }
    
    // Start is called before the first frame update
    IEnumerator YieldedStart()
    {
        string generalConfigText = "";
        string exercisesConfigText = "";
        string scoreConfigText = "";
        
        
        
        string generalConfigPath = Application.streamingAssetsPath + "/generalConfig.cfg";
        string exercisesConfigPath = Application.streamingAssetsPath + "/exercisesConfig.cfg";

        
        if (generalConfigPath.Contains("://") || generalConfigPath.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(generalConfigPath);
            yield return www.SendWebRequest();
            generalConfigText = www.downloadHandler.text;
        }
        else
        {
            generalConfigText = File.ReadAllText(generalConfigPath);
        }

        
        if (exercisesConfigPath.Contains("://") || exercisesConfigPath.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(generalConfigPath);
            yield return www.SendWebRequest();
            exercisesConfigText = www.downloadHandler.text;
        }
        else
        {
            exercisesConfigText = File.ReadAllText(exercisesConfigPath);
        }

        //string json = JsonUtility.ToJson(settings, true);
        Globals.settings.generalSettings = JsonUtility.FromJson<GeneralSettings>(generalConfigText);
        Globals.settings.exercisesGroups = JsonUtility.FromJson<ExerciseGroupsWrapper>(exercisesConfigText);
        


        if (Globals.savedObjects == null)
        {
            Globals.InitGlobals();
        }
        startButton.onClick.AddListener(delegate () {
            SceneManager.LoadScene("mainScene");
        });



        
        List<ScoreSystemParam> scoreSystemParams = Globals.settings.generalSettings.scoreSystemParams;
        foreach (ScoreSystemParam param in scoreSystemParams)
        {
            Button button = Object.Instantiate(buttonPrefab, paramsButtons.transform);
            button.GetComponentInChildren<Text>().text = param.obfuscatedName;
        }
        
        
        
        
//        int i = 0;
//        foreach(Button button in paramsButtons)
//        {
//            button.GetComponentInChildren<Image>().color = new Color(1.0f, 1.0f, 1.0f);
//            int j = i;
//            button.onClick.AddListener(delegate () {
//                Globals.gameParam = (Globals.ExercisesConfig) j;
//                
//                string scoreConfigPath = ;
//                
//                if (scoreConfigPath.Contains("://") || scoreConfigPath.Contains(":///")) //url instead of path
//                {
//                    UnityWebRequest www = UnityWebRequest.Get(scoreConfigPath);
//                    yield return www.SendWebRequest();
//                    scoreConfigText = www.downloadHandler.text;
//                }
//                else
//                {
//                    scoreConfigText = File.ReadAllText(scoreConfigPath);
//                }
//                Globals.settings.scoreSystem = JsonUtility.FromJson<ScoreSystem>(scoreConfigText);
//
//
//                //update button colors
//                foreach (Button innerButton in paramsButtons)
//                {
//                    if (button == innerButton)
//                    {
//                        continue;
//                    }
//                    innerButton.GetComponentInChildren<Image>().color = new Color(1.0f, 1.0f, 1.0f);
//                }
//                button.GetComponentInChildren<Image>().color = new Color(0.0f, 1.0f, 0.0f);
//            });
//            i++;
//        }

        Globals.backgroundAudioManager.StopCurrentClip();
        Globals.backgroundAudioManager.PlayInfinitClip(Globals.backgroundMusicPath, Globals.backgroundMusicPath);
    }
}
