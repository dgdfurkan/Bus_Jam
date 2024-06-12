using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Commands;
using Runtime.Datas.UnityObjects;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Runtime.Extentions;
using Runtime.Signals;
using Sirenix.Utilities;

namespace Runtime.Managers
{
    public class LevelEditorManager : MonoBehaviour
    {
        public static LevelEditorManager Instance;

        public static UnityAction<int> OnValueChanged = delegate { };
        
        [Header("Parent Objects")]
        [Space(10)]
        public Transform gridParent;
        public Transform passengerParent;
        public Transform busParent;
        
        [Header("UI Elements")]
        [Space(10)]
        
        [SerializeField] private Button createLevelButton;
        [SerializeField] private Button deleteLevelButton;
        [SerializeField] private Button saveLevelButton;
        [SerializeField] private TextMeshProUGUI levelIDText;
        [SerializeField] private TextMeshProUGUI passengerIssuesText;
        [SerializeField] private TextMeshProUGUI passengerIssuesMainText;
        
        [Header("Level Data")]
        [Space(10)]
        
        public int levelIDEditor = 1;
        public byte gridWidthEditor = 3;
        public byte gridLengthEditor = 3;
        public float timeEditor = 30;

        [Space(10)]
        public List<BusArea> busesEditor;
        [Space(10)]
        public List<CellArea> cellsEditor;
        [Space(10)]
        
        private const string LevelFileName = "Level_";
        private LevelData _currentLevelData;
        
        private OnGridCreator _onGridCreator;
        private OnBusCreator _onBusCreator;
        private OnGridDestroyer _onGridDestroyer;
        private OnBusDestroyer _onBusDestroyer;

        private Dictionary<ColorTypes, int> _currentColorCounts;
        
        private CD_Color _colorData;
        private Dictionary<ColorTypes, Material> _colorObjectList;        

        private void Awake()
        {
            Instance = this;
            Init();
        }

        private void Init()
        {
            _colorData = GetColorData();
            _onGridCreator = new OnGridCreator(gridParent, passengerParent);
            _onBusCreator = new OnBusCreator(busParent);
            _onGridDestroyer = new OnGridDestroyer(gridParent, passengerParent);
            _onBusDestroyer = new OnBusDestroyer(busParent);
        }
        
        private CD_Color GetColorData()
        {
            return Resources.Load<CD_Color>("Datas/Colors/CD_Color");
        }

        private void OnEnable()
        {
            CoreGameSignals.Instance.OnGetTotalPassengerCount += GetTotalPassengerCount;
        }

        private int GetTotalPassengerCount()
        {
            return cellsEditor.Count(cell => cell.passengerArea.colorType != ColorTypes.None);
        }
        
        private void OnDisable()
        {
            CoreGameSignals.Instance.OnGetTotalPassengerCount -= GetTotalPassengerCount;
        }
        
        private void Start()
        {
            _colorObjectList = new Dictionary<ColorTypes, Material>();
            
            _colorData.ColorList.ForEach(color => _colorObjectList.TryAdd(color.Key, color.Value.material));
    
            _onGridCreator.SetMaterialDictionary(_colorObjectList);
            _onBusCreator.SetMaterialDictionary(_colorObjectList);
            
            LoadLevel(LevelOrganizer.OnDetectMaksimumLevel.Invoke());
        }

        public void CreateNewLevel()
        {
            levelIDEditor = LevelOrganizer.OnDetectMissingLevel.Invoke();
            
            gridWidthEditor = 3;
            gridLengthEditor = 3;
            timeEditor = 30;
            cellsEditor.Clear();
            busesEditor.Clear();
            cellsEditor = new List<CellArea>(_onGridCreator.CreateGrid(gridWidthEditor, gridLengthEditor, new List<CellArea>(9)));
            
            var newLevelData = new LevelData
            {
                levelID = levelIDEditor,
                gridWidth = gridWidthEditor,
                gridLength = gridLengthEditor,
                time = timeEditor,
                buses = new List<BusArea>(busesEditor),
                cells = new List<CellArea>(cellsEditor)
            };
            
            _currentLevelData = newLevelData;
            
            CreateAndSaveLevelAsset(newLevelData);
            CreateGrid(gridWidthEditor, gridLengthEditor, newLevelData);
            UpdateLevelUI(levelIDEditor, $"Level {levelIDEditor} created!");
        }

        public void LoadLevel(int id)
        {
            var data = LevelOrganizer.OnGetLevelEditorData?.Invoke(id);
            if (data is null)
            {
                CreateNewLevel();
                return;
            }

            levelIDEditor = data.levelID;
            gridWidthEditor = data.gridWidth;
            gridLengthEditor = data.gridLength;
            timeEditor = data.time;
            busesEditor = new List<BusArea>(data.buses);
            cellsEditor = new List<CellArea>(data.cells);
            CreateGrid(gridWidthEditor, gridLengthEditor, data);
            UpdateLevelUI(levelIDEditor, $"Level {levelIDEditor} loaded!");
                
            _currentLevelData = data;
        }

