using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using UnityEngine;

namespace Runtime.Controllers.Objects
{
    public class TileController : MonoBehaviour
    {
        #region Self Variables
        
        #region Public  Variables
        
        public CellArea cellData;
        public bool IsClickable { get; set; }
        
        #endregion
        
        #region Serialized Variables
        
        //
        
        #endregion
        
        #region Private Variables
        
        private Renderer _renderer;
        
        #endregion
        
        #endregion
        
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

        private void UpdateAppearance()
        {
            _renderer.material.color = cellData.gridType == GridTypes.Normal ? Color.white 
                : Color.Lerp(Color.black, Color.gray, 0.5f);
            
            // TODO: Material color will be load from material dictionary on ColorData
        }
    }
}