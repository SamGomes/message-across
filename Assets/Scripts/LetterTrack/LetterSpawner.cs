using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using AuxiliaryStructs;
using Mirror;
using UnityEngine.UI;

public class LetterSpawner : NetworkBehaviour
{
    public GameObject letterPrefab;
    public GameManager gameManager;

    public float minIntervalRange;
    public float maxIntervalRange;

    private float randomInterval;
    private bool isStopped;

    private string currStarredWord;

    
    private List<char> lettersPool;
    private List<GameObject> currLetters;

    private int id;


    private void Awake()
    {
        lettersPool = new List<char>();
        currLetters = new List<GameObject>();
    }

    // Use this for initialization
    void Start()
    {
        isStopped = true;
    }

    public void SetId(int id)
    {
        this.id = id;
    }
    
    public int GetId()
    {
        return id;
    }
    
    [ClientRpc]
    public void StartSpawning()
    {
        isStopped = false;

    }
    
    [ClientRpc]
    public void StopSpawning()
    {
        isStopped = true;

    }

    public void UpdateCurrStarredWord(string currTargetWord)
    {
        this.currStarredWord = currTargetWord;
    }

    public void DestroyCurrLetters()
    {
        foreach (GameObject letter in currLetters)
        {
            Destroy(letter);
        }
    }


    public bool IsStopped()
    {
        return isStopped;
    }

    [ClientRpc]
    public void SpawnLetter(char currLetter)
    {

        GameObject newLetter = Instantiate(letterPrefab, transform.GetChild(0));
        currLetters.Add(newLetter);

        TextMesh letterText = newLetter.transform.GetComponentInChildren<TextMesh>();

        
        newLetter.GetComponent<Letter>().letterText = currLetter;
//        if (currStarredWord.Contains(currLetter.ToString().ToUpper()))
//        {
//            newLetter.GetComponent<SpriteRenderer>().color = Color.cyan;
//        }

        letterText.text =  currLetter.ToString();

        newLetter.transform.position = gameObject.transform.position;
        //newLetter.transform.GetChild(0).LookAt(gameManager.camera.transform.position);
        
    }

    


}
