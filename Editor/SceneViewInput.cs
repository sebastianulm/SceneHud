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
        public bool isMouseOver;
        public event Func<int, bool> OnScroll;
        public event Func<int, Vector2, bool> OnClick;
        public event Func<int, Vector2, bool> OnMouseDown;
        public event Func<int, Vector2, Vector2, bool> OnMouseUp;
        public event Action<Vector2, Vector2> OnMouseMove;
        public event Func<KeyCode, bool> OnKeyDown;
        public event Func<KeyCode, bool> OnKeyUp;

        public void ProcessEvent(Event evt)
        {
            //Weird Unity incantation to make left mouse work in editor;
            HandleUtility.AddControl(-1, 0);
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
                    if (OnMouseDown != null)
                    {
                        var consume = false;
                        foreach (var @delegate in OnMouseDown.GetInvocationList())
                        {
                            var del = (Func<int, Vector2, bool>)@delegate;
                            consume |= del(Event.current.button, mousePos);
                        }
                        if (consume)
                        {
                            Event.current.Use();
                        }
                    }
                }
                    break;
                case EventType.MouseUp:
                {
                    mousePos = Event.current.mousePosition;
                    mouseState[Event.current.button] = false;
                    var consume = false;
                    if (OnMouseUp != null)
                    {
                        foreach (var @delegate in OnMouseUp.GetInvocationList())
                        {
                            var del = (Func<int, Vector2, bool>)@delegate;
                            consume |= del(Event.current.button, mousePos);
                        }
                    }
                    if (OnClick != null && Vector2.Distance(mouseDownPos[Event.current.button], mousePos) < 2)
                    {
                        foreach (var @delegate in OnClick.GetInvocationList())
                        {
                            var del = (Func<int, Vector2, bool>)@delegate;
                            consume |= del(Event.current.button, mousePos);
                        }
                    }
                    if (consume)
                    {
                        Event.current.Use();
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
                    var consume = false;
                    if (OnKeyDown != null)
                    {
                        foreach (var @delegate in OnKeyDown.GetInvocationList())
                        {
                            var del = (Func<KeyCode, bool>)@delegate;
                            consume |= del(Event.current.keyCode);
                        }
                        if (consume)
                        {
                            Event.current.Use();
                        }
                    }
                }
                    break;
                case EventType.KeyUp:
                {
                    var consume = false;
                    if (OnKeyUp != null)
                    {
                        foreach (var @delegate in OnKeyUp.GetInvocationList())
                        {
                            var del = (Func<KeyCode, bool>)@delegate;
                            consume |= del(Event.current.keyCode);
                        }
                        if (consume)
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