using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class Popup
{
    
    private Func<int> OnHide;
    
    private Button UIcloseButton;
    private GameObject popupInstance;
    private string audioPath;

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
    public Popup(bool isGlobal, Camera worldCamera, Transform popupPositioner)
    {
        popupInstance = Object.Instantiate(Resources.Load<GameObject>( "Prefabs/Popup"), popupPositioner).gameObject;
        popupInstance.GetComponentInChildren<Canvas>().worldCamera = worldCamera;
        if (isGlobal)
        {
            Object.DontDestroyOnLoad(popupInstance); 
        }
        if (isGlobal)
        {
            Object.DontDestroyOnLoad(popupInstance); 
        }

        OnHide = delegate { return 0; };
        
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
    

    // public void AddOnShow(Func<int> OnShow)
    // {
    //     this.OnShow = OnShow;
    // }
    public void SetOnHide(Func<int> OnHide)
    {
        this.OnHide = OnHide;
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
        OnHide();
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
