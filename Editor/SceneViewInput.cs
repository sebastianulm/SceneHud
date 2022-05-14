using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;

namespace net.thewired.SceneHud
{
    public class SceneViewInput
    {
        private const int MaxPointers = 10;
        public Vector2 mousePos;
        public bool[] mouseState = new bool[MaxPointers];
        public Vector2[] mouseDownPos = new Vector2[MaxPointers];
        public bool consumeKeys;
        public readonly Dictionary<KeyCode, Action<KeyCode>> keyDownListeners = new Dictionary<KeyCode, Action<KeyCode>>();
        public readonly Dictionary<KeyCode, Action<KeyCode>> keyUpListeners = new Dictionary<KeyCode, Action<KeyCode>>();
        public bool isMouseOver;
        public Action<int> OnScroll;
        public Action<int, Vector2> OnClick;
        public Action<int, Vector2> OnMouseDown;
        public Action<int, Vector2, Vector2> OnMouseUp;
        public Action<Vector2, Vector2> OnMouseMove;
        public void ProcessEvent(Event evt)
        {
            //Weird Unity incantation to make left mouse work in editor;
            HandleUtility.AddControl(-1,0);
            ProcessMouseOver();
            ProcessMouse();
            ProcessKeyboard();
            switch (evt.type)
            {
                case EventType.Repaint:
                {
                }
                    break;
            }
        }
        private void ProcessMouseOver()
        {
            switch (Event.current.type)
            {
                case EventType.MouseEnterWindow:
                {
                    isMouseOver = true;
                }
                    break;
                case EventType.MouseLeaveWindow:
                {
                    isMouseOver = false;
                }
                    break;
            }
        }
        private void ProcessMouse()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                {
                    mousePos = Event.current.mousePosition;
                    mouseState[Event.current.button] = true;
                    mouseDownPos[Event.current.button] = mousePos;
                    OnMouseDown?.Invoke(Event.current.button, mousePos);
                    //if (Event.current.button == 2) Event.current.Use();
                    //if (Event.current.button == 0) Event.current.Use();
                }
                    break;
                case EventType.MouseUp:
                {
                    mousePos = Event.current.mousePosition;
                    mouseState[Event.current.button] = false;
                    OnMouseUp?.Invoke(Event.current.button, mousePos, mouseDownPos[Event.current.button]);
                    if (Vector2.Distance(mouseDownPos[Event.current.button], mousePos) < 2)
                    {
                        OnClick(Event.current.button, mousePos);
                    }
                }
                    break;
                case EventType.MouseMove:
                {
                    var lastMousePos = mousePos;
                    mousePos = Event.current.mousePosition;
                    OnMouseMove?.Invoke(lastMousePos, mousePos);
                }
                    break;
                case EventType.ScrollWheel:
                {
                    OnScroll?.Invoke(Mathf.RoundToInt(Event.current.delta.y));
                }
                    break;
            }
        }
        private void ProcessKeyboard()
        {
            switch (Event.current.type)
            {
                case EventType.KeyDown:
                {
                    if (keyDownListeners.TryGetValue(Event.current.keyCode, out var action))
                    {
                        action?.Invoke(Event.current.keyCode);
                        if (consumeKeys)
                        {
                            Event.current.Use();
                        }
                    }
                }
                    break;
                case EventType.KeyUp:
                {
                    if (keyUpListeners.TryGetValue(Event.current.keyCode, out var action))
                    {
                        action?.Invoke(Event.current.keyCode);
                        if (consumeKeys)
                        {
                            Event.current.Use();
                        }
                    }
                }
                    break;
            }
        }
    }
}