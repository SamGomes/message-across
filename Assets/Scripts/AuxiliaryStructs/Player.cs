using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AuxiliaryStructs
{
    public class Player : NetworkBehaviour
    {
        public List<GameObject> playerPlaceholders;
        public Transform markerPlaceholders;
        
        private bool initted;
        
        public PlayerInfo info;

        public Color backgroundColor;

        private int currNumPossibleActionsPerLevel;

        private int score;
        private int activeButtonIndex;

        private Globals.KeyInteractionType activeInteraction;

        public GameObject ui;
        public GameObject wordPanel;
        public GameObject statePanel;

        public GameObject scoreUpdateUIup;
        public GameObject scoreUpdateUIdown;

        public Text possibleActionsText;
        public Text scoreText;

        private GameObject marker;
        public GameObject markerPrefab;
        private GameButton gameButton;
        
        private List<GameObject> maskedHalf;
        private List<GameObject> activeHalf;
        private List<GameObject> displayedHalf;
        

        private GameObject trackCanvas;

        private Color buttonColor;

        private IEnumerator currButtonLerp;

        private PlayerExercise currExercise;
        public bool currExerciseFinished;
        private string currWordState;
        
        public bool pressingButton;
        
        private bool isTopMask;
        private bool activeLayout; //parameterize the side of the board player will play in, true if left


        private Button[] playerButtons;
        private bool[] isButtonEnabled;
       
        //used to sync with game manager
        private bool changedLane;
        private bool actionStarted;
        private bool actionFinished;
        private int currPressedButtonI;

        
        
        
        public void Awake()
        {
            initted = false;
            playerButtons = new Button[]{};
        }

        // public void Init(bool allowInteraction, string id, GameManager gameManagerRef, GameObject markerPrefab, GameObject canvas, GameObject ui, GameObject wordPanel, GameObject statePanel, bool isTopMask)
        [ClientRpc]
        public void Init(PlayerInfo info, GameObject playerPrefabInstance, int orderNum)
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
            
            
            this.info = info;
            this.trackCanvas = GameObject.Find("Track/PlayerMarkers").gameObject;

            
            //init colors
            backgroundColor = new Color(info.buttonRGB[0], info.buttonRGB[1], info.buttonRGB[2], 0.8f);
            
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
            
            buttonColor = CalcButtonColor(backgroundColor);
            
            //for the state display
            scoreText.color = buttonColor;
            possibleActionsText.color = buttonColor;

            //for player buttons
            foreach (Image img in imgs)
            {
                img.color = buttonColor;
            }
            
            
            //init marker upon track set. Transform prefab in instance
            marker = Instantiate(markerPrefab, trackCanvas.transform);
            MeshRenderer[] meshes = marker.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = backgroundColor;
            }

            gameButton = marker.transform.Find("GameButton").GetComponent<GameButton>();
            
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
                image.color = this.backgroundColor;
            }
            activeHalf[1].SetActive(false); //lets init it to hidden
            
            score = -1;
            currNumPossibleActionsPerLevel = 0;
            pressingButton = false;
            
            
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
            
            markerPlaceholders = GameObject.Find("Track/MarkerPlaceholders").transform;
                
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
            isButtonEnabled= new []
            {
                true,true,true,true,true
            };

            //only set buttons actions for local player
            if (isLocalPlayer)
            {
                for (int buttonI = 0; buttonI < playerButtons.Length; buttonI++)
                {
                    Button currButton = playerButtons[buttonI];
                    currButton.interactable = true;
                    
                    currButton.GetComponent<Image>().color = GetButtonColor();
                    if (buttonI < markerPlaceholders.childCount)
                    {
                        int innerButtonI = buttonI; //for coroutine to save the iterated values
                        currButton.onClick.AddListener(delegate() { ChangeLaneRequest(innerButtonI); });

                    }
                    else
                    {
                        int innerButtonI = buttonI; //for coroutine to save the iterated values
                        EventTrigger trigger = currButton.gameObject.AddComponent<EventTrigger>();
                        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
                        pointerDown.eventID = EventTriggerType.PointerDown;
                        pointerDown.callback.AddListener(delegate(BaseEventData eventData) { ActionStartRequest(innerButtonI); });
                        trigger.triggers.Add(pointerDown);
                        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
                        pointerUp.eventID = EventTriggerType.PointerUp;
                        pointerUp.callback.AddListener(delegate (BaseEventData eventData) { ActionFinishRequest(innerButtonI); });
                        trigger.triggers.Add(pointerUp);
                    }
                }
            }
            
            initted = true;
        }
        
        
        //start action request and ack from server
        [Command]
        public void ActionStartRequest(int buttonI)
        {
            actionStarted = true;
            currPressedButtonI = buttonI;
        }
        public bool ActionStarted()
        {
            return actionStarted;
        }
        
        [ClientRpc] 
        public void AckActionStarted()
        {
            actionStarted = false;
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
        
        [ClientRpc] 
        public void AckActionFinished()
        {
            actionFinished = false;
        }

        [ClientRpc]
        public void ResetButtonStates()
        {
            changedLane = true;
            actionStarted = false;
            actionFinished = false;
            currPressedButtonI = 0;
        }
        
        //change lane request and ack from server
        [Command]
        public void ChangeLaneRequest(int buttonI)
        {
            changedLane = true;
            currPressedButtonI = buttonI;
        }
        
        public bool ChangedLane()
        {
            return this.changedLane;
        }
        
        [ClientRpc] 
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

        
        private Color CalcButtonColor(Color newColor)
        {
            float g = 1.0f - newColor.grayscale * 0.8f;
            Color dualColor = new Color(g, g, g);
            return dualColor;
        }


        public string GetId()
        {
            return info.id;
        }

        [ClientRpc]
        public void SetCurrExercise(PlayerExercise newExercise)
        {
            currExercise = newExercise;
        }

        public PlayerExercise GetCurrExercise()
        {
            return currExercise;
        }

        [ClientRpc]
        public void InitCurrWordState()
        {
            currWordState = "";
            int missingLength = currExercise.targetWord.Length;
            for (int i = 0; i < missingLength; i++)
            {
                currWordState += ' ';
            }

            //Update UI
            TextMesh[] playerDisplayTexts = wordPanel.GetComponentsInChildren<TextMesh>();
            playerDisplayTexts[0].text = currExercise.displayMessage;
            playerDisplayTexts[1].text = currWordState;
            
            //animate transition
            wordPanel.GetComponentInChildren<Animator>().Play(0);
        }

        [ClientRpc]
        public void SetCurrWordState(string newCurrWordState)
        {
            currWordState = newCurrWordState;

            //Update UI
            TextMesh[] playerDisplayTexts = wordPanel.GetComponentsInChildren<TextMesh>();
            playerDisplayTexts[0].text = currExercise.displayMessage;
            playerDisplayTexts[1].text = currWordState;
        }

        public string GetCurrWordState()
        {
            return currWordState;
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
            if (this.score != score)
            {
                this.score = score;

                //update UI
//                StartCoroutine(DelayedScoreDisplay(score, delay));
                scoreText.text = "Score: " + score;
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
        }

        public int GetScore()
        {
            return this.score;
        }

        public GameObject GetWordPanel()
        {
            return this.wordPanel;
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


        public List<GameObject> GetMaskedHalf()
        {
            return this.maskedHalf;
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
        public void UpdateActiveHalf(bool visible)
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
        public void SetActiveInteraction(Globals.KeyInteractionType activeInteraction)
        {
            this.activeInteraction = activeInteraction;
        }

        public Globals.KeyInteractionType GetActiveInteraction()
        {
            return activeInteraction;
        }

        [ClientRpc]
        public void ChangeButtonColor(int buttonI, Color color)
        {
            playerButtons[buttonI].GetComponent<Image>().color = color;
        }

        public bool IsButtonEnabled(int buttonI)
        {
            return isButtonEnabled[currPressedButtonI];
        }
        
        [ClientRpc]
        public void ChangeAllButtonsColor(Color color)
        {
            foreach (Button button in ui.GetComponentsInChildren<Button>())
            {
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
                isButtonEnabled[i] = false;
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
                isButtonEnabled[i] = true;
                button.GetComponent<Image>().color = GetButtonColor();
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

        public Color GetButtonColor()
        {
            return buttonColor;
        }

        public GameObject GetMarker()
        {
            return this.marker;
        }


        [ClientRpc]
        public void SetTempNumPossibleActions(int currNumPossibleActionsPerLevel)
        {
            this.currNumPossibleActionsPerLevel = currNumPossibleActionsPerLevel;
            //update UI
            possibleActionsText.text = "Actions: " + currNumPossibleActionsPerLevel;
            statePanel.GetComponent<Animator>().Play(0);
        }

        [ClientRpc]
        public void ResetNumPossibleActions()
        {
            currNumPossibleActionsPerLevel = info.numPossibleActionsPerLevel;
            //update UI
            possibleActionsText.text = "Actions: " + currNumPossibleActionsPerLevel;
            statePanel.GetComponent<Animator>().Play(0);
        }

        [ClientRpc]
        public void DecreasePossibleActionsPerLevel()
        {
            currNumPossibleActionsPerLevel--;
            //update UI
            possibleActionsText.text = "Actions: " + currNumPossibleActionsPerLevel;
            statePanel.GetComponent<Animator>().Play(0);
        }

        public int GetCurrNumPossibleActionsPerLevel()
        {
            return this.currNumPossibleActionsPerLevel;
        }

        [ClientRpc]
        public void PressGameButton()
        {
            this.pressingButton = true;
            this.gameButton.RegisterButtonDown();
        }

        [ClientRpc]
        public void ReleaseGameButton()
        {
            this.pressingButton = false;
            this.gameButton.RegisterButtonUp();

        }

        public GameObject GetUI()
        {
            return this.ui;
        }
        
        
        public bool IsPressingButton()
        {
            return pressingButton;
        }

        public GameButton GetGameButton()
        {
            return this.gameButton;
        }
    }
}