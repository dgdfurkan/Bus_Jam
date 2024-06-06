using System.Collections.Generic;
using System.Linq;
using Runtime.Datas.UnityObjects;
using Runtime.Datas.ValueObjects;
using Runtime.Signals;
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

        [SerializeField] private Transform levelHolder;

        #endregion

        #region Private Variables

        private const string PathOfData = "Datas/Levels";
        private List<LevelData> _datas = new List<LevelData>();

        private OnLevelLoaderCommand _levelLoaderCommand;
        private OnLevelDestroyerCommand _levelDestroyerCommand;

        private int _currentLevel;
        private int _totalLevelCount;

        #endregion

        #endregion

        private void Awake()
        {
            (levelHolder = levelHolder == null ? new GameObject("LevelHolder").transform : levelHolder).SetParent(transform);
            _datas = GetDatas();
            _currentLevel = GetActiveLevel();
            _totalLevelCount = GetTotalLevelCount();
        
            Init();
        }

        private void Init()
        {
            _levelLoaderCommand = new OnLevelLoaderCommand(levelHolder);
            _levelDestroyerCommand = new OnLevelDestroyerCommand(levelHolder);
        }
    
        private List<LevelData> GetDatas()
        {
            return _datas = new List<LevelData>(Resources.LoadAll<CD_Level>(PathOfData)
                .Select(item => item.GetData())
                .ToList());
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
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.OnLevelInitialize += _levelLoaderCommand.Execute;
            CoreGameSignals.OnClearActiveLevel += _levelDestroyerCommand.Execute;
            CoreGameSignals.OnGetLevelValue += OnGetLevelValue;
            CoreGameSignals.OnGetLevelData += ReturnGetLevelData;
            CoreGameSignals.OnNextLevel += OnNextLevel;
            CoreGameSignals.OnRestartLevel += OnRestartLevel;
        }

        private int OnGetLevelValue()
        {
            return _currentLevel % _totalLevelCount;
        }
    
        private LevelData ReturnGetLevelData(int levelIndex)
        {
            return _datas[levelIndex];
        }
    
        private void OnNextLevel()
        {
            _currentLevel++;
            CoreGameSignals.OnClearActiveLevel?.Invoke();
            CoreGameSignals.OnReset?.Invoke();
            CoreGameSignals.OnLevelInitialize?.Invoke(_currentLevel % _totalLevelCount);
        }
    
        private void OnRestartLevel()
        {
            CoreGameSignals.OnClearActiveLevel?.Invoke();
            CoreGameSignals.OnReset?.Invoke();
            CoreGameSignals.OnLevelInitialize?.Invoke(_currentLevel % _totalLevelCount);
        }

        private void UnsubscribeEvents()
        {
            CoreGameSignals.OnLevelInitialize -= _levelLoaderCommand.Execute;
            CoreGameSignals.OnClearActiveLevel -= _levelDestroyerCommand.Execute;
            CoreGameSignals.OnGetLevelValue -= OnGetLevelValue;
            CoreGameSignals.OnGetLevelData -= ReturnGetLevelData;
            CoreGameSignals.OnNextLevel -= OnNextLevel;
            CoreGameSignals.OnRestartLevel -= OnRestartLevel;
        }
        
        private void OnDisable()
        {
            UnsubscribeEvents();
        }
        
        #endregion
        
        private void Start()
        {
            print($"Level Count: {ReturnGetLevelData(0).levelID}");
            CoreGameSignals.OnLevelInitialize?.Invoke(_currentLevel % _totalLevelCount);
            //CoreUISignals.Instance.onOpenPanel?.Invoke(UIPanelTypes.Start, 1);
        }
    }
}