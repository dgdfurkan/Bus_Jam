using System.Collections.Generic;
using System.Linq;
using _Modules.ObjectPooling.Scripts.Enums;
using _Modules.ObjectPooling.Scripts.Signals;
using Runtime.Controllers.Objects;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Runtime.Managers;
using Runtime.Signals;
using UnityEngine;

namespace Runtime.Commands
{
    public class OnBusCreator
    {
        private readonly Transform _busHolder;
        private readonly Queue<Vector3> _spawnPositions;
        private Dictionary<ColorTypes, Material> _materialDictionary;

        public OnBusCreator(Transform busHolder)
        {
            _busHolder = busHolder;
            _spawnPositions = new Queue<Vector3>();

            InitializeSpawnPositions(/*LevelEditorManager.Instance.GetTotalPassengerCount()*/ CoreGameSignals.OnGetTotalPassengerCount.Invoke() / 3);
        }

        public void SetMaterialDictionary(Dictionary<ColorTypes, Material> materialDictionary)
        {
            _materialDictionary = materialDictionary;
        }

        public void InitializeSpawnPositions(int count)
        {
            var startPosition = _busHolder.position;
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

                CreateBusEditor(busArea.colorType);

                colorCounts[cell.passengerArea.colorType] = 0;
            }

            return buses;
        }

        public void CreateBusesFunc(int levelID)
        {
            var levelData = CoreGameSignals.OnGetLevelData.Invoke(levelID);
            //var buses = CreateBuses(levelData.cells);
            
            var buses = new List<BusArea>();
            var colorCounts = new Dictionary<ColorTypes, int>();

            foreach (var cell in levelData.cells.Where(cell => cell.passengerArea.colorType != ColorTypes.None))
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
        }

        private void CreateBusEditor(ColorTypes colorType)
        {
            if (_spawnPositions.Count == 0)
            {
                InitializeSpawnPositions(CoreGameSignals.OnGetTotalPassengerCount.Invoke() / 3);
            }

            var position = _spawnPositions.Dequeue();
            var bus = PoolSignals.Instance.OnGetPoolableGameObject(PoolTypes.BusEditor, _busHolder, position, Quaternion.identity);
            //var bus = UnityEngine.Object.Instantiate(_busPrefab, position, Quaternion.identity, _busParent);
            var renderer = bus.GetComponentInChildren<Renderer>();
            renderer.material = _materialDictionary[colorType];
            bus.GetComponent<BusEditor>().Initialize(colorType);
        }
        
        private void CreateBus(ColorTypes colorType)
        {
            if (_spawnPositions.Count == 0)
            {
                InitializeSpawnPositions(CoreGameSignals.OnGetTotalPassengerCount.Invoke() / 3);
            }

            var position = _spawnPositions.Dequeue();
            var bus = PoolSignals.Instance.OnGetPoolableGameObject(PoolTypes.Bus, _busHolder, position, Quaternion.identity);
            //var bus = UnityEngine.Object.Instantiate(_busPrefab, position, Quaternion.identity, _busParent);
            var renderer = bus.GetComponentInChildren<Renderer>();
            renderer.material = _materialDictionary[colorType];
            bus.GetComponent<BusController>().Initialize(new BusArea{colorType = colorType});
        }
        
        public void ResetSpawnPositions()
        {
            _spawnPositions.Clear();
        }
    }
}