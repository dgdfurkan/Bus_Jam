using System;
using System.Collections.Generic;
using Runtime.Controllers.Objects;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Runtime.Extentions;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Signals
{
    public class CoreGameSignals : MonoSingleton<CoreGameSignals>
    {
        public UnityAction<int> OnLevelInitialize = delegate { };
        public UnityAction OnClearActiveLevel = delegate { };
        public Func<int ,LevelData> OnGetLevelData = delegate { return new LevelData(); };
        public Func<LevelData> OnGetCurrentLevelData = delegate { return new LevelData(); };
        public UnityAction<int> OnLoadLevelInitialize = delegate { };
        public Func<int> OnGetTotalPassengerCount = delegate { return 0; };
        public UnityAction OnLevelSuccessful = delegate { };
        public UnityAction OnLevelFailed = delegate { };
        public UnityAction OnNextLevel = delegate { };
        public UnityAction OnRestartLevel = delegate { };
        public UnityAction OnPlay = delegate { };
        public UnityAction OnReset = delegate { };
        public Func<int> OnGetLevelID = delegate { return 0; };
        public UnityAction OnBusFull = delegate { };
        public UnityAction OnBusArrived = delegate { };
        public UnityAction OnFirstInputTaken = delegate { };
        public UnityAction<int> OnSetLevelID = delegate { };
        public Func<bool> OnGetFirstInput = delegate { return false;};
        
        public UnityAction<CellArea> OnUpdateCellArea = delegate { };
        public Func<List<CellArea>> OnGetCellArea = delegate { return new List<CellArea>(); };
        public Func<BusArea, ColorTypes> OnUpdateBusColor = delegate { return ColorTypes.None; };
        public Func<ColorTypes> OnGetCurrentBusColor = delegate { return ColorTypes.None; };
        public Func<BusStopController> OnSendAvailableBusStop = delegate { return null; };
    }
}