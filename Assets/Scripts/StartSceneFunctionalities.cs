using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneFunctionalities : MonoBehaviour
{
    public UnityEngine.UI.Button startButton;
    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(delegate () {
            SceneManager.LoadScene("mainScene");
        });
    }
}
