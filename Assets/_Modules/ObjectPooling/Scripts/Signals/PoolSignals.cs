using System;
using _Modules.ObjectPooling.Scripts.Enums;
using Runtime.Extentions;
using UnityEngine;
using UnityEngine.Events;

namespace _Modules.ObjectPooling.Scripts.Signals
{
    public class PoolSignals : MonoSingleton<PoolSignals>
    {
        public Func<PoolTypes, Transform, Vector3, Quaternion, GameObject> OnGetPoolableGameObject = delegate { return null; };
        public UnityAction<GameObject, PoolTypes> OnSetPooledGameObject = delegate { };
        public Func<PoolTypes, Vector3> OnGetPrefabScale = delegate { return Vector3.one; };
    }
}