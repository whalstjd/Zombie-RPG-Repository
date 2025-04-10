using System.Collections.Generic;
using UnityEngine;

namespace DataInfo
{
    [System.Serializable]
    public class Map
    {
        public Floor[] floors;
    }

    [System.Serializable]
    public class Floor
    {
        public string name;
        public int lvLimit = 1;
        public bool isBossClear = false;
        public Stage[] stages;
    }

    [System.Serializable]
    public class Stage
    {
        public string name;
        public bool isBossStage = false;
        public MonsterTypeEnum spawnMonsterType;
        public int spawnCount;
        public List<DecorationEnum> decorations;
        public List<Vector2> decoration_pos;
    }
} 