namespace ConnectFoods.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "level", menuName = "LevelData", order = 0)]
    public class LevelData : ScriptableObject
    {
        [Serializable]
        public struct TargetObjective
        {
            public string name;
            public int count;
        }

        public int level;
        public int totalMove;
        public int rowCount;
        public int columnCount;
        [Tooltip("Target Names : apple - leaf - banana - blob - baloon")] 
        public List<TargetObjective> targets;
    }
}