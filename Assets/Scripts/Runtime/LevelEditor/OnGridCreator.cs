using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.LevelEditor
{
    public class OnGridCreator
    {
        private readonly Transform _gridParent;
        private readonly GameObject _tilePrefab;
        private readonly GameObject _passengerPrefab;
        private readonly Dictionary<ColorTypes, int> _colorCounts;
        private Dictionary<ColorTypes, Material> _materialDictionary;

        public OnGridCreator(Transform gridParent, GameObject tilePrefab, GameObject passengerPrefab)
        {
            _gridParent = gridParent;
            _tilePrefab = tilePrefab;
            _passengerPrefab = passengerPrefab;
            _colorCounts = new Dictionary<ColorTypes, int>();
        }

        public void SetMaterialDictionary(Dictionary<ColorTypes, Material> materialDictionary)
        {
            _materialDictionary = materialDictionary;
        }
        
        public List<CellArea> CreateGrid(byte width, byte length, List<CellArea> cellData)
        {
            var cells = new List<CellArea>();
            const float gridSpacing = 0.1f;

            for (var x = 0; x < width; x++)
            {
                var totalWidth = width * (1 + gridSpacing) - gridSpacing;
                var xOffset = -totalWidth / 2 + .5f;

                for (var z = 0; z < length; z++)
                {
                    var position = new Vector3(x * (1 + gridSpacing) + xOffset, 0, -z * (1 + gridSpacing) - 0.5f);
                    var tile = UnityEngine.Object.Instantiate(_tilePrefab, position, Quaternion.identity, _gridParent);
                    var tileEditor = tile.GetComponent<TileEditor>();

                    var cell = cellData.FirstOrDefault(cell => cell.position == new Vector2Int(x, z)) ?? new CellArea
                    {
                        position = new Vector2Int(x, z), 
                        gridTypes = GridTypes.Normal,
                        colorTypes = ColorTypes.None
                    };

                    tileEditor.Initialize(cell);
                    cells.Add(cell);

                    if (cell.colorTypes == ColorTypes.None) continue;
                    _colorCounts.TryAdd(cell.colorTypes, 0);

                    _colorCounts[cell.colorTypes]++;
                    CreatePassenger(tile.transform, cell.colorTypes);
                }
            }
            return cells;
        }
        
        private void CreatePassenger(Transform parent, ColorTypes colorType)
        {
            var passenger = UnityEngine.Object.Instantiate(_passengerPrefab, parent.position, Quaternion.identity, parent);
            passenger.transform.localScale = Vector3.one;
            var renderer = passenger.GetComponentInChildren<Renderer>();
            renderer.material = _materialDictionary[colorType];
            passenger.GetComponent<PassengerEditor>().Initialize(colorType);
        }
    }
}