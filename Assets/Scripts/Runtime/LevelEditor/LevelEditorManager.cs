using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Datas.UnityObjects;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Runtime.Extentions;

namespace Runtime.LevelEditor
{
    public class LevelEditorManager : MonoBehaviour
    {
        public static LevelEditorManager Instance;

        public static UnityAction<int> OnValueChanged = delegate { };
        
        [Header("Prefabs & Parent Objects")]
        [Space(10)]
        public Transform gridParent;
        public Transform busParent;
        public GameObject tilePrefab;
        public GameObject passengerPrefab;
        public GameObject busPrefab;
        
        [Header("Materials")]
        [Space(10)]
        public Material redMaterial;
        public Material blueMaterial;
        public Material greenMaterial;
        public Material yellowMaterial;
        public Material purpleMaterial;
        public Material orangeMaterial;

        private Dictionary<ColorTypes, Material> _materialDictionary;
        private Dictionary<ColorTypes, int> _colorCounts;
        
        [Header("UI Elements")]
        [Space(10)]
        
        [SerializeField] private Button createLevelButton;
        [SerializeField] private Button deleteLevelButton;
        [SerializeField] private Button saveLevelButton;
        [SerializeField] private TextMeshProUGUI levelIDText;
        [SerializeField] private TextMeshProUGUI passengerIssuesText;
        
        [Header("Level Data")]
        [Space(10)]
        
        public int levelID = 1;
        public byte gridWidth = 3;
        public byte gridLength = 3;
        public float time = 30;

        [Space(10)]
        public List<BusArea> buses = new List<BusArea>();
        [Space(10)]
        public List<CellArea> cells = new List<CellArea>();
        [Space(10)]
        
        private const string LevelFileName = "Level_";
        private int _totalPassengerCount;
        private OnGridCreator _onGridCreator;
        private OnBusCreator _onBusCreator;

        private void Awake()
        {
            Instance = this;
            Init();
        }

        private void Init()
        {
            _onGridCreator = new OnGridCreator(gridParent, tilePrefab, passengerPrefab);
            _onBusCreator = new OnBusCreator(busPrefab, busParent);
        }

        private void Start()
        {
            _materialDictionary = new Dictionary<ColorTypes, Material>
            {
                { ColorTypes.Red, redMaterial },
                { ColorTypes.Blue, blueMaterial },
                { ColorTypes.Green, greenMaterial },
                { ColorTypes.Yellow, yellowMaterial },
                { ColorTypes.Purple, purpleMaterial },
                { ColorTypes.Orange, orangeMaterial }
            };
    
            _onGridCreator.SetMaterialDictionary(_materialDictionary);
            _onBusCreator.SetMaterialDictionary(_materialDictionary);
            
            if (LevelOrganizer.OnGetLevelEditorData?.Invoke(1) is not null)
                LoadLevel(1);
            else CreateNewLevel();
        }

        public void CreateNewLevel()
        {
            levelID = LevelOrganizer.OnDetectMissingLevel.Invoke();
            
            gridWidth = 3;
            gridLength = 3;
            time = 30;
            cells.Clear();
            buses.Clear();
            
            var newLevelData = new LevelData
            {
                levelID = levelID,
                gridWidth = gridWidth,
                gridLength = gridLength,
                time = time,
                buses = new List<BusArea>(),
                cells = new List<CellArea>()
            };
            
            CreateAndSaveLevelAsset(newLevelData, () => LevelOrganizer.OnCreateLevels?.Invoke());
            CreateGrid(gridWidth, gridLength, newLevelData);
            UpdateLevelUI(levelID, $"Level {levelID} created!");
            SaveLevel();
        }

        public void LoadLevel(int id)
        {
            var data = LevelOrganizer.OnGetLevelEditorData?.Invoke(id);
            if (data != null)
            {
                levelID = data.levelID;
                gridWidth = data.gridWidth;
                gridLength = data.gridLength;
                time = data.time;
                buses = new List<BusArea>(data.buses);
                cells = new List<CellArea>(data.cells);
                CreateGrid(gridWidth, gridLength, data);
                UpdateLevelUI(levelID, $"Level {levelID} loaded!");
            }
            else
            {
                Debug.LogWarning($"Level not found!");
            }
        }

        public void DeleteLevel()
        {
            if (LevelOrganizer.OnGetTotalLevels?.Invoke() == 1)
            {
                levelIDText.GetComponent<TextMeshProUGUI>().CreateAndFadeOut(levelIDText.transform.parent,"You can't delete the last level!", 0.69f);
                return;
            }
            
            var path = $"Assets/Resources/Datas/Levels/{LevelFileName}{levelID}.asset";
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
            
            LevelOrganizer.OnCreateLevels?.Invoke();
            
            UpdateLevelUI(levelID, $"Level {levelID} deleted!");
        }

