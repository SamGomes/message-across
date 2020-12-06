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

    public static AudioManager backgroundAudioManager;
    public static AudioManager effectsAudioManager;
    public static AudioManager trackEffectsAudioManager;

    public static LogManager logManager;


    public static List<string> bufferedPlayerIds;

    public static string backgroundMusicPath;

    
    public static IEnumerator LerpAnimation(GameObject source, Vector3 targetPos, float speed)
    {
        Vector3 sourcePos = source.transform.position;
        float totalDist = (targetPos - sourcePos).sqrMagnitude;
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

        Globals.backgroundAudioManager = new AudioManager(true);
        Globals.effectsAudioManager = new AudioManager(false);
        Globals.trackEffectsAudioManager = new AudioManager(false);

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
        Globals.backgroundAudioManager.StopCurrentClip();
        Globals.backgroundAudioManager.PlayInfinitClip(Globals.backgroundMusicPath,Globals.backgroundMusicPath);
    }
}
