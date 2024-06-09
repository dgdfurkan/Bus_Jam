using System;
using Runtime.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Datas.ValueObjects
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class ColorData
    {
        public Material material;
        // public Attribute Data; 

        [HideInInspector] 
        public ColorTypes type;
    }
}