using System;
using UnityEngine.Events;

namespace Runtime.Signals
{
    public static class ScoreSignals
    {
        public static UnityAction<int> OnSetScore = delegate { };
        public static UnityAction<int> OnSendFinalScore = delegate { };
        public static UnityAction<int> OnSendMoney = delegate { };
        
        public static Func<int> OnGetMoney = delegate { return 0; };
    }
}