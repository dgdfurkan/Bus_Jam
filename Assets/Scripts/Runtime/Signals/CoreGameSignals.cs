﻿using System;
using System.Collections.Generic;
using Runtime.Controllers.Objects;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using UnityEngine.Events;

namespace Runtime.Signals
{
    public static class CoreGameSignals
    {
        //public static UnityAction<GameStates> OnChangeGameState = delegate { };
        public static UnityAction<int> OnLevelInitialize = delegate { };
        public static UnityAction OnClearActiveLevel = delegate { };
        public static Func<int ,LevelData> OnGetLevelData = delegate { return new LevelData(); };
        public static Func<LevelData> OnGetCurrentLevelData = delegate { return new LevelData(); };
        public static UnityAction<int> OnLoadLevelInitialize = delegate { };
        public static Func<int> OnGetTotalPassengerCount = delegate { return 0; };
        public static UnityAction OnLevelSuccessful = delegate { };
        public static UnityAction OnLevelFailed = delegate { };
        public static UnityAction OnNextLevel = delegate { };
        public static UnityAction OnRestartLevel = delegate { };
        public static UnityAction OnPlay = delegate { };
        public static UnityAction OnReset = delegate { };
        public static Func<int> OnGetLevelID = delegate { return 0; };
        public static UnityAction OnBusFull = delegate { };
        public static UnityAction OnBusArrived = delegate { };
        
        public static UnityAction<CellArea> OnUpdateCellArea = delegate { };
        public static Func<List<CellArea>> OnGetCellArea = delegate { return new List<CellArea>(); };
        public static Func<BusArea, ColorTypes> OnUpdateBusColor = delegate { return ColorTypes.None; };
        public static Func<ColorTypes> OnGetCurrentBusColor = delegate { return ColorTypes.None; };
        public static Func<BusStopController> OnSendAvailableBusStop = delegate { return null; };
    }
}