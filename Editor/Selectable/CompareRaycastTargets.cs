using System.Collections.Generic;
using UnityEngine;

namespace net.thewired.SceneHud
{
    public class CompareRaycastTargets : IComparer<(float, GameObject)>
    {
        public int Compare((float, GameObject) x, (float, GameObject) y)
        {
            return x.Item1.CompareTo(y.Item1);
        }
    }
}