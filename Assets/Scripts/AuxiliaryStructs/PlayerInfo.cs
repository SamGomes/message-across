using System;
using System.Collections.Generic;
using UnityEngine;

namespace AuxiliaryStructs
{
    [Serializable]
    public class PlayerInfo
    {
        [SerializeField]
        public List<float> colorRGB;

        [SerializeField]
        public int numPossibleActionsPerLevel;
    }
}