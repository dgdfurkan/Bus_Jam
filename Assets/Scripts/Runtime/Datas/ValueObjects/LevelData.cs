using System;
using System.Collections.Generic;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.Datas.ValueObjects
{
    [Serializable]
    public class LevelData
    {
        public int levelID;
        public byte gridWidth;
        public byte gridLength;
        public float time;
        
        public List<BusArea> buses;
        public List<CellArea> cells;
    }

    [Serializable]
    public class BusArea
    {
        public ColorTypes colorTypes;
        public Color color;
    }

    [Serializable]
    public class CellArea
    {
        public Vector2Int position;
        public GridTypes gridTypes;
        public ColorTypes colorTypes;
    }
}

