using System.Collections.Generic;
using System.Linq;
using Runtime.Commands;
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

        #endregion

        #endregion

        private void Awake()
        {
            (levelHolder = levelHolder == null ? new GameObject("LevelHolder").transform : levelHolder).SetParent(transform);
            _datas = GetDatas();
            _colorData = GetColorData();
            _currentLevel = GetActiveLevel();
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
                .ToList());
        }
        
        private CD_Color GetColorData()
        {
            return Resources.Load<CD_Color>("Datas/Colors/CD_Color");
        }

        private int GetActiveLevel()
        {
            return _currentLevel;
        }
    
        private int GetTotalLevelCount()
        {
            return _datas.Count;
        }
        
        #region Subscribe and UnSubscribe Events
        
        private void OnEnable()
        {
            SubscribeEvents();
            
            _currentLevel = GetLevelID();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.OnLevelInitialize += _levelLoaderCommand.Execute;
            
            CoreGameSignals.OnLevelInitialize += _gridCreator.CreateGridFunc;
            CoreGameSignals.OnLevelInitialize += _busCreator.CreateBusesFunc;
            
            CoreGameSignals.OnClearActiveLevel += _levelDestroyerCommand.Execute;
            
            CoreGameSignals.OnClearActiveLevel += _gridDestroyer.DestroyGridFunc;
            CoreGameSignals.OnClearActiveLevel += _gridDestroyer.DestroyPassengerFunc;
            CoreGameSignals.OnClearActiveLevel += _busDestroyer.DestroyBusFunc;
            
            CoreGameSignals.OnGetLevelID += GetLevelID;
            CoreGameSignals.OnGetLevelData += ReturnGetLevelData;
            CoreGameSignals.OnNextLevel += OnNextLevel;
            CoreGameSignals.OnRestartLevel += OnRestartLevel;
            CoreGameSignals.OnGetTotalPassengerCount += GetTotalPassengerCount;
        }
    
        private int GetLevelID()
        {
            if (!ES3.FileExists()) return 0;
            return ES3.KeyExists("Level") ? ES3.Load<int>("Level") % _totalLevelCount : 0;
        }
        
        private LevelData ReturnGetLevelData(int levelIndex)
        {
            return _datas[levelIndex];
        }
    
        [ButtonGroup("Level")]
        private void OnNextLevel()
        {
            _currentLevel++;
            _currentLevel %= _totalLevelCount;
            SaveSignals.OnSaveGameData?.Invoke();
            CoreGameSignals.OnClearActiveLevel?.Invoke();
            CoreGameSignals.OnReset?.Invoke();
            CoreGameSignals.OnLevelInitialize?.Invoke(GetLevelID());
        }
    
        [ButtonGroup("Level")]
        private void OnRestartLevel()
        {
            SaveSignals.OnSaveGameData?.Invoke();
            CoreGameSignals.OnClearActiveLevel?.Invoke();
            CoreGameSignals.OnReset?.Invoke();
            CoreGameSignals.OnLevelInitialize?.Invoke(GetLevelID());
        }
        
        private int GetTotalPassengerCount()
        {
            var activeLevelData = _datas[_currentLevel];
            return activeLevelData.cells.Count(cell => cell.passengerArea.colorType != ColorTypes.None);
        }

        private void UnsubscribeEvents()
        {
            CoreGameSignals.OnLevelInitialize -= _levelLoaderCommand.Execute;
            
            CoreGameSignals.OnLevelInitialize -= _gridCreator.CreateGridFunc;
            CoreGameSignals.OnLevelInitialize -= _busCreator.CreateBusesFunc;
            
            CoreGameSignals.OnClearActiveLevel -= _levelDestroyerCommand.Execute;
            
            CoreGameSignals.OnClearActiveLevel -= _gridDestroyer.DestroyGridFunc;
            CoreGameSignals.OnClearActiveLevel -= _gridDestroyer.DestroyPassengerFunc;
            CoreGameSignals.OnClearActiveLevel -= _busDestroyer.DestroyBusFunc;
            
            CoreGameSignals.OnGetLevelData -= ReturnGetLevelData;
            CoreGameSignals.OnNextLevel -= OnNextLevel;
            CoreGameSignals.OnRestartLevel -= OnRestartLevel;
            CoreGameSignals.OnGetTotalPassengerCount -= GetTotalPassengerCount;
        }
        
        private void OnDisable()
        {
            UnsubscribeEvents();
        }
        
        #endregion
        
        private void Start()
        {
            print($"Level Count: {ReturnGetLevelData(0).levelID}");
            CoreGameSignals.OnLevelInitialize?.Invoke(GetLevelID());
            //CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Start, 1);
        }
    }
}