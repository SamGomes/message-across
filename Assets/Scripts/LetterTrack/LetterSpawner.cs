using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LetterSpawner : MonoBehaviour
{
    public GameObject letterPrefab;
    public GameManager gameManager;

    public float minIntervalRange;
    public float maxIntervalRange;

    private float randomInterval;

    private string currStarredWord;


    private List<char> letters = new List<char>(){ 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    //"1", "2", "3", "4", "5", "6", "7", "8", "9"};


    private List<char> lettersPool;

    public float exerciseWordsPercentage; //to export


    private void Awake()
    {
        exerciseWordsPercentage = 0.8f; //default to 80\%
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
        GameObject newLetter = Instantiate(letterPrefab, transform.GetChild(0));

        TextMesh letterText = newLetter.transform.GetComponentInChildren<TextMesh>();

        int random = Random.Range(0, lettersPool.Count - 1);
        char currLetter = lettersPool[random];
        lettersPool.RemoveAt(random);
        //currLetter = 'A';
        

        newLetter.GetComponent<Letter>().letterText = currLetter;
        if (currStarredWord.Contains(currLetter.ToString().ToUpper()))
        {
            newLetter.GetComponent<SpriteRenderer>().color = Color.cyan;
        }

        letterText.text =  currLetter.ToString();

        newLetter.transform.position = gameObject.transform.position;
        //newLetter.transform.GetChild(0).LookAt(gameManager.camera.transform.position);
        

        randomInterval = Random.Range(minIntervalRange, maxIntervalRange);
        StartCoroutine(SpawnLetterWithDelay(randomInterval));
    }

    private void ResetPool()
    {

        float total = letters.Count / (1.0f - exerciseWordsPercentage);

        List<char> currWordsLetters = new List<char>();
        List<char> remainingWordsLetters = new List<char>();
        foreach (Player player in gameManager.settings.generalSettings.players)
        {
            currWordsLetters = currWordsLetters.Union(player.GetCurrExercise().targetWord.ToCharArray()).ToList<char>();
        }

        lettersPool.AddRange(letters);
        while (lettersPool.Count < total)
        {
            if (remainingWordsLetters.Count == 0)
            {
                remainingWordsLetters.AddRange(currWordsLetters);
            }

            char newLetter = remainingWordsLetters[Random.Range(0, remainingWordsLetters.Count - 1)];
            lettersPool.Add(newLetter);
            remainingWordsLetters.Remove(newLetter);
        }
    }


}
