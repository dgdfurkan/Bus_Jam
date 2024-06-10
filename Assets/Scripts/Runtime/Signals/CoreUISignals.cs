using System;
using Runtime.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Signals
{
    public static class CoreUISignals
    {
        public static UnityAction<UIPanelTypes, int> OnOpenPanel = delegate { };
        public static UnityAction<int> OnClosePanel = delegate { };
        public static UnityAction OnCloseTopPanel = delegate { };
        public static UnityAction OnCloseAllPanels = delegate { };
        
    }
}