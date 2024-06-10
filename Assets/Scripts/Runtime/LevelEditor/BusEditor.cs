using Runtime.Enums;
using UnityEngine;

namespace Runtime.Managers
{
    public class BusEditor : MonoBehaviour
    {
        public ColorTypes colorType;
        public void Initialize(ColorTypes type)
        {
            colorType = type;
        }
    }
}