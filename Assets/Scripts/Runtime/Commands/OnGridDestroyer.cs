using _Modules.ObjectPooling.Scripts.Enums;
using Runtime.Extentions;
using UnityEngine;

namespace Runtime.Commands
{
    public class OnGridDestroyer
    {
        private readonly Transform _gridHolder;
        private readonly Transform _passengerHolder;

        public OnGridDestroyer(Transform gridHolder, Transform passengerHolder)
        {
            _gridHolder = gridHolder;
            _passengerHolder = passengerHolder;
        }

        public void DestroyGridEditor()
        {
            _gridHolder.ClearChildren(PoolTypes.TileEditor);
        }

        public void DestroyGridFunc()
        {
            _gridHolder.ClearChildren(PoolTypes.Tile);
        }

        public void DestroyPassengerEditor()
        {
            _passengerHolder.ClearChildren(PoolTypes.PassengerEditor);
        }
        
        public void DestroyPassengerFunc()
        {
            _passengerHolder.ClearChildren(PoolTypes.Passenger);
        }
    }
}