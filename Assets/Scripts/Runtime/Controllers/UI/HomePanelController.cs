using System;
using DG.Tweening;
using Runtime.Datas.ValueObjects;
using Runtime.Signals;
using TMPro;
using UnityEngine;

namespace Runtime.Controllers.Objects.UI
{
    public class HomePanelController : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables
        
        //

        #region Serialized Variables
        
        [SerializeField] private TextMeshProUGUI levelText;
        #endregion

        #region Self Variables

        private float _levelID;
        
        #endregion
        
        #endregion

        #endregion

        private void OnEnable()
        {
            LevelInitialize();
        }
        
        private void LevelInitialize()
        {
            _levelID = CoreGameSignals.Instance.OnGetLevelID.Invoke() + 1;
            levelText.text = "Level " + _levelID;
        }
    }
}