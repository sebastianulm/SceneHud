using UnityEngine;

namespace net.thewired.SceneHud.Hotkeys
{
    public interface IHotKey
    {
        public KeyCode KeyCode { get; }
        public bool OnHotkey(Event evt);
    }
}