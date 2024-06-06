using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Runtime.Datas.UnityObjects;
using Runtime.Datas.ValueObjects;
using TMPro;
using UnityEngine.UI;

namespace Runtime.LevelEditor
{
    public class LevelOrganizer : MonoBehaviour
    {
        public static Func<int ,LevelData> OnGetLevelEditorData = delegate { return new LevelData(); };
        
        [SerializeField] private GameObject levelPrefab;
        [SerializeField] private Transform levelParent;
        
        private const string PathOfData = "Datas/Levels";
        private List<LevelData> _datas = new List<LevelData>();
        private int _totalLevelCount;
        
        private void Awake()
        {
            _datas = GetDatas();
            _totalLevelCount = GetTotalLevelCount();
        }

        private void OnEnable()
        {
            OnGetLevelEditorData += id => _datas[id];
        }

        private void OnDisable()
        {
            OnGetLevelEditorData -= id => _datas[id];
        }

        private List<LevelData> GetDatas()
        {
            return _datas = new List<LevelData>(Resources.LoadAll<CD_Level>(PathOfData)
                .Select(item => item.GetData())
                .ToList());
        }
        
        private int GetTotalLevelCount()
        {
            return _datas.Count;
        }

        private void Start()
        {
            CreateLevels();
        }

        private void CreateLevels()
        {
            for (var i = 0; i < _totalLevelCount; i++)
            {
                var level = Instantiate(levelPrefab, levelParent);
                //level.GetComponent<CD_Level>().GetData(_datas[i]);
                level.GetComponent<Button>().onClick.AddListener(() => LevelEditorManager.Instance.LoadLevel(_datas[i]));
                level.GetComponentsInChildren<TextMeshProUGUI>().First().text = $"Level {_datas[i].levelID}";
                level.GetComponentsInChildren<TextMeshProUGUI>().Last().text = $""; // TODO: Level name or description
            }
        }
    }
}

