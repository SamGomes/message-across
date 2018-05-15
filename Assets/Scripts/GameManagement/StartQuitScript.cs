using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartQuitScript : MonoBehaviour {

    private bool isGamePaused = false;
    public GameManager gameManager;

    private GameObject pauseMenu;


    
    public void startGame(Utilities.PlayerId caller)
    {
        gameObject.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene("crossAnt");
    }
    public void pauseGame(Utilities.PlayerId caller)
    {
        if (isGamePaused)
        {
            gameManager.pauseGame();
            pauseMenu.SetActive(true);
        }
        else
        {
            gameManager.resumeGame();
            pauseMenu.SetActive(false);
        }
        isGamePaused = !isGamePaused;
    }
    public void quitGame(Utilities.PlayerId caller)
    {
        Application.Quit();
    }
}
