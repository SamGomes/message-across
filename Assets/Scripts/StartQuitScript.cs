﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartQuitScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButton("Start") || Input.GetKeyDown("s")){
            gameObject.GetComponent<AudioSource>().Play();
            SceneManager.LoadScene("crossAnt");
		}
	}
}
