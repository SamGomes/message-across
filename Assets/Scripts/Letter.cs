using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour {

    public float speed;
    public string letterText;
    private float totalTranslation = 0;


	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float translation = Time.deltaTime * speed;
        transform.Translate(translation, 0, 0);
        totalTranslation += translation;
        
    }

    void destroyLetter() {
        Destroy(GetComponent<SpriteRenderer>().gameObject);
        Destroy(this.gameObject);
    }
}
