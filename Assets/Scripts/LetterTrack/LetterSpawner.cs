﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LetterSpawner : MonoBehaviour
{
    public GameObject letterPrefab;
    public GameManager gameManager;

    public float minIntervalRange;
    public float maxIntervalRange;

    private float randomInterval;

    private string currStarredWord;


    private char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'L', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
                                 //"1", "2", "3", "4", "5", "6", "7", "8", "9"};
    private List<char> lettersPool;

    private void Awake()
    {
        lettersPool = new List<char>();
    }

    // Use this for initialization
    void Start()
    {
        float initialDelayInSeconds = 1.0f;
        StartCoroutine(SpawnLetterWithDelay(initialDelayInSeconds));
    }


    public void UpdateCurrStarredWord(string currTargetWord)
    {
        this.currStarredWord = currTargetWord;
    }

    IEnumerator SpawnLetterWithDelay(float sec)
    {

        yield return new WaitForSeconds(sec);
        if (lettersPool.Count == 0)
        {
            ResetPool();
        }
        GameObject newLetter = Instantiate(letterPrefab);

        SpriteRenderer letterRenderer = newLetter.transform.GetComponent<SpriteRenderer>();

        int random = Random.Range(0, lettersPool.Count - 1);
        char currLetter = lettersPool[random];
        lettersPool.RemoveAt(random);

        //currLetter = 'A';

        string path = "Textures/Alphabet/" + currLetter;

        newLetter.GetComponent<Letter>().letterText = currLetter;
        if (currStarredWord.Contains(currLetter.ToString().ToUpper()))
        {
            newLetter.GetComponent<SpriteRenderer>().color = Color.cyan;
        }
        //newLetter.GetComponent<Letter>().speed = newLetter.GetComponent<Letter>().speed + ((score + 1) * 0.05f);

        letterRenderer.sprite = (Sprite) Resources.Load(path, typeof(Sprite));

        newLetter.transform.position = gameObject.transform.position;
        newLetter.transform.rotation = gameObject.transform.rotation;

        randomInterval = Random.Range(minIntervalRange, maxIntervalRange);
        StartCoroutine(SpawnLetterWithDelay(randomInterval));
    }

    private void ResetPool()
    {
        lettersPool = letters.ToList<char>();
        List<char> currWordLetters = gameManager.currExercise.targetWord.ToCharArray().ToList<char>();

        //bias generation of letters in word
        lettersPool.AddRange(currWordLetters);
        lettersPool.AddRange(currWordLetters);
    }


}
