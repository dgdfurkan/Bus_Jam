using System.Collections.Generic;
using Runtime.Controllers.Objects;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Runtime.Signals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Managers
{
    public class GameManager : MonoBehaviour
    {
        #region Self Variables

        #region Public Variables

        //
        
        #endregion
        
        #region Serialized Variables

        //
        
        #endregion
        
        #region Private Variables
        
        private ColorTypes _currentBusColor;
        [ShowInInspector] private List<CellArea> _currentCells = new List<CellArea>();
        
        #endregion

        #endregion

        protected void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize += OnLevelInitialize;
            CoreGameSignals.Instance.OnGetCurrentBusColor += OnGetCurrentBusColor;
            CoreGameSignals.Instance.OnUpdateBusColor += UpdateBusColor;
            CoreGameSignals.Instance.OnGetCellArea += OnGetCellArea;
            CoreGameSignals.Instance.OnUpdateCellArea += UpdateCellArea;
        }
        
        private void OnLevelInitialize(int levelId)
        {
            _currentCells = CoreGameSignals.Instance.OnGetLevelData(levelId).cells;
        }
        
        private ColorTypes OnGetCurrentBusColor()
        {
            return _currentBusColor;
        }

        private ColorTypes UpdateBusColor(BusArea bus)
        {
            _currentBusColor = bus.colorType;
            return _currentBusColor;
        }
        
        private List<CellArea> OnGetCellArea()
        {
            return _currentCells;
        }
        
        private void UpdateCellArea(CellArea cell)
        {
            var foundCell = _currentCells.Find(x => x.position == cell.position);
            if (foundCell?.passengerArea == null)
            {
                return;
            }
            foundCell.passengerArea.colorType = ColorTypes.None;
        }
        
        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize -= OnLevelInitialize;
            CoreGameSignals.Instance.OnGetCurrentBusColor -= OnGetCurrentBusColor;
            CoreGameSignals.Instance.OnUpdateBusColor -= UpdateBusColor;
            CoreGameSignals.Instance.OnGetCellArea -= OnGetCellArea;
            CoreGameSignals.Instance.OnUpdateCellArea -= UpdateCellArea;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
    }
}