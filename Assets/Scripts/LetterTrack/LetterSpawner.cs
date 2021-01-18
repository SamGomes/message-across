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
    
    public float minIntervalRange;
    public float maxIntervalRange;

    private float randomInterval;

    private string currStarredWord;

    private List<GameObject> currLetters;

    private int id;


    private void Awake()
    {
        currLetters = new List<GameObject>();
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
    public void UpdateCurrStarredWord(string currTargetWord)
    {
        this.currStarredWord = currTargetWord;
    }

    [ClientRpc]
    public void DestroyFirstLetter()
    {
        if (currLetters.Count == 0) //no letters to destroy
        {
            return;
        }
        GameObject firstLetter = currLetters.First();
        currLetters.RemoveAt(0);
        Destroy(firstLetter);
    }
    
    [ClientRpc]
    public void DestroyAllLetters()
    {
        if (currLetters.Count == 0)
        {
            return;
        }
        List<GameObject> letters = new List<GameObject>(currLetters);
        currLetters.Clear();
        foreach (GameObject letter in letters)
        {
            Destroy(letter);
        }
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
        
    }

    public List<GameObject> GetCurrSpawnedLetterObjects()
    {
        return currLetters;
    }


}
