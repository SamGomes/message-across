using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager {

    private bool isGamePaused;
    private bool isGameLoaded;

    public void Init()
    {
        isGameLoaded = true;
        //initialConfigMenu.SetActive(false);
    }

    public void StartGame()
    {
        //set stuff
        isGamePaused = false;
        isGameLoaded = false;

        foreach (var obj in Globals.savedObjects)
        {
            Object.Destroy(obj);
        }
        SceneManager.LoadScene("start");

    }
    public void EndGame()
    {
        //reset stuff
        isGamePaused = false;
        isGameLoaded = false;

        Globals.backgroundAudioManager.PlayClip("Audio/backgroundLoop");
        SceneManager.LoadScene("gameover");

    }
}
