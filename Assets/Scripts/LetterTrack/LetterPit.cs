using System;
using Mirror;
using UnityEngine;

namespace LetterTrack
{
    public class LetterPit: MonoBehaviour
    {
        private void OnTriggerEnter(Collider letter)
        {
            Destroy(letter.gameObject);
        }
    }
}