        public void DeleteLevel()
        {
            if (LevelOrganizer.OnGetTotalLevels?.Invoke() == 1)
            {
                levelIDText.GetComponent<TextMeshProUGUI>().CreateAndFadeOut(levelIDText.transform.parent,"You can't delete the last level!", 1.89f);
                return;
            }
            
            var path = $"Assets/Resources/Datas/Levels/{LevelFileName}{levelIDEditor}.asset";
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
            
            LevelOrganizer.OnCreateLevels?.Invoke();
            LoadLevel(LevelOrganizer.OnDetectLowerLevel.Invoke(levelIDEditor));
            UpdateLevelUI(levelIDEditor, $"Level {levelIDEditor} deleted!");
        }

        public void SaveLevel()
        {
            var data = LevelOrganizer.OnGetLevelEditorData?.Invoke(levelIDEditor);
            
            var currentData = new LevelData
            {
                levelID = levelIDEditor,
                gridWidth = gridWidthEditor,
                gridLength = gridLengthEditor,
                time = timeEditor,
                buses = new List<BusArea>(busesEditor),
                cells = new List<CellArea>(cellsEditor)
            };
            
            if (data != null)
            {
                if (LevelDataComparer.AreDatasEqual(data, currentData))
                {
                    print("No value has changed!");
                    levelIDText.CreateAndFadeOut(levelIDText.transform.parent,$"No value has changed!", 1.89f, Color.white);
                    return; 
                }
            }
            
            if(_currentColorCounts.Count is not 0)
                foreach (var color in _currentColorCounts.Where(color => color.Value % 3 != 0))
                {
                    levelIDText.CreateAndFadeOut(levelIDText.transform.parent, $"Check Passenger Issues part!", 1.89f,
                        Color.Lerp(Color.red, Color.black, 0.3f));
                    return;
                }
            
            CreateAndSaveLevelAsset(currentData);
            CreateGrid(gridWidthEditor, gridLengthEditor, currentData);
            UpdateLevelUI(levelIDEditor, $"Level {levelIDEditor} saved!");
        }
        
        private void CreateAndSaveLevelAsset(LevelData levelData)
        {
            _currentLevelData = levelData;
            var levelAsset = ScriptableObject.CreateInstance<CD_Level>();
            levelAsset.Data = levelData;
            AssetDatabase.CreateAsset(levelAsset, $"Assets/Resources/Datas/Levels/{LevelFileName}{levelData.levelID}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            LevelOrganizer.OnCreateLevels?.Invoke();
        }

        private void UpdateLevelUI(int id, string message)
        {
            OnValueChanged?.Invoke(id);
            levelIDText.text = $"Current Level ID: {id}";
            levelIDText.GetComponent<TextMeshProUGUI>().CreateAndFadeOut(levelIDText.transform.parent, message, 1.89f);
        }

        public void CreateGrid(byte width, byte length, LevelData data, Action onCellDataChanged = null)
        {
            ClearGrid();
            ClearBuses();
            
            cellsEditor = new List<CellArea>(_onGridCreator.CreateGrid(width, length, data.cells));
            busesEditor = new List<BusArea>(_onBusCreator.CreateBuses(data.cells));

            LogIncompleteColorTypes(data.cells);
            
            onCellDataChanged?.Invoke();
        }
        
        private void LogIncompleteColorTypes(List<CellArea> cellAreas)
        {
            _currentColorCounts = new Dictionary<ColorTypes, int>();
            if(_currentColorCounts.Count is not 0)
                _currentColorCounts.Clear();
            passengerIssuesText.text = "";
            passengerIssuesMainText.enabled = false;
            foreach (var cell in cellAreas.Where(cell => cell.passengerArea.colorType != ColorTypes.None))
            {
                _currentColorCounts.TryAdd(cell.passengerArea.colorType, 0);
                _currentColorCounts[cell.passengerArea.colorType]++;
            }

            foreach (var colorCount in _currentColorCounts.Where(colorCount => colorCount.Value % 3 != 0))
            {
                if (colorCount.Value % 3 == 0) continue;
                passengerIssuesMainText.enabled = true;
                passengerIssuesText.text = passengerIssuesText.text + "\n" + $"{3 - colorCount.Value % 3} more {colorCount.Key}";
            }
        }
        
        public void UpdateCellDatas(CellArea cell)
        {
            var index = cellsEditor.FindIndex(c => c.position == cell.position);
            cellsEditor[index].gridType = cell.gridType;
            cellsEditor[index].passengerArea.colorType = cell.passengerArea.colorType;
            
            var levelData = new LevelData
            {
                levelID = levelIDEditor,
                gridWidth = gridWidthEditor,
                gridLength = gridLengthEditor,
                time = timeEditor,
                buses = new List<BusArea>(busesEditor),
                cells = new List<CellArea>(cellsEditor)
            };
            
            CreateGrid(gridWidthEditor, gridLengthEditor, levelData);
        }
        
        private void ClearGrid()
        {
            _onGridDestroyer.DestroyGridEditor();
            _onGridDestroyer.DestroyPassengerEditor();            
            cellsEditor.Clear();
        }   
        
        private void ClearBuses()
        {
            _onBusDestroyer.DestroyBusEditor();
            
            //busParent.ClearChildren( PoolTypes.BusEditor);
            
            busesEditor.Clear();
            
            _onBusCreator.ResetSpawnPositions();
        }
    }
}