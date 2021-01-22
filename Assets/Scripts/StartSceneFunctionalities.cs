using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Globals
{
    public enum ExercisesConfig
    {
        TUTORIAL,
        CUSTOM
    }

    public enum KeyInteractionType
    {
        NONE,
        TAKE,
        GIVE
    }


    public enum DiffLetters
    {
        HIGHER,
        EQUAL,
        LOWER
    }
    
    
    public static GameSettings settings;

    public static int currLevelId;
    public static string gameId;
    public static ExercisesConfig gameParam;

    public static List<AudioManager> audioManagers;
    public static string backgroundMusicPath;
    public static LogManager logManager;

    public static List<string> bufferedPlayerIds;
    
    public static bool activeInfoPopups;

    public static List<GameObject> savedGameObjects;
    
    public static IEnumerator LerpAnimation(GameObject source, Vector3 targetPos, float speed)
    {
        Vector3 sourcePos = source.transform.position;
        float currT = 0;
        while (currT < 1.0f && source != null)
        {
            source.transform.position = Vector3.Lerp(sourcePos, targetPos, currT += speed * 0.025f);
            yield return new WaitForSeconds(0.025f);
        }
    }
    

    //settings and properties that derive from user choice are only
    // updated upon user score system choice
    public static void InitGlobals()
    {
        currLevelId = 0;

        gameId = "";
        for (int i = 0; i < 20; i++)
        {
            gameId += (char)('A' + UnityEngine.Random.Range(0, 26));
        }

        gameParam = ExercisesConfig.TUTORIAL;
        if (savedGameObjects == null)
        {
            savedGameObjects = new List<GameObject>();
            
            audioManagers = new List<AudioManager>();
            audioManagers.Add(new AudioManager(true));
            audioManagers.Add(new AudioManager(false));
            audioManagers.Add(new AudioManager(false));
            
            backgroundMusicPath = "Audio/backgroundLoop";
        }

        if(bufferedPlayerIds == null)
        {
            bufferedPlayerIds = new List<string>();
        }

    }
}



public class StartSceneFunctionalities : MonoBehaviour
{
    public Transform popupPositioner;
    public Camera camera;
    
    public Button localButton;
    public Button onlineButton;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(YieldedStart());
    }
    
     private IEnumerator YieldedStart(){
         
         Globals.InitGlobals();

         string scoreConfigPath =  ""; 
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
                 Debug.Log("Caught FileNotFoundException exception: " + e.Message);
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
                Debug.Log("Caught FileNotFoundException exception: " + e.Message);
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
            Debug.Log("Caught FileNotFoundException exception: " + e.Message);
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



        string lobbyScoreConfigPath = Application.streamingAssetsPath + "/scoreSystemConfigLobby.cfg";
        string lobbyExercisesConfigPath = Application.streamingAssetsPath + "/exercisesConfigLobby.cfg";

        string lobbyScoreConfigText = "";
        if (scoreConfigPath.Contains("://") || lobbyScoreConfigPath.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(lobbyScoreConfigPath);
            yield return www.SendWebRequest();
            lobbyScoreConfigText = www.downloadHandler.text;
        }
        else
        {
            lobbyScoreConfigText = File.ReadAllText(lobbyScoreConfigPath);
        }
        Globals.settings.lobbyScoreSystem = JsonUtility.FromJson<ScoreSystem>(lobbyScoreConfigText);


        string lobbyExercisesConfigText = "";
        if (scoreConfigPath.Contains("://") || lobbyScoreConfigPath.Contains(":///")) //url instead of path
        {
            UnityWebRequest www = UnityWebRequest.Get(lobbyScoreConfigPath);
            yield return www.SendWebRequest();
            lobbyExercisesConfigText = www.downloadHandler.text;
        }
        else
        {
            lobbyExercisesConfigText = File.ReadAllText(lobbyExercisesConfigPath);
        }
        Globals.settings.lobbyExercisesGroups = JsonUtility.FromJson<ExerciseGroupsWrapper>(lobbyExercisesConfigText);




        //start
        if (Globals.settings.networkSettings.currMultiplayerOption == "ONLINE")
        {
            SceneManager.LoadScene("startOnline");
        }else if (Globals.settings.networkSettings.currMultiplayerOption == "LOCAL")
        {
            SceneManager.LoadScene("startLocal");
        }
        else
        {
            localButton.onClick.AddListener(delegate () {
                SceneManager.LoadScene("startLocal");
            });
            onlineButton.onClick.AddListener(delegate () {
                SceneManager.LoadScene("startOnline");
            });
        }


        Popup popup = new Popup(false, camera, popupPositioner);
        popup.SetOnHide(delegate
        {
            Popup popup2 = new Popup(false, camera, popupPositioner);
            popup2.SetMessage("Would you like to enable messages explaining the game screens?");
            popup2.AddButton("Sure!", delegate
            {
                Globals.activeInfoPopups = true;
                popup2.HidePopupPanel();
                return 0;
            });
            popup2.AddButton("No", delegate
            {
                Globals.activeInfoPopups = false;
                popup2.HidePopupPanel();
                return 0;
            });
            popup2.HasCloseButton(false);
            popup2.DisplayPopup();
            return 0;
        });
        popup.SetMessage("Welcome to MessageAcross ver 2.1! This version contemplates the new online mode " +
                          "(offline is not available yet in this version), and a new word display interface, " +
                          "focused on the game application for learning. Enjoy!");
        popup.AddButton("OK", delegate
        {
            popup.HidePopupPanel();
            return 0;
        });
        popup.HasCloseButton(false);
        popup.DisplayPopup();

        Globals.audioManagers[0].StopCurrentClip();
        Globals.audioManagers[0].PlayInfiniteClip(
        Globals.backgroundMusicPath,
        Globals.backgroundMusicPath);
        Globals.audioManagers[0].GetSource().pitch = 1.1f;
            
     }
}
