using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace net.thewired.SceneHud
{
    public interface IBarContent
    {
        public static Action<IBarContent> Add;
        public static Action<IBarContent> Remove;
        private static List<IBarContent> all = new List<IBarContent>();
        public static IReadOnlyList<IBarContent> All => all; 
        static IBarContent()
        {
            Add += content => all.Add(content);
            Remove += content => all.Remove(content);
        }
        public GameObject Get(int index);
        public Object ControlClickTarget(int index);
        public Texture2D Icon(int index);
        public string Tooltip(int index);
        public int Length { get; set; }
        public string BarName { get; }
    }
}