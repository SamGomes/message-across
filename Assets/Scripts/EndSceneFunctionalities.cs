using AuxiliaryStructs;
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
        //remove game manager object
        Destroy(Globals.savedGameObjects[3]);

        //hide player UIs, leave scores
        if (Globals.savedGameObjects[4] != null)
        {
            Globals.savedGameObjects[4].GetComponent<PlayerClient>().HideUI();
        }
        if (Globals.savedGameObjects[5] != null)
        {
            Globals.savedGameObjects[5].GetComponent<PlayerClient>().HideUI();
        }
        
        quitButton.onClick.AddListener(delegate(){
            Application.Quit();
        });
        restartButton.onClick.AddListener(delegate () {
            //destroy client UIs
            Destroy(Globals.savedGameObjects[4]);
            Destroy(Globals.savedGameObjects[5]);
            SceneManager.LoadScene("start");
        });
        
        // Globals.audioManagers[0].StopCurrentClip();
        // Globals.audioManagers[0].PlayInfiniteClip(
        //     Globals.backgroundMusicPath,
        //     Globals.backgroundMusicPath);
        
    }
}
