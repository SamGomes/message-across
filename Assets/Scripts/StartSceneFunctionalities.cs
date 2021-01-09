using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Globals
{
    public enum ExercisesConfig
    {
        TUTORIAL,
        CUSTOM
    }

    public enum PlayerId
    {
        PLAYER_0,
        PLAYER_1,
        PLAYER_2,
        NONE
    }

    public enum ButtonId
    {
        BTN_0,
        BTN_1,
        BTN_2,
        NONE
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
    
    public static KeyInteractionType[] keyInteractionTypes;
    public static ButtonId[] buttonIds;

    public static int currLevelId;
    public static string gameId;
    public static ExercisesConfig gameParam;
    public static List<GameObject> savedObjects;

    public static List<AudioManager> audioManagers;

    public static LogManager logManager;

    public static List<string> bufferedPlayerIds;

    public static string backgroundMusicPath;

    public static bool activeInfoPopups;
    
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
        Globals.keyInteractionTypes = (Globals.KeyInteractionType[])Enum.GetValues(typeof(Globals.KeyInteractionType));
        Globals.buttonIds = (Globals.ButtonId[])Enum.GetValues(typeof(Globals.ButtonId));

        Globals.currLevelId = 0;

        Globals.gameId = "";
        for (int i = 0; i < 20; i++)
        {
            Globals.gameId += (char)('A' + UnityEngine.Random.Range(0, 26));
        }

        Globals.gameParam = Globals.ExercisesConfig.TUTORIAL;
        if (Globals.savedObjects == null)
        {
            Globals.savedObjects = new List<GameObject>();
        }

        Globals.audioManagers = new List<AudioManager>();
        Globals.audioManagers.Add(new AudioManager(true));
        Globals.audioManagers.Add(new AudioManager(false));
        Globals.audioManagers.Add(new AudioManager(false));

        if(Globals.bufferedPlayerIds == null)
        {
            Globals.bufferedPlayerIds = new List<string>();
        }

        Globals.backgroundMusicPath = "Audio/backgroundLoop";
    }
}

public class StartSceneFunctionalities : MonoBehaviour
{
    public Button localButton;
    public Button onlineButton;

    // Start is called before the first frame update
    void Start()
    {
        localButton.onClick.AddListener(delegate () {
            SceneManager.LoadScene("startLocal");
        });
        onlineButton.onClick.AddListener(delegate () {
            SceneManager.LoadScene("startOnline");
        });
        Globals.InitGlobals();
        
        
        Popup popup = new Popup(false);
        popup.SetMessage("Welcome to MessageAcross ver 2.1! This version contemplates the new online mode " +
                         "(offline is not currently working in this version), and a new word display interface, " +
                         "focused on the game application for learning. Enjoy!" +
                         "\n\n" +
                         "Would you like to enable messages explaining the game screens?");
        popup.AddButton("Sure!", delegate
        {
            Globals.activeInfoPopups = true;
            popup.HidePopupPanel();
            return 0;
        });
        popup.AddButton("No", delegate
        {
            Globals.activeInfoPopups = false;
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
