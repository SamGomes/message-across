using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letter : MonoBehaviour {

    public float speed;
    public char letterText;

    public bool isTranslationEnabled;

	void Awake() {
        isTranslationEnabled = true;
        speed = 10.5f;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isTranslationEnabled)
        {
            float translation = Time.deltaTime * speed;
            transform.Translate(translation, 0, 0);
        }
    }

    void DestroyLetter() {
        Destroy(this.gameObject);
    }
}
