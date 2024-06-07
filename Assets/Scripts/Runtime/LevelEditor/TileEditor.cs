using System.Linq;
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

        public void Initialize(CellArea cellData)
        {
            this.cellData = cellData;
            UpdateAppearance();
        }

        private void OnMouseDown()
        {
            cellData.gridTypes = cellData.gridTypes == GridTypes.Normal ? GridTypes.Disabled : GridTypes.Normal;
            LevelEditorManager.Instance.UpdateCellDatas(cellData);
            UpdateAppearance();
            if (cellData.gridTypes != GridTypes.Disabled) return;
            if (transform.childCount > 0) 
                Destroy(transform.GetChild(0).gameObject);
            cellData.colorTypes = ColorTypes.None;
        }

        private void UpdateAppearance()
        {
            _renderer.material.color = cellData.gridTypes == GridTypes.Normal ? Color.white : Color.grey;
        }
    }
}