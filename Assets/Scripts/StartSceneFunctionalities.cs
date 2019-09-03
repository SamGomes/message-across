using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneFunctionalities : MonoBehaviour
{
    public Button startButton;
    public Button[] paramsButtons;
    // Start is called before the first frame update
    void Start()
    {
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
