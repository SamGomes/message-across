using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour {

    private bool isGamePaused;
    private bool isGameLoaded;
    public GameManager gameManager;

    private GameObject currPauseMenu;
    private GameObject plainPauseMenu;
    private GameObject initialConfigMenu;
    

    public void MainSceneLoadedNotification()
    {
        isGameLoaded = true;
        this.plainPauseMenu = GameObject.Find("CanvasForUI/PauseCanvas");
        this.initialConfigMenu = GameObject.Find("CanvasForUI/InitialConfigPauseCanvas");
        //initialConfigMenu.SetActive(false);
        this.currPauseMenu = initialConfigMenu;
        Object.DontDestroyOnLoad(this);
        Globals.savedObjects.Add(this.gameObject);
    }

    public void StartGame()
    {
        //set stuff
        isGamePaused = false;
        isGameLoaded = false;
        StartCoroutine(DelayedRestartGame(3.0f));

    }
    public void EndGame()
    {
        //reset stuff
        isGamePaused = false;
        isGameLoaded = false;

        SceneManager.LoadScene("gameover");
        StartCoroutine(DelayedQuitGame(3.0f));

    }

    public IEnumerator DelayedQuitGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        Application.Quit();
    }
    public IEnumerator DelayedRestartGame(float delay)
    {
        yield return DelayedQuitGame(delay);
        foreach (var obj in Globals.savedObjects)
        {
            Destroy(obj);
        }
        SceneManager.LoadScene("start");
    }
}
