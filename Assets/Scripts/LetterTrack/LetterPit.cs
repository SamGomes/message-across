using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterPit : MonoBehaviour
{
    void OnTriggerEnter(Collider letter)
    {
        Destroy(letter.gameObject);
    }
}
