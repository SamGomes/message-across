using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AuxiliaryStructs
{
    public class PlayerServerState
    {
        public int orderNum;
        public int numPossibleActionsPerLevel;
        public int currNumPossibleActionsPerLevel;
        public int score;
        
        public PlayerExercise currExercise;
        public bool currExerciseFinished;
        public string currWordState;
        
        public Color buttonColor;

        public int numGives;
        public int numTakes;

        public int markerI;
        public Globals.KeyInteractionType activeInteraction;
    } 
    
    public class PlayerClient : NetworkBehaviour
    {
        public GameObject playerMarkerPrefab;
        
        private Color buttonColor;
        
        public List<GameObject> playerPlaceholders;
        
        private bool initted;
        private int activeButtonIndex;

        private GameObject ui;
        private GameObject wordPanel;

        private TextMesh[] playerDisplayTexts;
        private SpriteRenderer playerDisplayImage;
        
        private GameObject statePanel;

        private GameObject scoreUpdateUIup;
        private GameObject scoreUpdateUIdown;

        private Text possibleActionsText;
        private Text scoreText;

        private GameObject marker;
        
        private List<GameObject> maskedHalf;
        private List<GameObject> activeHalf;
        private List<GameObject> displayedHalf;
        

        private GameObject trackCanvas;

        private IEnumerator currButtonLerp;

        private bool isTopMask;
        private bool activeLayout; //parameterize the side of the board player will play in, true if left


        private Button[] playerButtons;
       
        //used to sync with game manager
        private bool changedLane;
        private bool actionStarted;
        private bool actionFinished;
        private bool actionPerforming;
        private int currPressedButtonI;

        private Button readyButton;
        private bool amIReady;

        
        
        public void Awake()
        {
            amIReady = false;
            initted = false;
            playerButtons = new Button[]{};
        }

        // public void Init(bool allowInteraction, string id, GameManager gameManagerRef, GameObject markerPrefab, GameObject canvas, GameObject ui, GameObject wordPanel, GameObject statePanel, bool isTopMask)
        [ClientRpc]
        public void Init(GameObject playerPrefabInstance, int orderNum, Color backgroundColor, Color buttonColor)
        {
            Debug.Log("init player "+orderNum);
            
            //force change to the first lane at start
            changedLane = true;
            actionStarted = false;
            actionFinished = false;
            currPressedButtonI = 0;
            
            if (initted)
            {
                return;
            }
            
            trackCanvas = GameObject.Find("Track/PlayerMarkers").gameObject;

            //init active layout
            wordPanel = activeLayout
                ? gameObject.transform.Find("WordPanels/LeftDisplay").gameObject
                : gameObject.transform.Find("WordPanels/RightDisplay").gameObject;
            
            statePanel = activeLayout
                ? gameObject.transform.Find("StateCanvas/LeftPanel").gameObject
                : gameObject.transform.Find("StateCanvas/RightPanel").gameObject;

            ui = activeLayout
                ? gameObject.transform.Find("UICanvas/LeftPlayerUI").gameObject
                : gameObject.transform.Find("UICanvas/RightPlayerUI").gameObject;

            
            readyButton = activeLayout
                ? gameObject.transform.Find("UICanvas/LeftPlayerUI/ReadyButton/Button").GetComponent<Button>()
                : gameObject.transform.Find("UICanvas/RightPlayerUI/ReadyButton/Button").GetComponent<Button>();
            readyButton.transform.parent.gameObject.SetActive(false);
            readyButton.interactable = false;
            
            
            if (activeLayout)
            {
                Destroy(gameObject.transform.Find("WordPanels/RightDisplay").gameObject);
                Destroy(gameObject.transform.Find("StateCanvas/RightPanel").gameObject);
                Destroy(gameObject.transform.Find("UICanvas/RightPlayerUI").gameObject);
            }
            else
            {
                Destroy(gameObject.transform.Find("WordPanels/LeftDisplay").gameObject);
                Destroy(gameObject.transform.Find("StateCanvas/LeftPanel").gameObject);
                Destroy(gameObject.transform.Find("UICanvas/LeftPlayerUI").gameObject);
            }
            
            wordPanel.transform.Find("Panel/Layout").GetComponent<SpriteRenderer>().color = backgroundColor;
            
            playerDisplayTexts = wordPanel.GetComponentsInChildren<TextMesh>();
            playerDisplayImage = wordPanel.GetComponentsInChildren<SpriteRenderer>()[2];

            wordPanel.SetActive(true);
            statePanel.SetActive(true);

            scoreUpdateUIup = statePanel.transform.Find("ScoreUpdateUI/Up").gameObject;
            scoreUpdateUIup.SetActive(false);
            scoreUpdateUIdown = statePanel.transform.Find("ScoreUpdateUI/Down").gameObject;
            scoreUpdateUIdown.SetActive(false);

            possibleActionsText = statePanel.transform.Find("PossibleActionsText").GetComponent<Text>();
            scoreText = statePanel.transform.Find("ScoreText").GetComponent<Text>();

            statePanel.GetComponentInChildren<Image>().color = backgroundColor;
            ui.GetComponentInChildren<Image>().color = backgroundColor;

            List<Image> imgs = new List<Image>(ui.GetComponentsInChildren<Image>());
            imgs.RemoveAt(0);
            
            //for the state display
            this.buttonColor = buttonColor;
            scoreText.color = buttonColor;
            possibleActionsText.color = buttonColor;

            //init marker upon track set. Transform prefab in instance
            marker = Instantiate(playerMarkerPrefab, trackCanvas.transform);
            MeshRenderer[] meshes = marker.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = backgroundColor;
            }
            
            //for player buttons
            foreach (Image img in imgs)
            {
                img.color = buttonColor;
            }

            //update marker sides
            maskedHalf = new List<GameObject>();
            maskedHalf.Add((isTopMask)
                ? marker.transform.Find("GameButton/BackgroundTH").gameObject
                : marker.transform.Find("GameButton/BackgroundBH").gameObject);
            maskedHalf.Add((isTopMask)
                ? marker.transform.Find("TrackTH").gameObject
                : marker.transform.Find("TrackBH").gameObject);
            
            activeHalf = new List<GameObject>();
            activeHalf.Add((!isTopMask)
                ? marker.transform.Find("GameButton/BackgroundTH").gameObject
                : marker.transform.Find("GameButton/BackgroundBH").gameObject);
            activeHalf.Add((!isTopMask)
                ? marker.transform.Find("TrackTH").gameObject
                : marker.transform.Find("TrackBH").gameObject);
            
            displayedHalf = new List<GameObject>();
            displayedHalf.Clear();
            displayedHalf.AddRange(activeHalf);
            displayedHalf.AddRange(maskedHalf);

            
            foreach (SpriteRenderer image in marker.GetComponentsInChildren<SpriteRenderer>())
            {
                image.color = backgroundColor;
            }
            activeHalf[1].SetActive(false); //lets init it to hidden

            //update ui placeholders after updating the order num
            playerPlaceholders =
                new List<GameObject>();
            
            foreach (Transform child in GameObject.Find("CanvasForUI/PlayerPlaceholders").transform){
                if (null == child)
                    continue;
                //child.gameobject contains the current child you can do whatever you want like add it to an array
                playerPlaceholders.Add(child.gameObject);
            }
            
            playerPlaceholders[(orderNum +1) % 2].SetActive(false);
            
                
            playerPrefabInstance.SetActive(true);
            //init ui
            //set buttons for touch screen
            playerButtons = new []
            {
                ui.transform.Find("Button(1)").GetComponent<Button>(),
                ui.transform.Find("Button(2)").GetComponent<Button>(),
                ui.transform.Find("Button(3)").GetComponent<Button>(),
                ui.transform.Find("Button(4)").GetComponent<Button>(),
                ui.transform.Find("Button(5)").GetComponent<Button>()
            };


            readyButton.onClick.AddListener(delegate() { 
//                Debug.Log("readyButton");
                GetReady();
                readyButton.interactable = false;
            });
            readyButton.interactable = true;
            
            //only set buttons actions for local player
            if (isLocalPlayer)
            {
                
                for (int buttonI = 0; buttonI < playerButtons.Length; buttonI++)
                {
                    Button currButton = playerButtons[buttonI];
                    currButton.interactable = true;
                    
                    currButton.GetComponent<Image>().color = buttonColor;
                    if (buttonI < 3)
                    {
                        int innerButtonI = buttonI; //for coroutine to save the iterated values
                        currButton.onClick.AddListener(delegate { ChangeLaneRequest(innerButtonI); });
                    }
                    else
                    {
                        int innerButtonI = buttonI; //for coroutine to save the iterated values
                        EventTrigger trigger = currButton.gameObject.AddComponent<EventTrigger>();
                        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
                        pointerDown.eventID = EventTriggerType.PointerDown;
                        pointerDown.callback.AddListener(delegate { ActionStartRequest(innerButtonI); });
                        trigger.triggers.Add(pointerDown);
                        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
                        pointerUp.eventID = EventTriggerType.PointerUp;
                        pointerUp.callback.AddListener(delegate { ActionFinishRequest(innerButtonI); });
                        trigger.triggers.Add(pointerUp);
                    }
                }
            }
            
            initted = true;
        }

        public bool IsReady()
        {
            return amIReady;
        }

        //start action request and ack from server
        [Command]
        public void ActionStartRequest(int buttonI)
        {
            actionStarted = true;
            actionPerforming = true;
            currPressedButtonI = buttonI;
        }
        public bool ActionStarted()
        {
            return actionStarted;
        }
        
        [Server]
        public void AckActionStarted()
        {
            actionStarted = false;
        }

        [ClientRpc] 
        public void ShowReadyButton()
        {
            readyButton.transform.parent.gameObject.SetActive(true);
        }

        [ClientRpc] 
        public void HideReadyButton()
        {
            readyButton.transform.parent.gameObject.SetActive(false);
        }            
        
        [Command]
        public void GetReady()
        {
            amIReady = true;
        }
        
        //finished action request and ack from server
        [Command]
        public void ActionFinishRequest(int buttonI)
        {
            actionFinished = true;
            currPressedButtonI = buttonI;
        }
        public bool ActionFinished()
        {
            return actionFinished;
        }
        
        [Server] 
        public void AckActionFinished()
        {
            actionFinished = false;
            actionPerforming = false;
        }
        
        [Server] 
        public bool IsPerformingAction()
        {
            return actionPerforming;
        }

        // [ClientRpc]
        // public void ResetButtonStates()
        // {
        //     changedLane = true;
        //     actionStarted = false;
        //     actionFinished = false;
        //     currPressedButtonI = 0;
        // }
        
        //change lane request and ack from server
        [Command]
        public void ChangeLaneRequest(int buttonI)
        {
            changedLane = true;
            currPressedButtonI = buttonI;
        }
        
        public bool ChangedLane()
        {
            return changedLane;
        }
        
        [Server]
        public void AckChangedLane()
        {
            changedLane = false;
        }
        
        
        [ClientRpc]
        public void SetActiveLayout(bool leftIfTrue)
        {
            if (initted)
            {
                return;
            }
            this.activeLayout = leftIfTrue;
        }
        
        [ClientRpc]
        public void SetTopMask(bool isTopMask)
        {
            if (initted)
            {
                return;
            }
            
            this.isTopMask = isTopMask;
        }

        [ClientRpc]
        public void HideScoreText()
        {
            scoreText.gameObject.SetActive(false);
        }
        
        [ClientRpc]
        public void ShowScoreText()
        {
            scoreText.gameObject.SetActive(true);
        }
        

        [ClientRpc]
        public void InitCurrWordState(string currWordState, PlayerExercise currExercise)
        {
            currWordState = "";
            int missingLength = currExercise.targetWord.Length;
            for (int i = 0; i < missingLength; i++)
            {
                currWordState += ' ';
            }

            
            
            //Update UI
            playerDisplayTexts[0].text = currExercise.displayMessage;
            playerDisplayTexts[1].text = currWordState;
            
            try
            {
                playerDisplayImage.sprite =
                    Resources.Load<Sprite>("Textures/PlayerUI/ExerciseImages/" + currExercise.targetWord);
            }
            catch (FileNotFoundException e)
            {
                playerDisplayImage.sprite =
                    Resources.Load<Sprite>("Textures/PlayerUI/ExerciseImages/placeholder");
            }

            //animate transition
            wordPanel.GetComponentInChildren<Animator>().Play(0);
        }

        [ClientRpc]
        public void UpdateCurrWordState(string currWordState)
        {
            //Update UI
            playerDisplayTexts[1].text = currWordState;
        }

        
//        [also as ClientRpc, called from SetScore]
        public IEnumerator DelayedScoreDisplay(float score, float delay)
        {
            yield return new WaitForSeconds(delay);
            scoreText.text = "Score: " + score;
        }

        [ClientRpc]
        public void SetScore(int score, int increase, float delay)
        {
            //update UI
            scoreText.text = "Score: " + score;
            if (increase == 0)
            {
                return;
            }
            statePanel.GetComponent<Animator>().Play(0);
            if (increase > 0)
            {
                scoreUpdateUIup.GetComponentInChildren<Text>().text = "+" + increase;
                scoreUpdateUIup.SetActive(false);
                scoreUpdateUIup.SetActive(true);
            }
            else if (increase < 0)
            {
                scoreUpdateUIdown.GetComponentInChildren<Text>().text = "-" + Math.Abs(increase);
                scoreUpdateUIdown.SetActive(false);
                scoreUpdateUIdown.SetActive(true);
            }
        }
        

        [ClientRpc]
        public void SetActiveTrackButton(int activeButtonIndex, Vector3 activeButtonPos)
        {
            
            //updateTrack
            if (currButtonLerp != null)
            {
                StopCoroutine(currButtonLerp);
            }

            currButtonLerp = Globals.LerpAnimation(marker, activeButtonPos, 10.0f);
            StartCoroutine(currButtonLerp);

            this.activeButtonIndex = activeButtonIndex;
        }


        [ClientRpc]
        public void HideHalfMarker()
        {
            displayedHalf.Clear();
            displayedHalf.AddRange(activeHalf);
        }
        
        [ClientRpc]
        public void ShowHalfMarker()
        {
            displayedHalf.Clear();
            displayedHalf.AddRange(activeHalf);
            displayedHalf.AddRange(maskedHalf);
        }

        [ClientRpc]
        public void UpdateTrackHalf(bool visible)
        {
            foreach (GameObject obj in maskedHalf)
            {
                obj.SetActive(false);
            }
            
            for (int i = 0; i < displayedHalf.Count; i++)
            {
                GameObject currObj = displayedHalf[i];
                if (i % 2 == 0)
                {
                    currObj.SetActive(true);
                }
                else
                {
                    currObj.SetActive(visible);
                }

            }
        }

        [ClientRpc]
        public void ChangeButtonColor(int buttonI, Color color)
        {
            playerButtons[buttonI].GetComponent<Image>().color = color;
        }

        
        [ClientRpc]
        public void ChangeAllButtonsColor(Color color)
        {
            for (int i=0; i<playerButtons.Length; i++)
            {
                Button button = playerButtons[i];
                button.GetComponent<Image>().color = color;
            }
        }
        
        [ClientRpc]
        public void DisableAllButtons(Color disabledColor)
        {
            for (int i=0; i<playerButtons.Length; i++)
            {
                Button button = playerButtons[i];
                button.interactable = false;
                button.GetComponent<Image>().color = disabledColor;
            }
        }
        [ClientRpc]
        public void EnableAllButtons()
        {
            for (int i=0; i<playerButtons.Length; i++)
            {
                Button button = playerButtons[i];
                button.interactable = true;
                button.GetComponent<Image>().color = buttonColor;
            }
        }
        
        public int GetActiveButtonIndex()
        {
            return activeButtonIndex;
        }

        public int GetPressedButtonIndex()
        {
            return currPressedButtonI;
        }


        [ClientRpc]
        public void UpdateNumPossibleActions(int currNumPossibleActionsPerLevel)
        {
            possibleActionsText.text = "Actions: " + currNumPossibleActionsPerLevel;
            statePanel.GetComponent<Animator>().Play(0);
        }

//        [ClientRpc]
//        public void ResetNumPossibleActions()
//        {
//            currNumPossibleActionsPerLevel = info.numPossibleActionsPerLevel;
//            //update UI
//            possibleActionsText.text = "Actions: " + currNumPossibleActionsPerLevel;
//            statePanel.GetComponent<Animator>().Play(0);
//        }
//
//        [ClientRpc]
//        public void DecreasePossibleActionsPerLevel()
//        {
//            currNumPossibleActionsPerLevel--;
//            //update UI
//            possibleActionsText.text = "Actions: " + currNumPossibleActionsPerLevel;
//            statePanel.GetComponent<Animator>().Play(0);
//        }

        [ClientRpc]
        public void PressMarker()
        {
            marker.transform.localScale = new Vector3(0.24f, 0.24f, 0.24f);
        }

        [ClientRpc]
        public void ReleaseMarker()
        {
            marker.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        }

    }
}