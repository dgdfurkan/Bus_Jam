using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Signals
{
    public static class PathfindingSignals
    {
        public static Func<Vector2Int ,bool> OnGetCanMove = delegate { return false; };
        public static Func<Vector2Int ,List<Vector2Int>> OnGetVector2IntPath = delegate { return null; };
        public static Func<Vector3[]> OnGet3DPath = delegate { return null; };
    }
}