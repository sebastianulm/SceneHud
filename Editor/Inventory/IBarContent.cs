using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace net.thewired.SceneHud
{
    public interface IBarContent
    {
        public GameObject Get(int index);
        public Object ControlClickTarget(int index);
        public Texture2D Icon(int index);
        public string Tooltip(int index);
        public int Length { get; set; }
        public string BarName { get; }
    }
}