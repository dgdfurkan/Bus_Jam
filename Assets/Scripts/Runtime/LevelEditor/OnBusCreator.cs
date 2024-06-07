using System.Collections.Generic;
using System.Linq;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.LevelEditor
{
    public class OnBusCreator
    {
        private readonly GameObject _busPrefab;
        private readonly Transform _busParent;
        private readonly Queue<Vector3> _spawnPositions;
        private Dictionary<ColorTypes, Material> _materialDictionary;

        public OnBusCreator(GameObject busPrefab, Transform busParent)
        {
            _busPrefab = busPrefab;
            _busParent = busParent;
            _spawnPositions = new Queue<Vector3>();

            InitializeSpawnPositions(LevelEditorManager.Instance.GetTotalPassengerCount() / 3);
        }

        public void SetMaterialDictionary(Dictionary<ColorTypes, Material> materialDictionary)
        {
            _materialDictionary = materialDictionary;
        }

        public void InitializeSpawnPositions(int count)
        {
            var startPosition = _busParent.position;
            var offset = new Vector3(-6, 0, 0);

            for (var i = 0; i < count; i++)
            {
                _spawnPositions.Enqueue(startPosition + i * offset);
            }
        }

        public List<BusArea> CreateBuses(List<CellArea> cells)
        {
            var buses = new List<BusArea>();
            var colorCounts = new Dictionary<ColorTypes, int>();

            foreach (var cell in cells.Where(cell => cell.colorTypes != ColorTypes.None))
            {
                colorCounts.TryAdd(cell.colorTypes, 0);

                colorCounts[cell.colorTypes]++;

                if (colorCounts[cell.colorTypes] < 3) continue;

                var busArea = new BusArea
                {
                    colorTypes = cell.colorTypes,
                    color = Color.white
                };
                buses.Add(busArea);

                CreateBus(busArea.colorTypes);

                colorCounts[cell.colorTypes] = 0;
            }

            return buses;
        }

        private void CreateBus(ColorTypes colorType)
        {
            if (_spawnPositions.Count == 0)
            {
                InitializeSpawnPositions(LevelEditorManager.Instance.GetTotalPassengerCount() / 3);
            }

            var position = _spawnPositions.Dequeue();
            var bus = UnityEngine.Object.Instantiate(_busPrefab, position, Quaternion.identity, _busParent);
            var renderer = bus.GetComponentInChildren<Renderer>();
            renderer.material = _materialDictionary[colorType];
            bus.GetComponent<BusEditor>().Initialize(colorType);
        }
        
        public void ResetSpawnPositions()
        {
            _spawnPositions.Clear();
        }
    }
}