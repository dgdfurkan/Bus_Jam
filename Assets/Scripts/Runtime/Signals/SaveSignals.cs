using UnityEngine.Events;

namespace Runtime.Signals
{
    public static class SaveSignals
    {
        public static UnityAction<int> OnSaveGameData = delegate { };
        public static UnityAction OnLoadSaveData = delegate { };
    }
}