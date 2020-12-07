using System;
using System.Collections;
using System.Collections.Generic;
using AuxiliaryStructs;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace AuxiliaryStructs
{
    public class Player : NetworkBehaviour
    {
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


        // public void Init(bool allowInteraction, string id, GameManager gameManagerRef, GameObject markerPrefab, GameObject canvas, GameObject ui, GameObject wordPanel, GameObject statePanel, bool isTopMask)
        public void Init()
        {
            //init colors
            this.backgroundColor = new Color(info.buttonRGB[0], info.buttonRGB[1], info.buttonRGB[2], 0.8f);
            
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
            marker = UnityEngine.Object.Instantiate(markerPrefab, trackCanvas.transform);
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
                : marker.transform.Find("trackBH").gameObject); //activeHalf[1] is the one of the track
            activeHalf.Add((isTopMask)
                ? marker.transform.Find("Button/BackgroundTH").gameObject
                : marker.transform.Find("Button/BackgroundBH").gameObject);
            activeHalf.Add((isTopMask)
                ? marker.transform.Find("trackTH").gameObject
                : marker.transform.Find("trackBH").gameObject);

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
            SetNumPossibleActions(0);
            pressingButton = false;
        }
        
        public void SetTrackCanvas(GameObject trackCanvas)
        {
            this.trackCanvas = trackCanvas;
        }
        
        public void SetActiveLayout(bool leftIfTrue)
        {
            this.activeLayout = leftIfTrue;
        }
        
        public void SetTopMask(bool isTopMask)
        {
            this.isTopMask = isTopMask;
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

        public void SetCurrExercise(PlayerExercise newExercise)
        {
            currExercise = newExercise;
        }

        public PlayerExercise GetCurrExercise()
        {
            return currExercise;
        }

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

        public void SetActiveButton(int activeButtonIndex, Vector3 activeButtonPos)
        {
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

        public List<GameObject> GetActiveHalf()
        {
            return this.activeHalf;
        }

        public void SetActiveHalf(List<GameObject> activeHalf)
        {
            this.activeHalf = activeHalf;
        }

        public void UpdateActiveHalf(bool visible)
        {
            for (int i = 0; i < activeHalf.Count; i++)
            {
                if (i % 2 == 0)
                {
                    continue;
                }

                GameObject currObj = activeHalf[i];
                currObj.SetActive(visible);
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

        public int GetActivebuttonIndex()
        {
            return this.activeButtonIndex;
        }


        public Color GetButtonColor()
        {
            return buttonColor;
        }

        public GameObject GetMarker()
        {
            return this.marker;
        }

        public void SetNumPossibleActions(int currNumPossibleActionsPerLevel)
        {
            this.currNumPossibleActionsPerLevel = currNumPossibleActionsPerLevel;
            //update UI
            possibleActionsText.text = "Actions: " + currNumPossibleActionsPerLevel;
            statePanel.GetComponent<Animator>().Play(0);
        }

        public void ResetNumPossibleActions()
        {
            SetNumPossibleActions(info.numPossibleActionsPerLevel);
        }

        public int GetCurrNumPossibleActionsPerLevel()
        {
            return this.currNumPossibleActionsPerLevel;
        }

        public void DecreasePossibleActionsPerLevel()
        {
            currNumPossibleActionsPerLevel--;
            //update UI
            possibleActionsText.text = "Actions: " + currNumPossibleActionsPerLevel;
            statePanel.GetComponent<Animator>().Play(0);
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