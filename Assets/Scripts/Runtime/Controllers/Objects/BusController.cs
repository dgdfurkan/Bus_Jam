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

        //

        #endregion

        #region Serialized Variables

        [SerializeField] private int capacity = 3;
        
        #endregion

        #region Private Variables

        public BusArea Data { get; private set; }
        private int _currentPassengerCount;

        #endregion

        #endregion

        private void Awake()
        {
            _currentPassengerCount = 0;
        }
        
        private bool IsFull()
        {
            return _currentPassengerCount >= capacity;
        }

        public void Initialize(BusArea data)
        {
            Data = data;
        }

        public void IncreasePassengerCount()
        {
            _currentPassengerCount++;
            if (!IsFull()) return;
            
            CoreGameSignals.OnBusFull?.Invoke();
            transform.DOMoveX(transform.position.x + 12f, .6f)
                .SetEase(Ease.InOutCirc)
                .OnComplete(() =>
                {
                    PoolSignals.Instance.OnSetPooledGameObject(gameObject, PoolTypes.Bus);
                });
        }

        public void Move(float positionX)
        {
            print(transform.position.x - positionX);
            transform.DOMoveX(positionX, .5f)
                .SetEase(Ease.InOutCirc);
        }
    }
}