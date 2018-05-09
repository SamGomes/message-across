using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSpawner : MonoBehaviour
{

    public GameObject antPrefab;
    public GameObject QueenAnt;

    // Use this for initialization
    void Start()
    {
        /// spawnAnt();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void spawnAnt(string currTargetWord)
    {
        GameObject ant = Instantiate(antPrefab, this.transform.position, Quaternion.identity);
        Ant antScript = ant.GetComponent<Ant>();
        Animator queenAnimator = QueenAnt.GetComponent<Animator>();
        antScript.setQueenAnimator(queenAnimator);

        antScript.setCargo(currTargetWord);
    }
}
