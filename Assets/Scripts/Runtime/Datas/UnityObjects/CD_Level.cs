using Runtime.Datas.ValueObjects;
using UnityEngine;

namespace Runtime.Datas.UnityObjects
{
    [CreateAssetMenu(fileName = "Level_", menuName = "Bus Jam/CD_Level", order = 0)]
    public class CD_Level : ScriptableObject
    {
        public LevelData Data;
        public LevelData GetData() => Data;
    }
}