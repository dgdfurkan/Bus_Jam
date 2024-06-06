using System.Collections.Generic;
using Runtime.Datas.UnityObjects;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Runtime.LevelEditor
{
    public class LevelEditorManager : MonoBehaviour
    {
        public static LevelEditorManager Instance;
        
        public string levelFileName = "Level_";
        public GameObject tilePrefab;
        public GameObject passengerPrefab;
        public Transform gridParent;

        public int levelID;
        public byte gridWidth = 3;
        public byte gridLength = 3;
        public float time;

        public List<BusArea> busses = new List<BusArea>();
        public List<CellArea> cells = new List<CellArea>();

        private List<GameObject> gridTiles = new List<GameObject>();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            CreateGrid(gridWidth, gridLength);
        }

        public void CreateLevel()
        {
            var newLevelData = new LevelData
            {
                levelID = levelID,
                gridWidth = gridWidth,
                gridLength = gridLength,
                time = time,
                busses = new List<BusArea>(busses),
                cells = new List<CellArea>(cells)
            };

            var levelAsset = ScriptableObject.CreateInstance<CD_Level>();
            levelAsset.Data = newLevelData;

            AssetDatabase.CreateAsset(levelAsset, $"Assets/Resources/Datas/Levels/{levelFileName}{levelID}.asset");
            AssetDatabase.SaveAssets();
        }

        public void LoadLevel(LevelData data)
        {
            if (data != null)
            {
                levelID = data.levelID;
                gridWidth = data.gridWidth;
                gridLength = data.gridLength;
                time = data.time;
                busses = new List<BusArea>(data.busses);
                cells = new List<CellArea>(data.cells);
                CreateGrid(gridWidth, gridLength);
                PopulateGrid();
            }
            else
            {
                Debug.LogWarning($"Level not found!");
            }
        }

        public void DeleteLevel(int id)
        {
            var path = $"Assets/Resources/Datas/Levels/{levelFileName}{id}.asset";
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
        }

        [Button("Create Grid")]
        private void CreateGrid(int width, int length)
        {
            ClearGrid();

            const float gridSpacing = 0.1f; // Gridler arası boşluk

            for (var x = 0; x < width; x++)
            {
                var totalWidth = width * (1 + gridSpacing) - gridSpacing; // Toplam grid genişliği
                var xOffset = -totalWidth / 2 + .5f; // Düzeltilmiş xOffset hesabı, x pozisyonu 0.5f'ten başlıyor

                for (var z = 0; z < length; z++)
                {
                    var totalLength = length * (1 + gridSpacing) - gridSpacing; // Toplam grid uzunluğu
                    var zOffset = -totalLength / 2 + 0.5f; // z ekseninde ortalama için offset

                    var position = new Vector3(x * (1 + gridSpacing), 0, z * (1 + gridSpacing) + zOffset);
                    var tile = Instantiate(tilePrefab, position, Quaternion.identity, gridParent);
                    gridTiles.Add(tile);
                    var cell = new CellArea
                    {
                        position = new Vector2Int(x, z), 
                        gridTypes = GridTypes.Normal,
                        colorTypes = ColorTypes.None
                    };
                    cells.Add(cell);
                }
            }
        }

        private void ClearGrid()
        {
            foreach (var tile in gridTiles)
            {
                DestroyImmediate(tile);
            }
            gridTiles.Clear();
            cells.Clear();
        }

        private void PopulateGrid()
        {
            foreach (var cell in cells)
            {
                var position = new Vector3(cell.position.x, 0, cell.position.y);
                var tile = Instantiate(tilePrefab, position, Quaternion.identity, gridParent);
                gridTiles.Add(tile);
            }
        }
    }
}