using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class Popup
{
    private Button UIcloseButton;
    private GameObject popupInstance;
    private string audioPath;

    private Func<int> OnShow;
    private Func<int> OnHide;

    private GameObject buttonsContainer;
    private GameObject buttonPrefab;

    private Text popupMessage;
    private int StopAllAnimations()
    {
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        List<Animator> mainSceneAnimators = new List<Animator>();
        for (int i=0; i< rootGameObjects.Length; i++)
        {
            GameObject root = rootGameObjects[i];
            mainSceneAnimators.AddRange(root.GetComponents<Animator>());
            mainSceneAnimators.AddRange(root.GetComponentsInChildren<Animator>());
        }

        for (int i = 0; i < mainSceneAnimators.Count; i++)
        {
            mainSceneAnimators[i].enabled = false;
        }
        return 0;
    }
    private int PlayAllAnimations()
    {
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        List<Animator> mainSceneAnimators = new List<Animator>();
        for (int i = 0; i < rootGameObjects.Length; i++)
        {
            GameObject root = rootGameObjects[i];
            mainSceneAnimators.AddRange(root.GetComponents<Animator>());
            mainSceneAnimators.AddRange(root.GetComponentsInChildren<Animator>());
        }

        for (int i = 0; i < mainSceneAnimators.Count; i++)
        {
            mainSceneAnimators[i].enabled = true;
        }
        return 0;
    }

    // Use this for initialization
    public Popup(bool isGlobal)
    {
        popupInstance = Object.Instantiate(Resources.Load<GameObject>( "Prefabs/Popup")).gameObject;
        if (isGlobal)
        {
            Object.DontDestroyOnLoad(popupInstance); 
        }

//        Image background = popupInstance.transform.Find("Background").GetComponent<Image>();
//        backround.color = backgroundColor;
        Transform canvas = popupInstance.transform.Find("Canvas");
        popupMessage = canvas.Find("Message").GetComponent<Text>();
        UIcloseButton = canvas.Find("CloseButton").GetComponent<Button>();
        buttonsContainer = canvas.Find("ButtonsContainer").gameObject;
        buttonPrefab = buttonsContainer.transform.Find("ButtonPrefab").gameObject;
        buttonPrefab.SetActive(false);
        HidePopupPanel();
        UIcloseButton.onClick.AddListener(delegate ()
        {
            HidePopupPanel();
        });

        audioPath = null;
    }
    

    public void AddOnShow(Func<int> OnShow)
    {
        this.OnShow = OnShow;
    }
    public void AddOnHide(Func<int> OnHide)
    {
        this.OnHide = OnHide;
        UIcloseButton.onClick.AddListener(delegate ()
        {
            OnHide();
        });
    }
    
    public void AddButton(string title, Func<int> OnClick)
    {
        buttonPrefab.SetActive(true);
        GameObject button = Object.Instantiate(buttonPrefab, buttonsContainer.transform);
        button.GetComponentInChildren<Text>().text = title;
        button.GetComponent<Button>().onClick.AddListener(delegate ()
        {
            OnClick();
        });
        buttonPrefab.SetActive(false);
    }


    public void DestroyPopupPanel()
    {
        UnityEngine.Object.Destroy(popupInstance);
    }
    public void HidePopupPanel()
    {
        popupInstance.gameObject.SetActive(false);
        PlayAllAnimations();
    }

    public void HasCloseButton(bool hasCloseButton)
    {
        UIcloseButton.gameObject.SetActive(hasCloseButton);
    }

    public void SetMessage(string text)
    {
        popupMessage.text = text;
    }

    public void DisplayPopup()
    {
        popupInstance.SetActive(true);
        if (audioPath != null)
        {
            Globals.audioManagers[1].PlayClip(audioPath);
        }
        StopAllAnimations();
    }
    
}
