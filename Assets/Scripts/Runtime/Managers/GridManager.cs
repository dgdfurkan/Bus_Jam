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
        // public GameObject tilePrefab;
        // public Transform gridParent;
        //
        // private List<GameObject> _gridTiles = new List<GameObject>();
        // private List<CellArea> _cells = new List<CellArea>();

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
            CoreGameSignals.OnLoadLevelInitialize += LoadLevel;
        }

        private void OnDisable()
        {
            CoreGameSignals.OnLoadLevelInitialize -= LoadLevel;
        }

        private void LoadLevel(int levelID)
        {
            
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


        // public List<CellArea> CreateGrid(int width, int length)
        // {
        //     ClearGrid();
        //
        //     var xOffset = width % 2 == 0 ? -width / 2 : -(width - 1) / 2;
        //     var zOffset = length % 2 == 0 ? -length / 2 : -(length - 1) / 2;
        //
        //     for (var x = 0; x < width; x++)
        //     {
        //         for (var z = 0; z < length; z++)
        //         {
        //             var position = new Vector3(x + xOffset, 0, z + zOffset);
        //             var tile = Instantiate(tilePrefab, position, Quaternion.identity, gridParent);
        //             _gridTiles.Add(tile);
        //
        //             var cell = new CellArea
        //             {
        //                 position = new Vector2Int(x + xOffset, z),
        //                 gridType = GridTypes.Normal,
        //                 //colorTypes = ColorTypes.None,
        //                 passengerArea = new PassengerArea{colorType = ColorTypes.None}
        //             };
        //             _cells.Add(cell);
        //         }
        //     }
        //
        //     return _cells;
        // }
        //
        // private void ClearGrid()
        // {
        //     foreach (var tile in _gridTiles)
        //     {
        //         DestroyImmediate(tile);
        //     }
        //
        //     _gridTiles.Clear();
        //     _cells.Clear();
        // }
        //
        // public void PopulateGrid(List<CellArea> savedCells)
        // {
        //     ClearGrid();
        //
        //     foreach (var cell in savedCells)
        //     {
        //         var position = new Vector3(cell.position.x, 0, cell.position.y);
        //         var tile = Instantiate(tilePrefab, position, Quaternion.identity, gridParent);
        //         _gridTiles.Add(tile);
        //     }
        //
        //     _cells = savedCells;
        // }
    }
}