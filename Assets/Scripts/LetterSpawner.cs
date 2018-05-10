using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LetterSpawner : MonoBehaviour
{
    public GameObject letterPrefab;
    public float minIntervalRange;
    public float maxIntervalRange;
    private int score = 0;

    private float randomInterval;


    private string[] letters = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "L", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                                 "1", "2", "3", "4", "5", "6", "7", "8", "9"};
    private List<string> lettersPool;

    // Use this for initialization
    void Start()
    {
        resetPool();

        float initialDelayInSeconds = 2.0f;
        StartCoroutine(spawnLetterWithDelay(initialDelayInSeconds));
    }


    private void FixedUpdate()
    {
        //CustomWaitForSeconds(Random.Range(minIntervalRange, maxIntervalRange));
        
    }

    IEnumerator spawnLetterWithDelay(float sec)
    {

        yield return new WaitForSeconds(sec);
        if (lettersPool.Count == 0)
        {
            resetPool();
        }
        GameObject newLetter = Instantiate(letterPrefab);

        SpriteRenderer letterRenderer = newLetter.transform.GetComponent<SpriteRenderer>();

        int random = Random.Range(0, lettersPool.Count - 1);
        string currLetter = lettersPool[random];
        lettersPool.RemoveAt(random);
       
        string path = "Textures/Alphabet/" + currLetter;

        newLetter.GetComponent<Letter>().letterText = currLetter;
        newLetter.GetComponent<Letter>().speed = newLetter.GetComponent<Letter>().speed + ((score + 1) * 0.05f);

        letterRenderer.sprite = (Sprite) Resources.Load(path, typeof(Sprite));

        newLetter.transform.position = gameObject.transform.position;
        newLetter.transform.rotation = gameObject.transform.rotation;

        randomInterval = Random.Range(minIntervalRange, maxIntervalRange);
        StartCoroutine(spawnLetterWithDelay(randomInterval));
    }

    private void resetPool()
    {
        lettersPool = letters.ToList<string>();
    }

    public void setScore(int score) {
        this.score = score;
    }


}
