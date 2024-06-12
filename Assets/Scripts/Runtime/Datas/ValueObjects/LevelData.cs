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
        public ColorTypes colorType;
    }
    
    [Serializable]
    public class PassengerArea
    {
        public ColorTypes colorType;
    }

    [Serializable]
    public class CellArea
    {
        public Vector2Int position;
        public GridTypes gridType;
        public PassengerArea passengerArea;
    }
}

