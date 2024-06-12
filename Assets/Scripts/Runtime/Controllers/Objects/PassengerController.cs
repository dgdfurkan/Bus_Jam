using System;
using _Modules.ObjectPooling.Scripts.Enums;
using _Modules.ObjectPooling.Scripts.Signals;
using DG.Tweening;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Runtime.Extentions;
using Runtime.Interfaces;
using Runtime.Managers;
using Runtime.Signals;
using Unity.VisualScripting;
using UnityEngine;

namespace Runtime.Controllers.Objects
{
    public class PassengerController : MonoBehaviour, IClickable
    {
        #region Self Variables

        #region Public  Variables

        //public bool IsMoving { get; set; } = false;

        #endregion

        #region Serialized Variables

        [SerializeField] private Animator animator;
        [SerializeField] private Outline outline;

        #endregion

        #region Private Variables

        private CellArea _cellArea;
        public PassengerArea Data { get; private set; }
        private LevelManager _levelManager;
        private readonly float _speed = 0.25f;
        private bool _isMoving = false;
        
        private Tween _firstTween;
        private Tween _secondTween;
        private Tween _thirdTween;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Sit = Animator.StringToHash("Sit");
        private Quaternion _oldRotation;
        
        #endregion

        #endregion
        
        private void Awake()
        {
            _levelManager = FindObjectOfType<LevelManager>();
        }

        public void Initialize(CellArea area, PassengerArea data)
        {
            _cellArea = area;
            Data = data;
            SetOutline();
            _oldRotation = transform.rotation;
        }
        
        public void SetOutline()
        {
            outline.enabled = PathfindingSignals.OnGetCanMove(_cellArea.position);
        }

        private void Start()
        {
            SetOutline();
        }

        public void Click()
        {
            CoreGameSignals.Instance.OnFirstInputTaken?.Invoke();
            
            if(_isMoving) return;
            
            var busColor = CoreGameSignals.Instance.OnGetCurrentBusColor?.Invoke();
            
            _isMoving = PathfindingSignals.OnGetCanMove(_cellArea.position);

            PathfindingSignals.OnGetVector2IntPath.Invoke(_cellArea.position);
            CoreGameSignals.Instance.OnUpdateCellArea?.Invoke(_cellArea);
            
            var firstPath = PathfindingSignals.OnGet3DPath.Invoke();
            
            _firstTween.Kill();
            _firstTween = DoPath(firstPath)
                .OnComplete(() =>
                {
                    transform.rotation = _oldRotation;
                    try
                    {
                        outline.enabled = false;
                        if (busColor == Data.colorType && !_levelManager.buses.Peek().IsFull())
                        {
                            _firstTween.Kill();
                            _firstTween = MoveToBus();
                        }
                        else if(busColor != Data.colorType || busColor == Data.colorType && _levelManager.buses.Peek().IsFull())
                        {
                            _firstTween.Kill();
                            _firstTween = MoveToBusStop();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        print(e);
                        throw;
                    }
                });
        }
        
        private Tween DoPath(Vector3[] path)
        {
            DOTween.To(() => animator.GetFloat(Speed), x => animator.SetFloat(Speed, x), 1,.2f);
            return transform.DOPath(path, _speed * path.Length, PathType.CatmullRom)
                .SetEase(Ease.Linear)
                .OnWaypointChange(waypointIndex =>
                {
                    if (waypointIndex < path.Length - 1)
                    {
                        transform.DOLookAt(path[waypointIndex + 1], 0.5f);
                    }
                });
        }
        
        private Tween MoveToBus()
        {
            outline.enabled = false;
            var currentBus = _levelManager.buses.Peek();
            if(currentBus == null) return null;
            
            currentBus.IncreasePassengerCount(gameObject);
            var busPosition = currentBus.gameObject.transform.position;
            var targetPosition = new Vector3(busPosition.x, transform.position.y, busPosition.z);
            var mesure = Vector3.Distance(targetPosition, transform.position);
            return transform.DOMove(targetPosition, _speed * mesure)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    DOTween.To(() => animator.GetFloat(Speed), x => animator.SetFloat(Speed, x), 0, .2f);
                    currentBus.PlacePassenger();
                    animator.SetTrigger(Sit);
                    DOVirtual.DelayedCall((_speed * mesure) + .5f, () =>
                    {
                        currentBus.BusFull();
                    });
                });
        }
        
        private Tween MoveToBusStop()
        {
            outline.enabled = false;
            var availableBusStop = CoreGameSignals.Instance.OnSendAvailableBusStop.Invoke();
            if(!availableBusStop) return null;
            
            availableBusStop.AssignPassenger(this);
            
            var mesure = Vector3.Distance(availableBusStop.gameObject.transform.position, transform.position);
            return transform.DOMove(availableBusStop.gameObject.transform.position, _speed * mesure)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    transform.rotation = _oldRotation;
                    DOTween.To(() => animator.GetFloat(Speed), x => animator.SetFloat(Speed, x), 0, .2f);
                });
        }

        public void MoveBus()
        {
            _firstTween.Kill();
            _firstTween = MoveToBus();
        }
        
        public void MoveBusStop()
        {
            _firstTween.Kill();
            _firstTween = MoveToBusStop();
        } 
    }
}