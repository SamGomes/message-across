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
        COMPETITIVE,
        INDIVIDUALISTIC,
        MUTUAL_HELP,
        P_ALTROISTIC,
        NEUTRAL
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

    public static KeyInteractionType[] keyInteractionTypes;
    public static ButtonId[] buttonIds;

    public static int currLevelId;
    public static string gameId;
    public static ExercisesConfig gameParam;
    internal static List<GameObject> savedObjects;

    public static AudioManager backgroundAudioManager;
    public static AudioManager effectsAudioManager;
    public static AudioManager trackEffectsAudioManager;
    public static GameSceneManager gameSceneManager;

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

        Globals.gameParam = Globals.ExercisesConfig.NEUTRAL;
        if (Globals.savedObjects == null)
        {
            Globals.savedObjects = new List<GameObject>();
        }

        Globals.backgroundAudioManager = new AudioManager(true);
        Globals.effectsAudioManager = new AudioManager(false);
        Globals.trackEffectsAudioManager = new AudioManager(false);
        Globals.gameSceneManager = new GameSceneManager();
    }
}


public class StartSceneFunctionalities : MonoBehaviour
{
    public Button startButton;
    public Button[] paramsButtons;

   
    // Start is called before the first frame update
    void Start()
    {
        Globals.InitGlobals();
        Globals.gameSceneManager.Init();
        Globals.backgroundAudioManager.StopCurrentClip();
        Globals.backgroundAudioManager.PlayInfinitClip("Audio/backgroundLoop", "Audio/backgroundLoop");

        startButton.onClick.AddListener(delegate () {
            SceneManager.LoadScene("mainScene");
        });
        

        int i = 0;
        foreach(Button button in paramsButtons)
        {
            button.GetComponentInChildren<Image>().color = new Color(1.0f, 1.0f, 1.0f);
            int j = i;
            button.onClick.AddListener(delegate () {
                Globals.gameParam = (Globals.ExercisesConfig)j;

                foreach (Button innerButton in paramsButtons)
                {
                    if (button == innerButton)
                    {
                        continue;
                    }
                    innerButton.GetComponentInChildren<Image>().color = new Color(1.0f, 1.0f, 1.0f);
                }
                button.GetComponentInChildren<Image>().color = new Color(0.0f, 1.0f, 0.0f);
            });
            i++;
        }
    }
}
