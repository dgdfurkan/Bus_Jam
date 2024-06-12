using System;
using _Modules.ObjectPooling.Scripts.Enums;
using _Modules.ObjectPooling.Scripts.Signals;
using DG.Tweening;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Runtime.Extentions;
using Runtime.Managers;
using Runtime.Signals;
using UnityEngine;

namespace Runtime.Controllers.Objects
{
    public class PassengerController : MonoBehaviour
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
        private float _speed = 0.25f;
        
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
            _oldRotation = transform.rotation;
        }

        public void Initialize(CellArea area, PassengerArea data)
        {
            _cellArea = area;
            Data = data;
            SetOutline();
        }
        
        public void SetOutline()
        {
            outline.enabled = PathfindingSignals.OnGetCanMove(_cellArea.position);
        }

        private void Start()
        {
            SetOutline();
        }

        private void OnMouseDown()
        {
            var busColor = CoreGameSignals.OnGetCurrentBusColor?.Invoke();
            
            var moveable = PathfindingSignals.OnGetCanMove(_cellArea.position);
            print(moveable ? true.ToString().ColoredText(Color.green) : false.ToString().ColoredText(Color.red));

            if (!moveable) return;
            
            PathfindingSignals.OnGetVector2IntPath.Invoke(_cellArea.position);
            CoreGameSignals.OnUpdateCellArea?.Invoke(_cellArea);
            
            print(Data.colorType + "and" + busColor);
            var firstPath = PathfindingSignals.OnGet3DPath.Invoke();
            
            print("Saaaa");
            print(firstPath);
            print(firstPath.Length);

            //if (firstPath.Length == 0) return;
            
            _firstTween.Kill();
            _firstTween = DoPath(firstPath)
                .OnComplete(() =>
                {
                    if (busColor == Data.colorType)
                    {
                        _secondTween.Kill();
                        _secondTween = MoveToBus();
                    }
                    else
                    {
                        _secondTween.Kill();
                        _secondTween = MoveToBusStop();
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
            var currentBus = _levelManager.buses.Peek();
            if(currentBus == null) return null;
            
            //DOTween.To(() => animator.GetFloat(Speed), x => animator.SetFloat(Speed, x), 1, .2f);
            var busPosition = currentBus.gameObject.transform.position;
            var targetPosition = new Vector3(busPosition.x, transform.position.y, busPosition.z);
            return transform.DOMove(targetPosition, _speed * 3)
                .SetEase(Ease.Linear)
                .OnStart(()=> transform.DOLookAt(currentBus.gameObject.transform.position, _speed)
                .OnComplete(() =>
                {
                    DOTween.To(() => animator.GetFloat(Speed), x => animator.SetFloat(Speed, x), 0, .2f);
                    currentBus.IncreasePassengerCount();
                    // TODO: Otobüste üst kata sırayla oturacaklar.
                    transform.position =new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
                    //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + 90, transform.rotation.z);
                    transform.Rotate(0, 90, 0);
                    animator.SetTrigger(Sit);
                }));
        }
        
        private Tween MoveToBusStop()
        {
            var availableBusStop = CoreGameSignals.OnSendAvailableBusStop.Invoke();
            if(availableBusStop == null) return null;
            
            availableBusStop.AssignPassenger(this);
            
            //DOTween.To(() => animator.GetFloat(Speed), x => animator.SetFloat(Speed, x), 1, .2f);
            return transform.DOMove(availableBusStop.gameObject.transform.position, _speed)
                .SetEase(Ease.Linear)
                .OnStart(()=> transform.DOLookAt(availableBusStop.gameObject.transform.position, _speed)
                .OnComplete(() =>
                {
                    transform.rotation = _oldRotation;
                    DOTween.To(() => animator.GetFloat(Speed), x => animator.SetFloat(Speed, x), 0, .2f);
                }));
        }

        public void MoveBus()
        {
            _thirdTween.Kill();
            _thirdTween = MoveToBus();
        }
    }
}