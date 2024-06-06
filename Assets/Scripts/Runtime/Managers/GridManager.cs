using System.Collections.Generic;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.Managers
{
    public class GridManager : MonoBehaviour
    {
        public GameObject tilePrefab;
        public Transform gridParent;

        private List<GameObject> _gridTiles = new List<GameObject>();
        private List<CellArea> _cells = new List<CellArea>();

        public List<CellArea> CreateGrid(int width, int length)
        {
            ClearGrid();

            var xOffset = width % 2 == 0 ? -width / 2 : -(width - 1) / 2;
            var zOffset = length % 2 == 0 ? -length / 2 : -(length - 1) / 2;

            for (var x = 0; x < width; x++)
            {
                for (var z = 0; z < length; z++)
                {
                    var position = new Vector3(x + xOffset, 0, z + zOffset);
                    var tile = Instantiate(tilePrefab, position, Quaternion.identity, gridParent);
                    _gridTiles.Add(tile);

                    var cell = new CellArea
                    {
                        position = new Vector2Int(x + xOffset, z),
                        gridTypes = GridTypes.Normal,
                        colorTypes = ColorTypes.None
                    };
                    _cells.Add(cell);
                }
            }

            return _cells;
        }

        private void ClearGrid()
        {
            foreach (var tile in _gridTiles)
            {
                DestroyImmediate(tile);
            }

            _gridTiles.Clear();
            _cells.Clear();
        }

        public void PopulateGrid(List<CellArea> savedCells)
        {
            ClearGrid();

            foreach (var cell in savedCells)
            {
                var position = new Vector3(cell.position.x, 0, cell.position.y);
                var tile = Instantiate(tilePrefab, position, Quaternion.identity, gridParent);
                _gridTiles.Add(tile);
            }

            _cells = savedCells;
        }
    }
}