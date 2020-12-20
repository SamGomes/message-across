using System;
using System.Collections;
using System.Collections.Generic;
using AuxiliaryStructs;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AuxiliaryStructs
{
    public class Player : NetworkBehaviour
    {
        public List<GameObject> playerPlaceholders;
        public Transform markerPlaceholders;
        
        private bool initted;
//        private int orderNum;
        
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
        
        private List<GameObject> maskedHalf;
        private List<GameObject> activeHalf;
        private List<GameObject> displayedHalf;
        
        private GameButton gameButton;

        private GameObject trackCanvas;

        private Color buttonColor;

        private IEnumerator currButtonLerp;

        private PlayerExercise currExercise;
        public bool currExerciseFinished;
        private string currWordState;
        

        public bool pressingButton;
        private bool allowInteraction;

        private bool isTopMask;
        private bool activeLayout; //parameterize the side of the board player will play in, true if left


        private Button[] playerButtons;
       
        //used to sync with game manager
        private bool changedLane;
        private int currPressedButtonI;

        
        public void Awake()
        {
            initted = false;
            playerButtons = new Button[]{};
        }

        // public void Init(bool allowInteraction, string id, GameManager gameManagerRef, GameObject markerPrefab, GameObject canvas, GameObject ui, GameObject wordPanel, GameObject statePanel, bool isTopMask)
        [ClientRpc]
        public void Init(PlayerInfo info, int orderNum)
        {
            Debug.Log("init player "+orderNum);
            
            //force change to the first lane at start
            changedLane = true;
            currPressedButtonI = 0;
            ChangedLane();
            
            if (initted)
            {
                return;
            }
            
            
            this.info = info;
            this.trackCanvas = GameObject.Find("Track/playerMarkers").gameObject;

            
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
            
            wordPanel.transform.Find("panel/Layout").GetComponent<SpriteRenderer>().color = backgroundColor;
            

            wordPanel.SetActive(true);
            statePanel.SetActive(true);

            scoreUpdateUIup = statePanel.transform.Find("scoreUpdateUI/up").gameObject;
            scoreUpdateUIup.SetActive(false);
            scoreUpdateUIdown = statePanel.transform.Find("scoreUpdateUI/down").gameObject;
            scoreUpdateUIdown.SetActive(false);

            possibleActionsText = statePanel.transform.Find("possibleActionsText").GetComponent<Text>();
            scoreText = statePanel.transform.Find("scoreText").GetComponent<Text>();

            buttonColor = SetColor(backgroundColor);
            
            
            //init marker upon track set. Transform prefab in instance
            marker = Instantiate(markerPrefab, trackCanvas.transform);
            MeshRenderer[] meshes = marker.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.material.color = backgroundColor;
            }
            
            //update marker sides
            maskedHalf = new List<GameObject>();
            maskedHalf.Add((isTopMask)
                ? marker.transform.Find("Button/BackgroundTH").gameObject
                : marker.transform.Find("Button/BackgroundBH").gameObject);
            maskedHalf.Add((isTopMask)
                ? marker.transform.Find("trackTH").gameObject
                : marker.transform.Find("trackBH").gameObject);
            
            activeHalf = new List<GameObject>();
            activeHalf.Add((!isTopMask)
                ? marker.transform.Find("Button/BackgroundTH").gameObject
                : marker.transform.Find("Button/BackgroundBH").gameObject);
            activeHalf.Add((!isTopMask)
                ? marker.transform.Find("trackTH").gameObject
                : marker.transform.Find("trackBH").gameObject);
            
            displayedHalf = new List<GameObject>();
            displayedHalf.Clear();
            displayedHalf.AddRange(activeHalf);
            displayedHalf.AddRange(maskedHalf);

            activeHalf[1].SetActive(false); //lets init it to hidden
            
            
            foreach (Button button in ui.GetComponentsInChildren<Button>())
            {
                button.interactable = allowInteraction;
            }
            foreach (SpriteRenderer image in marker.GetComponentsInChildren<SpriteRenderer>())
            {
                image.color = this.backgroundColor;
            }
            
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
            
            Debug.Log("orderI: "+((orderNum +1) % 2));
            playerPlaceholders[(orderNum +1) % 2].SetActive(false);
            
            this.markerPlaceholders = GameObject.Find("Track/MarkerPlaceholders").transform;

            //init ui buttons
            GameObject playerUI = ui;
            //set buttons for touch screen
            playerButtons = playerUI.GetComponentsInChildren<Button>();

            //only set buttons for local player, the others get "computer said no"
            if (isLocalPlayer)
            {
                for (int buttonI = 0; buttonI < playerButtons.Length; buttonI++)
                {
                    Button currButton = playerButtons[buttonI];
                    currButton.interactable = true;
                    
                    currButton.GetComponent<Image>().color = new Color(0.0f, 0.0f, 1.0f);
                    
                    if (buttonI < markerPlaceholders.childCount)
                    {
                        currButton.GetComponent<Image>().color = GetButtonColor();
                        int innerButtonI = buttonI; //for coroutine to save the iterated values
                        currButton.onClick.AddListener(delegate() { ChangeLane(innerButtonI); });

                    }
//                    else
//                    {
//                        int j = buttonI - markerPlaceholders.childCount + 1;
//                        Globals.KeyInteractionType iType = (Globals.KeyInteractionType)j;
//                        EventTrigger trigger = currButton.gameObject.AddComponent<EventTrigger>();
//                        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
//                        pointerDown.eventID = EventTriggerType.PointerDown;
//                        pointerDown.callback.AddListener(delegate (BaseEventData eventData)
//                        {
//                            currButton.GetComponent<Image>().color = new Color(1.0f, 0.82f, 0.0f);
//                
//                            //verify if button should be pressed
//                            bool playerOverlappedAndPressing = false;
//                            foreach (Player innerPlayer in players)
//                            {
//                                if (innerPlayer != player && player.IsPressingButton())
//                                {
//                                    playerOverlappedAndPressing = true;
//                                    break;
//                                }
//                            }
//                
//                            if (player.GetCurrNumPossibleActionsPerLevel() < 1 ||
//                                (isButtonOverlap && playerOverlappedAndPressing))
//                            {
//                                Globals.trackEffectsAudioManager.PlayClip("Audio/badMove");
//                                currButton.GetComponent<Image>().color = Color.red;
//                                return;
//                            }
//                
//                            Globals.trackEffectsAudioManager.PlayClip("Audio/clickDown");
//                            player.SetActiveInteraction(iType);
//                
//                            foreach (Player innerPlayer in players)
//                            {
//                                if (innerPlayer != player && player.IsPressingButton() &&
//                                    player.GetActivebuttonIndex() == player.GetActivebuttonIndex())
//                                {
//                                    return;
//                                }
//                            }
//                            player.PressGameButton();
//                        });
//                        trigger.triggers.Add(pointerDown);
//                        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
//                        pointerUp.eventID = EventTriggerType.PointerUp;
//                        pointerUp.callback.AddListener(delegate (BaseEventData eventData)
//                        {
//                            Globals.trackEffectsAudioManager.PlayClip("Audio/clickUp");
//                            //verify if button should be pressed
//                            if (player.GetCurrNumPossibleActionsPerLevel() > 0)
//                            {
//                                currButton.GetComponent<Image>().color = player.GetButtonColor();
//                            }
//                            player.SetActiveInteraction(Globals.KeyInteractionType.NONE);
//                            player.ReleaseGameButton();
//                        });
//                        trigger.triggers.Add(pointerUp);
//                    }
                }
            }
            
            
            
            initted = true;
        }
        
        //executed in server
        [Command]
        public void ChangeLane(int buttonI)
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
        
        private Color SetColor(Color newColor)
        {
            statePanel.GetComponentInChildren<Image>().color = newColor;
            ui.GetComponentInChildren<Image>().color = newColor;

            float g = 1.0f - newColor.grayscale * 0.8f;
            List<Image> imgs = new List<Image>(ui.GetComponentsInChildren<Image>());
            imgs.RemoveAt(0);

            Color dualColor = new Color(g, g, g);

            //for the state display
            scoreText.color = dualColor;
            possibleActionsText.color = dualColor;

            //for player buttons
            foreach (Image img in imgs)
            {
                img.color = dualColor;
            }
            
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
        }

        [ClientRpc]
        public void SetCurrWordState(string newCurrWordState)
        {
            currWordState = newCurrWordState;

            //Update UI
            TextMesh[] playerDisplayTexts = wordPanel.GetComponentsInChildren<TextMesh>();
            playerDisplayTexts[0].text = currExercise.targetWord;
            playerDisplayTexts[1].text = currWordState;
        }

        public string GetCurrWordState()
        {
            return currWordState;
        }

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
                StartCoroutine(DelayedScoreDisplay(score, delay));
                statePanel.GetComponent<Animator>().Play(0);
                Color newColor = backgroundColor;
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
        public void SetActiveButton(int activeButtonIndex, Vector3 activeButtonPos)
        {
            //updateUI
            foreach (Button button in playerButtons)
            {
//                button.GetComponent<Image>().color = new Color(1.0f, 0.82f, 0.0f);
            }
            playerButtons[activeButtonIndex].GetComponent<Image>().color =
                GetButtonColor();
            
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

        public void SetActiveInteraction(Globals.KeyInteractionType activeInteraction)
        {
            this.activeInteraction = activeInteraction;
        }

        public Globals.KeyInteractionType GetActiveInteraction()
        {
            return activeInteraction;
        }

        public int GetActiveButtonIndex()
        {
            return this.activeButtonIndex;
        }

        public int GetPressedButtonIndex()
        {
            return this.currPressedButtonI;
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
        public void SetNumPossibleActions(int currNumPossibleActionsPerLevel)
        {
            this.currNumPossibleActionsPerLevel = currNumPossibleActionsPerLevel;
            //update UI
            possibleActionsText.text = "Actions: " + currNumPossibleActionsPerLevel;
            statePanel.GetComponent<Animator>().Play(0);
        }

        [ClientRpc]
        public void ResetNumPossibleActions()
        {
            this.currNumPossibleActionsPerLevel = info.numPossibleActionsPerLevel;
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

        public void PressGameButton()
        {
            this.pressingButton = true;
            this.gameButton.RegisterButtonDown();
            UpdateActiveHalf(true);
        }

        public void ReleaseGameButton()
        {
            this.pressingButton = false;
            this.gameButton.RegisterButtonUp();
            UpdateActiveHalf(false);

        }

        public GameObject GetUI()
        {
            return this.ui;
        }

        public bool IsPressingButton()
        {
            return pressingButton;
        }

        public GameObject GetStatePanel()
        {
            return this.statePanel;
        }
    }
}