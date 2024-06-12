using System.Collections.Generic;
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
            CoreGameSignals.OnLevelInitialize += OnLevelInitialize;
            CoreGameSignals.OnGetCurrentBusColor += OnGetCurrentBusColor;
            CoreGameSignals.OnUpdateBusColor += OnUpdateBusColor;
            CoreGameSignals.OnGetCellArea += OnGetCellArea;
            CoreGameSignals.OnUpdateCellArea += OnUpdateCellArea;
        }
        
        private void OnLevelInitialize(int levelId)
        {
            _currentCells = CoreGameSignals.OnGetLevelData(levelId).cells;
        }
        
        private ColorTypes OnGetCurrentBusColor()
        {
            return _currentBusColor;
        }

        private ColorTypes OnUpdateBusColor(BusArea bus)
        {
            _currentBusColor = bus.colorType;
            return _currentBusColor;
        }
        
        private List<CellArea> OnGetCellArea()
        {
            return _currentCells;
        }
        
        private void OnUpdateCellArea(CellArea cell)
        {
            //_currentCells.Find(x => x.position == cell.position).passengerArea.colorType = ColorTypes.None;
            print("Update Cell Area");
            var foundCell = _currentCells.Find(x => x.position == cell.position);
            if (foundCell == null)
            {
                print("No CellArea found with the given position");
                return;
            }
            if (foundCell.passengerArea == null)
            {
                print("No PassengerArea found in the CellArea");
                return;
            }
            foundCell.passengerArea.colorType = ColorTypes.None;
            print("ColorType is set to None");
        }
        
        private void UnSubscribeEvents()
        {
            CoreGameSignals.OnLevelInitialize -= OnLevelInitialize;
            CoreGameSignals.OnGetCurrentBusColor -= OnGetCurrentBusColor;
            CoreGameSignals.OnUpdateBusColor -= OnUpdateBusColor;
            CoreGameSignals.OnGetCellArea -= OnGetCellArea;
            CoreGameSignals.OnUpdateCellArea -= OnUpdateCellArea;
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
    }
}