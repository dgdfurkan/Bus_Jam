using System.Collections.Generic;
using System.Linq;
using _Modules.ObjectPooling.Scripts.Enums;
using _Modules.ObjectPooling.Scripts.Signals;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.LevelEditor
{
    public class OnBusCreator
    {
        private readonly Transform _busParent;
        private readonly Queue<Vector3> _spawnPositions;
        private Dictionary<ColorTypes, Material> _materialDictionary;

        public OnBusCreator(Transform busParent)
        {
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
            //var xOffset = PoolSignals.OnGetPrefabScale(PoolTypes.BusEditor).x;
            //Debug.Log($"xOffset: {xOffset}");
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

            foreach (var cell in cells.Where(cell => cell.passengerArea.colorType != ColorTypes.None))
            {
                colorCounts.TryAdd(cell.passengerArea.colorType, 0);

                colorCounts[cell.passengerArea.colorType]++;

                if (colorCounts[cell.passengerArea.colorType] < 3) continue;

                var busArea = new BusArea
                {
                    colorType = cell.passengerArea.colorType
                };
                buses.Add(busArea);

                CreateBus(busArea.colorType);

                colorCounts[cell.passengerArea.colorType] = 0;
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
            var bus = PoolSignals.OnGetPoolableGameObject(PoolTypes.BusEditor, _busParent, position, Quaternion.identity);
            //var bus = UnityEngine.Object.Instantiate(_busPrefab, position, Quaternion.identity, _busParent);
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