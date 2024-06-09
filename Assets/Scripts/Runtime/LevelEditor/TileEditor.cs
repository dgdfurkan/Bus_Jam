using _Modules.ObjectPooling.Scripts.Enums;
using _Modules.ObjectPooling.Scripts.Signals;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.LevelEditor
{
    public class TileEditor : MonoBehaviour
    {
        private Renderer _renderer;
        public CellArea cellData;
        
        private void Awake()
        {
            GetReferences();
        }

        private void GetReferences()
        {
            _renderer = GetComponent<Renderer>();
        }

        public void Initialize(CellArea data)
        {
            cellData = data;
            UpdateAppearance();
        }

        private void OnMouseDown()
        {
            if(PassengerEditorManager.Instance.selectedPassengerEditor is not null) return;
            
            cellData.gridType = cellData.gridType == GridTypes.Normal ? GridTypes.Disabled : GridTypes.Normal;
            UpdateAppearance();
            if (cellData.gridType == GridTypes.Disabled)
            {
                foreach (Transform child in PassengerEditorManager.Instance.passengerParent)
                {
                    if (child.position != transform.position) continue;
                    PoolSignals.OnSetPooledGameObject?.Invoke(child.gameObject, PoolTypes.PassengerEditor);
                    //Destroy(child.gameObject);
                    break;
                }
                cellData.passengerArea.colorType = ColorTypes.None;
            }
            LevelEditorManager.Instance.UpdateCellDatas(cellData);
        }

        private void UpdateAppearance()
        {
            _renderer.material.color = cellData.gridType == GridTypes.Normal ? Color.white 
                : Color.Lerp(Color.black, Color.gray, 0.5f);
        }
    }
}