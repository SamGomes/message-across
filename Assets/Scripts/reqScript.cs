using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class reqScript : MonoBehaviour {


	public void updateRequirement(string word){
		gameObject.GetComponent<Text>().text = word;
	}

}
