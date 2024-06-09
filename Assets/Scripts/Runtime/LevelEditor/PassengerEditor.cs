using Runtime.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.LevelEditor
{
    public class PassengerEditor : MonoBehaviour
    {
        private static PassengerEditor _activePassenger;

        public ColorTypes colorType;
        
        private Vector3 _mOffset;
        private float _mZCameraDistance;
        
        public event UnityAction<PassengerEditor> OnClick;

        private void OnMouseUp()
        {
            OnClick?.Invoke(this);
        }
        
        public void Initialize(ColorTypes type)
        {
            colorType = type;
        }
    }
}