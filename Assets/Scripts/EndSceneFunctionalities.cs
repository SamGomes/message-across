using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneFunctionalities : MonoBehaviour
{
    public Button quitButton;
    public Button restartButton;
    
    // Start is called before the first frame update
    void Start()
    {
        quitButton.onClick.AddListener(delegate(){
            Application.Quit();
        });
        restartButton.onClick.AddListener(delegate () {
            foreach (var obj in Globals.savedObjects)
            {
                Destroy(obj);
            }
            Globals.InitGlobals();
            SceneManager.LoadScene("paramsSetup");
        });
        
        Globals.audioManagers[0].StopCurrentClip();
        Globals.audioManagers[0].PlayInfiniteClip(
            Globals.backgroundMusicPath,
            Globals.backgroundMusicPath);
        
    }
}
