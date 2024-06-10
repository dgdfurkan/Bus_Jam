using Runtime.Enums;
using Runtime.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Managers
{
    public class PassengerEditor : MonoBehaviour, IMovable, IClickable
    {
        private static PassengerEditor _activePassenger;

        public ColorTypes colorType;
        private Vector3 offset;
        public bool IsMoving { get; set; } = false;
        
        public event UnityAction<PassengerEditor> OnClick;

        private void OnMouseUp()
        {
            OnClick?.Invoke(this);
        }
        
        public void Initialize(ColorTypes type)
        {
            colorType = type;
        }

        public void StartMoving(Vector3 position)
        {
            
        }

        public void Move(Vector3 position)
        {
            if (IsMoving) return;
            
            // Objenin kopyasını oluştur ve referansı al
            var currentClone = Instantiate(gameObject);
            
            offset = currentClone.transform.position - position;
            IsMoving = true;

            // Kopyayı hareket ettir
            currentClone.transform.position = position + offset;
        }

        public void StopMoving()
        {
            
        }

        public void Click()
        {
            
        }
    }
}