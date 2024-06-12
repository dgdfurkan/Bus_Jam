using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Runtime.Signals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Controllers
{
    public class PathFinding : MonoBehaviour
    {
        private LevelData _levelData;
        private List<Vector2Int> _shortestPath;
        private List<Vector3> _3dPathVectors;
        private static List<CellArea> CurrentCells => CoreGameSignals.OnGetCellArea.Invoke();
     
        private const float GridSpacing = 0.1f;
        
        private void OnEnable()
        {
            CoreGameSignals.OnLevelInitialize += OnLevelInitialize;
            PathfindingSignals.OnGetCanMove += CanMoveToTopRow;
            PathfindingSignals.OnGetVector2IntPath += FindPath;
            PathfindingSignals.OnGet3DPath += ConvertPathTo3D;
        }
        
        private void OnLevelInitialize(int levelId)
        {
            _levelData = CoreGameSignals.OnGetLevelData(levelId);
        }

        private void OnDisable()
        {
            CoreGameSignals.OnLevelInitialize -= OnLevelInitialize;
            PathfindingSignals.OnGetCanMove -= CanMoveToTopRow;
            PathfindingSignals.OnGetVector2IntPath -= FindPath;
            PathfindingSignals.OnGet3DPath -= ConvertPathTo3D;
        }
        

        [Button("Can Move To Top Row")]
        public bool CanMoveToTopRow(Vector2Int startPosition)
        {
            var startCell = GetCell(startPosition);
            if (startCell == null || startCell.gridType == GridTypes.Disabled || startCell.passengerArea.colorType == ColorTypes.None)
            {
                return false;
            }
            return startCell.position.y == 0 || CurrentCells.Where(cell => cell.position.y == 0 && cell.gridType == GridTypes.Normal 
               && cell.passengerArea.colorType == ColorTypes.None).Any(cell => PathExists(startPosition, cell.position));
        }

        private CellArea GetCell(Vector2Int position)
        {
            return CurrentCells.Find(cell => cell.position == position);
        }

        private bool PathExists(Vector2Int start, Vector2Int end)
        {
            return CalculatePath(start, end).Count > 0;
        }

        private List<Vector2Int> GetNeighbors(Vector2Int position)
        {
            var neighbors = new List<Vector2Int>
            {
                new(position.x + 1, position.y),
                new(position.x - 1, position.y),
                new(position.x, position.y + 1),
                new(position.x, position.y - 1)
            };

            neighbors.RemoveAll(p => p.x < 0 || p.y < 0 || p.x >= _levelData.gridWidth || p.y >= _levelData.gridLength);
            return neighbors;
        }

        private bool IsWalkable(Vector2Int position)
        {
            var cell = GetCell(position);
            return cell is { gridType: GridTypes.Normal } && cell.passengerArea.colorType == ColorTypes.None;
        }
        
        [Button("Find Path List")]
        public List<Vector2Int> FindPath(Vector2Int startPosition)
        {
            _shortestPath = null;
            
            var startCell = GetCell(startPosition);
        
            if (startCell == null || startCell.gridType == GridTypes.Disabled || startCell.passengerArea.colorType == ColorTypes.None)
            {
                return new List<Vector2Int>();
            }

            if (startCell.position.y == 0)
            {
                return new List<Vector2Int> { startPosition };
            }

            List<Vector2Int> shortestPath = null;
            
            foreach (var cell in CurrentCells.Where(c => c.position.y == 0 && c.gridType == GridTypes.Normal && c.passengerArea.colorType == ColorTypes.None))
            {
                var path = CalculatePath(startPosition, cell.position);
                if (path.Count > 0 && (shortestPath == null || path.Count < shortestPath.Count))
                {
                    shortestPath = path;
                }
            }

            _shortestPath = shortestPath;
            return shortestPath ?? new List<Vector2Int>();
        }
        
        [Button("Convert Path To 3D")]
        public Vector3[] ConvertPathTo3D()
        {
            _3dPathVectors = new List<Vector3>();

            print($"_shortestPath.Count : {_shortestPath.Count}");
            if (_shortestPath.Count == 1) return null;
            
            foreach (var point in _shortestPath)
            {
                var totalWidth = _levelData.gridWidth * (1 + GridSpacing) - GridSpacing;
                var xOffset = -totalWidth / 2f + 0.5f;
                var x = point.x * (1 + GridSpacing) + xOffset;

                var z = -point.y * (1 + GridSpacing) - 0.5f;

                _3dPathVectors.Add(new Vector3(x, 0, z)); 
            }

            return _3dPathVectors.ToArray();
        }

        private List<Vector2Int> CalculatePath(Vector2Int start, Vector2Int end)
        {
            var visited = new HashSet<Vector2Int>();
            var queue = new Queue<Vector2Int>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();

            queue.Enqueue(start);
            visited.Add(start);
            cameFrom[start] = start;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current == end)
                {
                    return ReconstructPath(cameFrom, start, end);
                }

                foreach (var neighbor in GetNeighbors(current).Where(neighbor => !visited.Contains(neighbor) && IsWalkable(neighbor)))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                }
            }

            return new List<Vector2Int>();
        }

        private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
        {
            var path = new List<Vector2Int>();
            var current = end;

            while (current != start)
            {
                path.Add(current);
                current = cameFrom[current];
            }

            path.Add(start);
            path.Reverse();
            return path;
        }
    }
}