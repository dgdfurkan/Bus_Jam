using System;
using _Modules.ObjectPooling.Scripts.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace _Modules.ObjectPooling.Scripts.Signals
{
    public static class PoolSignals
    {
        public static Func<PoolTypes, Transform, Vector3, Quaternion, GameObject> OnGetPoolableGameObject = delegate { return null; };
        public static UnityAction<GameObject, PoolTypes> OnSetPooledGameObject = delegate { };
        public static Func<PoolTypes, Vector3> OnGetPrefabScale = delegate { return Vector3.one; };
    }
}