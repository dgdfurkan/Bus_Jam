using UnityEngine;

namespace Runtime.Interfaces
{
    public interface IMovable
    {
        void StartMoving(Vector3 position);
        void Move(Vector3 position);
        void StopMoving();
    }
}