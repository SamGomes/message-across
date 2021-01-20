using System;
using Mirror;
using UnityEngine;

namespace LetterTrack
{
    public class LetterPit: NetworkBehaviour
    {
        [Client]
        private void OnTriggerEnter(Collider letter)
        {
            Destroy(letter.gameObject);
        }
    }
}