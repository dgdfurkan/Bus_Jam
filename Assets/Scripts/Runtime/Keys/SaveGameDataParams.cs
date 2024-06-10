using System;

namespace Runtime.Keys
{
    [Serializable]
    public struct SaveGameDataParams
    {
        public bool IsFirstStart;
        public int Level;
        public int Heart;
        public float Money;
    }
}
