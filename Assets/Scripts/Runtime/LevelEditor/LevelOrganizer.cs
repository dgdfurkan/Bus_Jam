using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Runtime.Datas.UnityObjects;
using Runtime.Datas.ValueObjects;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.Managers
{
    public class LevelOrganizer : MonoBehaviour
    {
        public static Func<int ,LevelData> OnGetLevelEditorData = delegate { return new LevelData(); };
        public static Func<List<LevelData>> OnGetAllLevels = () => new List<LevelData>();
        public static UnityAction OnCreateLevels = delegate { };
        public static Func<int> OnGetTotalLevels  = () => 0;
        public static Func<int> OnDetectMissingLevel  = () => 1;
        public static Func<int> OnDetectMaksimumLevel  = () => 1;
        public static Func<int, int> OnDetectLowerLevel = delegate { return 1; };
        
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private Transform levelParent;
        [SerializeField] private ScrollRect scrollRect;
        
        private const string PathOfData = "Datas/Levels";
        private List<LevelData> _datas = new List<LevelData>();
        private int _totalLevelCount;
        
        private void Awake()
        {
            _datas = GetDatas();
            _totalLevelCount = GetTotalLevelCount();
        }

        private List<LevelData> GetDatas()
        {
            return _datas = new List<LevelData>(Resources.LoadAll<CD_Level>(PathOfData)
                .Select(item => item.GetData())
                .OrderBy(data => data.levelID).ToList());
        }
        
        private int GetTotalLevelCount()
        {
            return _datas.Count;
        }

        private void OnEnable()
        {
            OnGetAllLevels += GetDatas;
            OnGetTotalLevels += GetTotalLevelCount;
            OnGetLevelEditorData += GetLevelData;
            OnCreateLevels += CreateLevels;
            OnDetectMissingLevel += DetectMinimumMissingLevelID;
            OnDetectMaksimumLevel += DetectMaksimumLevel;
            OnDetectLowerLevel += DetectLowerLevelID;
        }
        
        private LevelData GetLevelData(int id)
        {
            return _datas.FirstOrDefault(data => data.levelID == id);
        }

        private void CreateLevels()
        {
            for(var i = 0; i < levelParent.childCount; i++)
            {
                Destroy(levelParent.GetChild(i).gameObject);
            }
            
            _datas = GetDatas();
            _totalLevelCount = GetTotalLevelCount();
            for (var i = 0; i < _totalLevelCount; i++)
            {
                var level = Instantiate(levelPrefab, levelParent);
                var i1 = i;
                level.GetComponent<Button>().onClick.AddListener(() => LevelEditorManager.Instance.LoadLevel(_datas[i1].levelID));
                level.GetComponentsInChildren<TextMeshProUGUI>().First().text = $"Level {_datas[i].levelID}";
            }
            scrollRect.verticalNormalizedPosition = 0;
        }
        
        private int DetectMinimumMissingLevelID()
        {
            if(_datas.Count == 0) return 1;
            var maxLevel = _datas.Max(data => data.levelID);
            if(maxLevel == 0) return 1;
            var allLevels = Enumerable.Range(1, maxLevel).ToList();
            var existingLevels = _datas.Select(data => data.levelID).ToList();

            var missingLevels = allLevels.Except(existingLevels).ToList();

            return missingLevels.Any() ? missingLevels.Min() : maxLevel + 1;
        }
        
        private int DetectMaksimumLevel()
        {
            return _datas.Any() ? _datas.Max(data => data.levelID) : 1;
        }

        private int DetectLowerLevelID(int levelId)
        {
            var lowerLevels = _datas.Where(data => data.levelID < levelId);
            var levelDatas = lowerLevels.ToList();
            return levelDatas.Any() ? levelDatas.Max(data => data.levelID) : 1;
        }

        private void OnDisable()
        {
            OnGetLevelEditorData -= GetLevelData;
            OnGetTotalLevels -= GetTotalLevelCount;
            OnGetAllLevels -= GetDatas;
            OnCreateLevels -= CreateLevels;
            OnDetectMissingLevel -= DetectMinimumMissingLevelID;
            OnDetectMaksimumLevel -= DetectMaksimumLevel;
            OnDetectLowerLevel -= DetectLowerLevelID;
        }

        private void Start()
        {
            CreateLevels();
        }
    }
}

