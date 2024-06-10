using System.Collections.Generic;
using System.Linq;
using _Modules.ObjectPooling.Scripts.Enums;
using _Modules.ObjectPooling.Scripts.Signals;
using Runtime.Controllers.Objects;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Runtime.Extentions;
using Runtime.Managers;
using Runtime.Signals;
using UnityEngine;

namespace Runtime.Commands
{
    public class OnGridCreator
    {
        private readonly Transform _gridHolder;
        private readonly Transform _passengerHolderParent;
        private readonly Dictionary<ColorTypes, int> _colorCounts;
        private Dictionary<ColorTypes, Material> _materialDictionary;
        private const float GridSpacing = 0.1f;

        public OnGridCreator(Transform gridHolder, Transform passengerHolder)
        {
            _gridHolder = gridHolder;
            _passengerHolderParent = passengerHolder;
            _colorCounts = new Dictionary<ColorTypes, int>();
        }

        public void SetMaterialDictionary(Dictionary<ColorTypes, Material> materialDictionary)
        {
            _materialDictionary = materialDictionary;
        }
        
        public List<CellArea> CreateGrid(byte width, byte length, List<CellArea> cellData)
        {
            var cells = new List<CellArea>();

            for (var x = 0; x < width; x++)
            {
                var totalWidth = width * (1 + GridSpacing) - GridSpacing;
                var xOffset = -totalWidth / 2 + .5f;

                for (var z = 0; z < length; z++)
                {
                    var position = new Vector3(x * (1 + GridSpacing) + xOffset, 0, -z * (1 + GridSpacing) - 0.5f);
                    var tile = PoolSignals.Instance.OnGetPoolableGameObject?.Invoke(PoolTypes.TileEditor, _gridHolder, position, Quaternion.identity);
                    
                    var tileEditor = tile?.GetComponent<TileEditor>();

                    tileEditor!.IsClickable = true;
                    
                    var cell = cellData.FirstOrDefault(cell => cell.position == new Vector2Int(x, z)) ?? new CellArea
                    {
                        position = new Vector2Int(x, z), 
                        gridType = GridTypes.Normal,
                        passengerArea = new PassengerArea(){colorType = ColorTypes.None}
                    };

                    var passengerCopy = new PassengerArea { colorType = cell.passengerArea.colorType };

                    var cellCopy = new CellArea
                    {
                        position = cell.position,
                        gridType = cell.gridType,
                        passengerArea = passengerCopy
                    };
                    
                    tileEditor?.Initialize(cellCopy);
                    cells.Add(cellCopy);

                    if (cellCopy.passengerArea.colorType == ColorTypes.None) continue;
                    _colorCounts.TryAdd(cellCopy.passengerArea.colorType, 0);

                    _colorCounts[cellCopy.passengerArea.colorType]++;
                    CreatePassengerEditor(tile?.transform, _passengerHolderParent, cellCopy.passengerArea.colorType);
                }
            }
            return cells;
        }
        
        public void CreateGridFunc(int levelID)
        {
            var width = CoreGameSignals.OnGetLevelData?.Invoke(levelID).gridWidth;
            var length = CoreGameSignals.OnGetLevelData?.Invoke(levelID).gridWidth;
            var cellData = CoreGameSignals.OnGetLevelData?.Invoke(levelID).cells;
            
            for (var x = 0; x < width; x++)
            {
                var totalWidth = width * (1 + GridSpacing) - GridSpacing;
                var xOffset = -totalWidth / 2 + .5f;

                for (var z = 0; z < length; z++)
                {
                    var position = new Vector3((float)(x * (1 + GridSpacing) + xOffset), 0, -z * (1 + GridSpacing) - 0.5f);
                    var tile = PoolSignals.Instance.OnGetPoolableGameObject?.Invoke(PoolTypes.Tile, _gridHolder, position, Quaternion.identity);
            
                    var tileEditor = tile?.GetComponent<TileController>();
                    
                    var cell = cellData?.FirstOrDefault(cell => cell.position == new Vector2Int(x, z)) ?? new CellArea
                    {
                        position = new Vector2Int(x, z), 
                        gridType = GridTypes.Normal,
                        passengerArea = new PassengerArea(){colorType = ColorTypes.None}
                    };

                    var passengerCopy = new PassengerArea { colorType = cell.passengerArea.colorType };

                    var cellCopy = new CellArea
                    {
                        position = cell.position,
                        gridType = cell.gridType,
                        passengerArea = passengerCopy
                    };
            
                    tileEditor?.Initialize(cellCopy);

                    if (cellCopy.passengerArea.colorType == ColorTypes.None) continue;
                    _colorCounts.TryAdd(cellCopy.passengerArea.colorType, 0);

                    _colorCounts[cellCopy.passengerArea.colorType]++;
                    CreatePassenger(tile?.transform, _passengerHolderParent, cellCopy.passengerArea.colorType);
                }
            }
        }
        
        private void CreatePassengerEditor(Transform position, Transform parent, ColorTypes colorType)
        {
            var passenger = PoolSignals.Instance.OnGetPoolableGameObject?.Invoke(PoolTypes.PassengerEditor, parent, position.position, Quaternion.identity);
            var renderer = passenger?.GetComponentInChildren<Renderer>();
            renderer!.material = _materialDictionary[colorType];
            passenger.GetComponent<PassengerEditor>().Initialize(colorType);
            
            passenger.SetColliderPassengerEditor(false);
        }
        
        private void CreatePassenger(Transform position, Transform parent, ColorTypes colorType)
        {
            var passenger = PoolSignals.Instance.OnGetPoolableGameObject?.Invoke(PoolTypes.Passenger, parent, position.position, Quaternion.identity);
            var renderer = passenger?.GetComponentInChildren<Renderer>();
            renderer!.material = _materialDictionary[colorType];
            passenger.GetComponent<PassengerController>().Initialize(new PassengerArea{colorType = colorType});
            
            //passenger.SetColliderPassengerEditor(false);
        }
    }
}