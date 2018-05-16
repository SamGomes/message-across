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
    private GameObject questionnairesMenu;

    public void startAndPauseGame(Utilities.PlayerId caller)
    {
        if (!isGameLoaded)
        {
            this.startGame(caller);
        }
        else
        {
            this.pauseGame(caller);
        }
        
    }

    public void mainSceneLoadedNotification()
    {
        isGameLoaded = true;
        this.plainPauseMenu = GameObject.Find("Canvas/PauseCanvas");
        this.questionnairesMenu = GameObject.Find("Canvas/QuestionnairePauseCanvas");
        plainPauseMenu.SetActive(false);
        questionnairesMenu.SetActive(false);
        this.currPauseMenu = plainPauseMenu;
    }

    private void startGame(Utilities.PlayerId caller)
    {
        //set stuff
        isGamePaused = false;
        isGameLoaded = false;

        gameObject.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene("crossAnt");
    }
    public void endGame()
    {
        //reset stuff
        isGamePaused = false;
        isGameLoaded = false;

        SceneManager.LoadScene("gameover");
    }

    private void pauseGame(Utilities.PlayerId caller)
    {
        if (!isGamePaused)
        {
            gameManager.pauseGame();
            currPauseMenu.SetActive(true);
        }
        else
        {
            gameManager.resumeGame();
            currPauseMenu.SetActive(false);

            //regain pause menu everytime user unpauses the questionnaire menu
            if (currPauseMenu != plainPauseMenu)
            {
                currPauseMenu = plainPauseMenu;
            }
        }
        isGamePaused = !isGamePaused;
    }

    public void quitGame(Utilities.PlayerId caller)
    {
        Application.Quit();
    }

    public void pauseForQuestionnaires(Utilities.PlayerId caller)
    {
        this.currPauseMenu = questionnairesMenu;
        this.pauseGame(caller);
    }
}
