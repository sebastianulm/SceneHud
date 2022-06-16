using System.Collections.Generic;
using UnityEngine;

namespace net.thewired.SceneHud.Hotkeys
{
    public interface IHotKey
    {
        public IEnumerable<KeyCode> KeyCode { get; }
        public bool OnHotkey(Event evt);
    }
}