        public void SaveLevel()
        {
            var data = LevelOrganizer.OnGetLevelEditorData?.Invoke(levelID);

            // Check if any value has changed
            if (data != null &&
                levelID == data.levelID &&
                gridWidth == data.gridWidth &&
                gridLength == data.gridLength &&
                Mathf.Approximately(time, data.time) &&
                AreListsEqual(buses, data.buses) &&
                AreListsEqual(cells, data.cells)) 
            {
                levelIDText.CreateAndFadeOut(levelIDText.transform.parent,$"No value has changed!", 0.69f);
                return; 
            }
            
            if(_colorCounts.Count is not 0)
                foreach (var color in _colorCounts.Where(color => color.Value % 3 != 0))
                {
                    levelIDText.CreateAndFadeOut(levelIDText.transform.parent,$"ColorType: {color.Key} needs {3 - color.Value % 3} more to complete a bus.", 0.69f);
                    return;
                }
            
            var currentLevelData = new LevelData
            {
                levelID = levelID,
                gridWidth = gridWidth,
                gridLength = gridLength,
                time = time,
                buses = new List<BusArea>(buses),
                cells = new List<CellArea>(cells)
            };

            CreateAndSaveLevelAsset(currentLevelData, () => LevelOrganizer.OnCreateLevels?.Invoke());
            CreateGrid(gridWidth, gridLength, currentLevelData);
            UpdateLevelUI(levelID, $"Level {levelID} saved!");
        }

        private bool AreListsEqual<T>(List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            for (var i = 0; i < list1.Count; i++)
            {
                switch (list1[i])
                {
                    case CellArea cellArea1 when list2[i] is CellArea cellArea2 && !AreCellAreasEqual(cellArea1, cellArea2):
                    case BusArea busArea1 when list2[i] is BusArea busArea2 && !AreBusAreasEqual(busArea1, busArea2):
                        return false;
                    default:
                    {
                        if (!EqualityComparer<T>.Default.Equals(list1[i], list2[i]))
                        {
                            return false;
                        }

                        break;
                    }
                }
            }
            return true;
        }

        private bool AreCellAreasEqual(CellArea cellArea1, CellArea cellArea2)
        {
            return cellArea1.position == cellArea2.position &&
                   cellArea1.gridTypes == cellArea2.gridTypes &&
                   cellArea1.colorTypes == cellArea2.colorTypes;
        }

        private bool AreBusAreasEqual(BusArea busArea1, BusArea busArea2)
        {
            return busArea1.colorTypes == busArea2.colorTypes &&
                   busArea1.color == busArea2.color;
        }
        
        private void CreateAndSaveLevelAsset(LevelData levelData, Action callback)
        {
            var levelAsset = ScriptableObject.CreateInstance<CD_Level>();
            levelAsset.Data = levelData;
            AssetDatabase.CreateAsset(levelAsset, $"Assets/Resources/Datas/Levels/{LevelFileName}{levelData.levelID}.asset");
            AssetDatabase.SaveAssets();

            callback?.Invoke();
        }

        private void UpdateLevelUI(int id, string message)
        {
            OnValueChanged?.Invoke(id);
            levelIDText.text = $"Current Level ID: {id}";
            levelIDText.GetComponent<TextMeshProUGUI>().CreateAndFadeOut(levelIDText.transform.parent, message, 0.69f);
        }

        public void CreateGrid(byte width, byte length, LevelData data)
        {
            ClearGrid();
            ClearBuses();
            
            cells = _onGridCreator.CreateGrid(width, length, data.cells);
            buses = _onBusCreator.CreateBuses(data.cells);

            LogIncompleteColorTypes(data.cells);
        }
        
        private void LogIncompleteColorTypes(List<CellArea> cellAreas)
        {
            _colorCounts = new Dictionary<ColorTypes, int>();
            if(_colorCounts.Count is not 0)
                _colorCounts.Clear();
            passengerIssuesText.text = "";
            foreach (var cell in cellAreas.Where(cell => cell.colorTypes != ColorTypes.None))
            {
                _colorCounts.TryAdd(cell.colorTypes, 0);
                _colorCounts[cell.colorTypes]++;
            }

            foreach (var colorCount in _colorCounts.Where(colorCount => colorCount.Value % 3 != 0))
            {
                if(colorCount.Value % 3 != 0)
                    passengerIssuesText.text = passengerIssuesText.text + "\n" + $"{3 - colorCount.Value % 3} more {colorCount.Key}";
            }
        }
        
        public void UpdateCellDatas(CellArea cell)
        {
            var index = cells.FindIndex(c => c.position == cell.position);
            cells[index].gridTypes = cell.gridTypes;
            cells[index].colorTypes = cell.colorTypes;
            
            var currentLevelData = new LevelData
            {
                levelID = levelID,
                gridWidth = gridWidth,
                gridLength = gridLength,
                time = time,
                buses = new List<BusArea>(buses),
                cells = new List<CellArea>(cells)
            };
            
            CreateGrid(gridWidth, gridLength, currentLevelData);
        }

        public int GetTotalPassengerCount()
        {
            return cells.Count(cell => cell.colorTypes != ColorTypes.None);
        }
        
        private void ClearGrid()
        {
            for(var i = 0; i < gridParent.childCount; i++)
            {
                Destroy(gridParent.GetChild(i).gameObject);
            }
            
            cells.Clear();
        }
        
        private void ClearBuses()
        {
            for(var i = 0; i < busParent.childCount; i++)
            {
                Destroy(busParent.GetChild(i).gameObject);
            }
            
            buses.Clear();
            
            _onBusCreator.ResetSpawnPositions();
        }
    }
}