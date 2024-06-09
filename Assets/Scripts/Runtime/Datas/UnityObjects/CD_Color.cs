using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.Datas.UnityObjects
{
    [CreateAssetMenu(fileName = "CD_Color", menuName = "Bus Jam/CD_Color", order = 0)]
    public class CD_Color : SerializedScriptableObject
    {
        public SerializedDictionary<ColorTypes, ColorData> ColorList;
    }
}