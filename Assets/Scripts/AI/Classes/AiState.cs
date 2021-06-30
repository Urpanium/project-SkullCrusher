using System;
using UnityEngine;

namespace AI.Classes
{
    [Serializable]
    public abstract class AiState
    {
        public string name = "AiState";
        
        public abstract void Update(AiBot bot, Transform player); 
        public abstract bool ShouldSwitch();
    }
}