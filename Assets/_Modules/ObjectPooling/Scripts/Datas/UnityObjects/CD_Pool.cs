using _Modules.ObjectPooling.Scripts.Datas.ValueObjects;
using _Modules.ObjectPooling.Scripts.Enums;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Modules.ObjectPooling.Scripts.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "CD_Pool", menuName = "Bus Jam/CD_Pool", order = 0)]
    public class CD_Pool : SerializedScriptableObject
    {
        public SerializedDictionary<PoolTypes, PoolData> PoolList;
    }
}