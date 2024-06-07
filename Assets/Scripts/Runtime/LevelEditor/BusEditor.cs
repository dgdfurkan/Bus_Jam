using Runtime.Enums;
using UnityEngine;

namespace Runtime.LevelEditor
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