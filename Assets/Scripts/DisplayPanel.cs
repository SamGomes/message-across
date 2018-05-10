using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void setTargetImage(string currWord)
    {
        GameObject.Find("food").GetComponent<SpriteRenderer>().sprite = (Sprite) Resources.Load("Textures/FoodItems/" + currWord, typeof(Sprite));
    }

    public void setTargetImage(Texture2D image)
    {
        //GameObject.Find("food").GetComponent<SpriteRenderer>().sprite = image;
    }
}
