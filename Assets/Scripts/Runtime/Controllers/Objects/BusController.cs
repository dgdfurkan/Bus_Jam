using System.Collections.Generic;
using _Modules.ObjectPooling.Scripts.Enums;
using _Modules.ObjectPooling.Scripts.Signals;
using DG.Tweening;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Runtime.Signals;
using UnityEngine;

namespace Runtime.Controllers.Objects
{
    public class BusController : MonoBehaviour
    {
        #region Self Variables

        #region Public  Variables

        public BusArea Data { get; private set; }
        [HideInInspector] public bool IsReadyToMove { get; set; } = false;

        #endregion

        #region Serialized Variables

        [SerializeField] private List<Transform> passengerPositions;
        
        #endregion

        #region Private Variables

        private int _currentPassengerCount;
        private readonly int _capacity = 3;
        private readonly List<GameObject> _passengers = new List<GameObject>();

        #endregion

        #endregion

        private void OnEnable()
        {
            _currentPassengerCount = 0;
            _passengers.Clear();
        }

        private void OnDisable()
        {
            _currentPassengerCount = 0;
            _passengers.Clear();
        }

        public bool IsFull()
        {
            return _currentPassengerCount >= _capacity;
        }
        
        public void Initialize(BusArea data)
        {
            Data = data;
        }

        public void IncreasePassengerCount(GameObject obj)
        {
            if (IsFull()) return;
            _currentPassengerCount++;
            _passengers.Add(obj);
        }
        
        public void PlacePassenger()
        {
            _passengers[_currentPassengerCount - 1].transform.SetParent(passengerPositions[_currentPassengerCount - 1]);
            _passengers[_currentPassengerCount - 1].transform.localPosition = Vector3.zero;
            _passengers[_currentPassengerCount - 1].transform.localRotation = Quaternion.identity;
            _passengers[_currentPassengerCount - 1].transform.Rotate(0,90,0);
        }

        public void BusFull()
        {
            if (!IsFull()) return;

            CoreGameSignals.Instance.OnBusFull?.Invoke();
            transform.DOMoveX(transform.position.x + 12f, .6f)
                .SetEase(Ease.InOutCirc)
                .OnComplete(() =>
                {
                    _passengers.ForEach(controller => PoolSignals.Instance.OnSetPooledGameObject(controller.gameObject, PoolTypes.Passenger));
                    PoolSignals.Instance.OnSetPooledGameObject(gameObject, PoolTypes.Bus);
                });
        }

        public void Move(float positionX)
        {
            transform.DOMoveX(positionX, .5f)
                .SetEase(Ease.InOutCirc);
        }
    }
}