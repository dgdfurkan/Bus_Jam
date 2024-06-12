using System.Collections.Generic;
using Runtime.Commands;
using Runtime.Datas.UnityObjects;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Runtime.Signals;
using Sirenix.Utilities;
using UnityEngine;

namespace Runtime.Managers
{
    public class GridManager : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        //
        
        #endregion
        
        #region Serialized Variables

        [Header("Parent Objects")]
        [Space(10)]
        [SerializeField] public Transform gridParent;
        [SerializeField] public Transform passengerParent;
        [SerializeField] public Transform busParent;
        
        #endregion
        
        #region Private Variables
        
        private int _currentLevelID = 1;
        private byte _currentGridWidth = 3;
        private byte _currentGridLength = 3;
        private float _currentTime = 30;

        private List<BusArea> _currentBuses;
        private List<CellArea> _currentCells;
        
        private OnGridCreator _gridCreator;
        private OnGridDestroyer _gridDestroyer;
        private OnBusCreator _busCreator;
        private OnBusDestroyer _busDestroyer;
        
        private CD_Color _colorData;
        private Dictionary<ColorTypes, Material> _colorObjectList;   

        #endregion

        #endregion

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            _colorData = GetColorData();
            
            _gridCreator = new OnGridCreator(gridParent, passengerParent);
            _gridDestroyer = new OnGridDestroyer(gridParent, passengerParent);
            _busCreator = new OnBusCreator(busParent);
            _busDestroyer = new OnBusDestroyer(busParent);
        }
        
        private CD_Color GetColorData()
        {
            return Resources.Load<CD_Color>("Datas/Colors/CD_Color");
        }

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnLoadLevelInitialize += LoadLevel;
        }

        private void LoadLevel(int levelID)
        {
            
        }

        private void OnDisable()
        {
            CoreGameSignals.Instance.OnLoadLevelInitialize -= LoadLevel;
        }
        
        private void Start()
        {
            LoadColorDictionary();
        }

        private void LoadColorDictionary()
        {
            _colorObjectList = new Dictionary<ColorTypes, Material>();
            
            _colorData.ColorList.ForEach(color => _colorObjectList.TryAdd(color.Key, color.Value.material));
    
            _gridCreator.SetMaterialDictionary(_colorObjectList);
            _busCreator.SetMaterialDictionary(_colorObjectList);
        }
    }
}