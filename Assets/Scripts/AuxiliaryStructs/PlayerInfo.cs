using System;
using System.Collections.Generic;
using UnityEngine;

namespace AuxiliaryStructs
{
    [Serializable]
    public class PlayerInfo
    {
        [SerializeField]
        public List<float> buttonRGB;
    
        [SerializeField]
        public string id;

        [SerializeField]
        public List<KeyCode> myKeys;

        [SerializeField]
        public List<string> myButtons;

        [SerializeField]
        public int numPossibleActionsPerLevel;

        //log stuff
        public int numGives;
        public int numTakes;

    }
}