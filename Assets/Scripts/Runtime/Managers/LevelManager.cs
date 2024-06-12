using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Runtime.Commands;
using Runtime.Controllers.Objects;
using Runtime.Datas.UnityObjects;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Runtime.Signals;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Runtime.Managers
{
    public class LevelManager : MonoBehaviour
    {
        #region Self Variables

        #region Public  Variables

        //

        #endregion

        #region Serialized Variables

        [Header("Holder Objects")]
        [Space(10)]
        [SerializeField] private Transform levelHolder;
        [SerializeField] private Transform gridHolder;
        [SerializeField] private Transform passengerHolder;
        [SerializeField] private Transform busHolder;

        [Header("List")]
        [Space(20)]
        [SerializeField] private List<BusStopController> busStops;
        [SerializeField] private List<PassengerController> passengers;
        [SerializeField] private List<TileController> tiles;
        [ShowInInspector] internal Queue<BusController> buses = new Queue<BusController>();
        
        #endregion

        #region Private Variables

        private const string PathOfData = "Datas/Levels";
        private List<LevelData> _datas = new List<LevelData>();

        private OnLevelLoaderCommand _levelLoaderCommand;
        private OnLevelDestroyerCommand _levelDestroyerCommand;

        private int _currentLevelID;
        private byte _currentGridWidth;
        private byte _currentGridLength;
        private float _currentTime;

        private List<BusArea> _currentBuses;
        private List<CellArea> _currentCells;
        
        private OnGridCreator _gridCreator;
        private OnGridDestroyer _gridDestroyer;
        private OnBusCreator _busCreator;
        private OnBusDestroyer _busDestroyer;
        
        private CD_Color _colorData;
        private Dictionary<ColorTypes, Material> _colorObjectList;  
        
        private int _currentLevel;
        private int _totalLevelCount;
        private bool _isFirstInputTaken = false; 
        
        #endregion

        #endregion

        private void Awake()
        {
            (levelHolder = levelHolder == null ? new GameObject("LevelHolder").transform : levelHolder).SetParent(transform);
            _datas = GetDatas();
            _colorData = GetColorData();
            _totalLevelCount = GetTotalLevelCount();
        
            Init();
        }

        private void Init()
        {
            _gridCreator = new OnGridCreator(gridHolder, passengerHolder);
            _gridDestroyer = new OnGridDestroyer(gridHolder, passengerHolder);
            _busCreator = new OnBusCreator(busHolder);
            _busDestroyer = new OnBusDestroyer(busHolder);
            
            _levelLoaderCommand = new OnLevelLoaderCommand(levelHolder);
            _levelDestroyerCommand = new OnLevelDestroyerCommand(levelHolder);
            
            _colorObjectList = new Dictionary<ColorTypes, Material>();
            _colorData.ColorList.ForEach(color
                => _colorObjectList.TryAdd(color.Key, color.Value.material));
            _gridCreator.SetMaterialDictionary(_colorObjectList);
            _busCreator.SetMaterialDictionary(_colorObjectList);
        }
    
        private List<LevelData> GetDatas()
        {
            return _datas = new List<LevelData>(Resources.LoadAll<CD_Level>(PathOfData)
                .Select(item => item.GetData())
                .OrderBy(data => data.levelID)
                .ToList());
        }
        
        private CD_Color GetColorData()
        {
            return Resources.Load<CD_Color>("Datas/Colors/CD_Color");
        }
    
        private int GetTotalLevelCount()
        {
            return _datas.Count;
        }
        
        #region Subscribe and UnSubscribe Events
        
        private void OnEnable()
        {
            SubscribeEvents();
            
            SaveSignals.OnLoadSaveData?.Invoke();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize += _levelLoaderCommand.Execute;
            
            CoreGameSignals.Instance.OnLevelInitialize += _gridCreator.CreateGridFunc;
            CoreGameSignals.Instance.OnLevelInitialize += _busCreator.CreateBusesFunc;
            
            CoreGameSignals.Instance.OnClearActiveLevel += _levelDestroyerCommand.Execute;
            
            CoreGameSignals.Instance.OnClearActiveLevel += _gridDestroyer.DestroyGridFunc;
            CoreGameSignals.Instance.OnClearActiveLevel += _gridDestroyer.DestroyPassengerFunc;
            CoreGameSignals.Instance.OnClearActiveLevel += _busDestroyer.DestroyBusFunc;
            
            CoreGameSignals.Instance.OnGetFirstInput += () => _isFirstInputTaken;
            CoreGameSignals.Instance.OnFirstInputTaken += () => _isFirstInputTaken = true;
            CoreGameSignals.Instance.OnGetLevelID += () => _currentLevel;
            CoreGameSignals.Instance.OnSetLevelID += level => _currentLevel = level;
            CoreGameSignals.Instance.OnGetLevelData += ReturnGetLevelData;
            CoreGameSignals.Instance.OnGetCurrentLevelData += ReturnGetCurrentLevelData;
            CoreGameSignals.Instance.OnNextLevel += OnNextLevel;
            CoreGameSignals.Instance.OnRestartLevel += OnRestartLevel;
            CoreGameSignals.Instance.OnGetTotalPassengerCount += GetTotalPassengerCount;
            CoreGameSignals.Instance.OnBusFull += OnBusFull;
            CoreGameSignals.Instance.OnUpdateCellArea += UpdateCellArea;
            CoreGameSignals.Instance.OnSendAvailableBusStop += OnSendAvailableBusStop;
        }
        
        private LevelData ReturnGetLevelData(int levelIndex)
        {
            return _datas[levelIndex];
        }
        
        private LevelData ReturnGetCurrentLevelData()
        {
            return _datas[_currentLevel];
        }
    
        [ButtonGroup("Level")]
        private void OnNextLevel()
        {
            _currentLevel++;
            _currentLevel %= _totalLevelCount;
            SaveSignals.OnSaveGameData?.Invoke(_currentLevel);
            CoreGameSignals.Instance.OnClearActiveLevel?.Invoke();
            CoreGameSignals.Instance.OnReset?.Invoke();
            CoreGameSignals.Instance.OnLevelInitialize?.Invoke(_currentLevel);
            GetCreatedObjects();
        }
    
        [ButtonGroup("Level")]
        private void OnRestartLevel()
        {
            SaveSignals.OnSaveGameData?.Invoke(_currentLevel);
            GetCreatedObjects();
            CoreGameSignals.Instance.OnClearActiveLevel?.Invoke();
            CoreGameSignals.Instance.OnReset?.Invoke();
            CoreGameSignals.Instance.OnLevelInitialize?.Invoke(_currentLevel);
        }
        
        private int GetTotalPassengerCount()
        {
            var activeLevelData = _datas[_currentLevel];
            return activeLevelData.cells.Count(cell => cell.passengerArea.colorType != ColorTypes.None);
        }

        private void GetCreatedObjects()
        {
            passengers.Clear();
            tiles.Clear();
            //buses.Clear();
            passengers = _gridCreator.GetCreatedPassengers();
            tiles = _gridCreator.GetCreatedTiles();
            buses = _busCreator.GetCreatedBuses();
            CoreGameSignals.Instance.OnUpdateBusColor?.Invoke(buses.Peek().Data);
            //busStops.ForEach(bus => bus.GetBusStopData());
        }
        
        private void OnBusFull()
        {
            var busesArray = buses.ToArray();
            for (var i = 1; i < busesArray.Length; i++)
            {
                var previousBusPositionX = busesArray[i - 1].transform.position.x;
                busesArray[i].Move(previousBusPositionX);
            }
            
            buses.Dequeue();
            //CoreGameSignals.Instance.OnUpdateBusColor?.Invoke(new BusArea{colorType = ColorTypes.None});

            DOVirtual.DelayedCall(0.5f, CheckBusStop);
            DOVirtual.DelayedCall(0.6f, CheckBusStop);

            if (buses.Count != 0) return;
            
            DOVirtual.DelayedCall(1f, () =>
            {
                CoreGameSignals.Instance.OnLevelSuccessful?.Invoke();
            });
        }

        [Button("Check Bus Stop")]
        public void CheckBusStop()
        {
            CoreGameSignals.Instance.OnUpdateBusColor?.Invoke(buses.Peek().Data);

            foreach (var busStop in busStops.Where(busStop => busStop.isOccupied && busStop.Data.colorType == buses.Peek().Data.colorType))
            {
                busStop.PassengerController.MoveBus();
            }
        }

        [Button("passengers.SetOutline")]
        private void UpdateCellArea(CellArea cellArea)
        {
            //passengers.ForEach(passenger => passenger.SetOutline());
            foreach (var passenger in passengers)
            {
                passenger.SetOutline();
            }
        }
        
        private BusStopController OnSendAvailableBusStop()
        {
            var availableBusStop = busStops.Find(stop => !stop.isOccupied);
            return availableBusStop;
        }

        private void UnsubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize -= _levelLoaderCommand.Execute;
            
            CoreGameSignals.Instance.OnLevelInitialize -= _gridCreator.CreateGridFunc;
            CoreGameSignals.Instance.OnLevelInitialize -= _busCreator.CreateBusesFunc;
            
            CoreGameSignals.Instance.OnClearActiveLevel -= _levelDestroyerCommand.Execute;
            
            CoreGameSignals.Instance.OnClearActiveLevel -= _gridDestroyer.DestroyGridFunc;
            CoreGameSignals.Instance.OnClearActiveLevel -= _gridDestroyer.DestroyPassengerFunc;
            CoreGameSignals.Instance.OnClearActiveLevel -= _busDestroyer.DestroyBusFunc;
            
            CoreGameSignals.Instance.OnGetFirstInput -= () => _isFirstInputTaken;
            CoreGameSignals.Instance.OnFirstInputTaken -= () => _isFirstInputTaken = true;
            CoreGameSignals.Instance.OnGetLevelID -= () => _currentLevel;
            CoreGameSignals.Instance.OnSetLevelID -= level => _currentLevel = level;
            CoreGameSignals.Instance.OnGetLevelData -= ReturnGetLevelData;
            CoreGameSignals.Instance.OnGetCurrentLevelData -= ReturnGetCurrentLevelData;
            CoreGameSignals.Instance.OnNextLevel -= OnNextLevel;
            CoreGameSignals.Instance.OnRestartLevel -= OnRestartLevel;
            CoreGameSignals.Instance.OnGetTotalPassengerCount -= GetTotalPassengerCount;
            CoreGameSignals.Instance.OnBusFull -= OnBusFull;
            CoreGameSignals.Instance.OnUpdateCellArea -= UpdateCellArea;
            CoreGameSignals.Instance.OnSendAvailableBusStop -= OnSendAvailableBusStop;
        }
        
        private void OnDisable()
        {
            UnsubscribeEvents();
        }
        
        #endregion
        
        private void Start()
        {
            CoreGameSignals.Instance.OnLevelInitialize?.Invoke(_currentLevel);
            GetCreatedObjects();
            //CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Start, 1);
        }
    }
}