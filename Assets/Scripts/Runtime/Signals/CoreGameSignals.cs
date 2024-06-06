using System;
using System.Collections.Generic;
using Runtime.Datas.ValueObjects;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Signals
{
    public static class CoreGameSignals
    {
        //public static UnityAction<GameStates> OnChangeGameState = delegate { };
        public static UnityAction<int> OnLevelInitialize = delegate { };
        public static UnityAction OnClearActiveLevel = delegate { };
        public static Func<int ,LevelData> OnGetLevelData = delegate { return new LevelData(); };
        public static UnityAction<List<CellArea>, Transform> OnGridInitialize = delegate { };
        public static UnityAction OnLevelSuccessful = delegate { };
        public static UnityAction OnLevelFailed = delegate { };
        public static UnityAction OnNextLevel = delegate { };
        public static UnityAction OnRestartLevel = delegate { };
        public static UnityAction OnPlay = delegate { };
        public static UnityAction OnReset = delegate { };
        public static Func<int> OnGetLevelValue = delegate { return 0; };

        public static UnityAction<int> OnStageAreaSuccessful = delegate { };
        public static UnityAction OnStageAreaEntered = delegate { };
        public static UnityAction OnFinishAreaEntered = delegate { };
        public static UnityAction OnMiniGameAreaEntered = delegate { };
        public static UnityAction OnMultiplierAreaEntered = delegate { };
    }
}