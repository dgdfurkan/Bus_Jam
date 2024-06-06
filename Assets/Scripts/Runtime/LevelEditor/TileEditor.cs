using Runtime.Enums;
using UnityEngine;

namespace Runtime.LevelEditor
{
    public class TileEditor : MonoBehaviour
    {
        private Renderer _renderer;
        private GridTypes _gridType;
        private ColorTypes _colorType;
        public GridTypes GridType
        {
            get => _gridType;
            set
            {
                _gridType = value;
                UpdateAppearance();
            }
        }

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            GridType = GridTypes.Normal;
            _colorType = ColorTypes.None;
        }

        private void OnMouseDown()
        {
            GridType = GridType == GridTypes.Normal ? GridTypes.Disabled : GridTypes.Normal;
        }

        private void UpdateAppearance()
        {
            _renderer.enabled = GridType == GridTypes.Normal;
        }
    }
}