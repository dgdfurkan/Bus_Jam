using System;
using _Modules.ObjectPooling.Scripts.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Modules.ObjectPooling.Scripts.Datas.ValueObjects
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class PoolData
    {
        public int amount;
        public GameObject prefab;

        // public Attribute Data; 

        [HideInInspector] 
        public PoolTypes type;
    }
}