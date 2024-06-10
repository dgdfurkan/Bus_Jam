using System.Collections.Generic;
using Runtime.Datas.ValueObjects;
using UnityEngine;

namespace Runtime.Managers
{
    public static class LevelDataComparer
    {
        public static bool AreDatasEqual(LevelData data1, LevelData data2)
        {
            return data1.levelID == data2.levelID &&
                   data1.gridWidth == data2.gridWidth &&
                   data1.gridLength == data2.gridLength &&
                   Mathf.Approximately(data1.time, data2.time) &&
                   AreListsEqual(data1.buses, data2.buses) &&
                   AreListsEqual(data1.cells, data2.cells);
        }

        private static bool AreListsEqual<T>(List<T> list1, List<T> list2)
        {
            if (list1.Count != list2.Count)
            {
                Debug.Log($"List counts not equal: {list1.Count} != {list2.Count}");
                return false;
            }

            for (var i = 0; i < list1.Count; i++)
            {
                switch (list1[i])
                {
                    case CellArea cellArea1 when list2[i] is CellArea cellArea2 && !AreCellAreasEqual(cellArea1, cellArea2):
                    case BusArea busArea1 when list2[i] is BusArea busArea2 && !AreBusAreasEqual(busArea1, busArea2):
                        return false;
                }
            }
            return true;
        }

        private static bool AreCellAreasEqual(CellArea cellArea1, CellArea cellArea2)
        {
            if (!Mathf.Approximately(cellArea1.position.x, cellArea2.position.x))
            {
                Debug.Log($"Cell positions X not equal: {cellArea1.position.x} != {cellArea2.position.x}");
                return false;
            }
            if (!Mathf.Approximately(cellArea1.position.y, cellArea2.position.y))
            {
                Debug.Log($"Cell positions Y not equal: {cellArea1.position.y} != {cellArea2.position.y}");
                return false;
            }
            if (cellArea1.gridType != cellArea2.gridType)
            {
                Debug.Log($"Grid types not equal: {cellArea1.gridType} != {cellArea2.gridType}");
                return false;
            }
            if (cellArea1.passengerArea.colorType != cellArea2.passengerArea.colorType)
            {
                Debug.Log($"Passenger color types not equal: {cellArea1.passengerArea.colorType} != {cellArea2.passengerArea.colorType}");
                return false;
            }
            return true;
        }

        private static bool AreBusAreasEqual(BusArea busArea1, BusArea busArea2)
        {
            if (busArea1.colorType == busArea2.colorType) return true;
            Debug.Log($"Bus color types not equal: {busArea1.colorType} != {busArea2.colorType}");
            return false;
        }
    }
}
