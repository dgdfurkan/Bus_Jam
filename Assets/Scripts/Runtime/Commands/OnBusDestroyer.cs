using _Modules.ObjectPooling.Scripts.Enums;
using Runtime.Extentions;
using UnityEngine;

namespace Runtime.Commands
{
    public class OnBusDestroyer
    {
        private readonly Transform _busHolder;

        public OnBusDestroyer(Transform busHolder)
        {
            _busHolder = busHolder;
        }

        public void DestroyBusEditor()
        {
            _busHolder.ClearChildren(PoolTypes.BusEditor);
        }
        
        public void DestroyBusFunc()
        {
            _busHolder.ClearChildren(PoolTypes.Bus);
        }
    }
}