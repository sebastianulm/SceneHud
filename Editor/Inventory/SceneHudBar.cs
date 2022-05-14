using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace net.thewired.SceneHud
{
    [Overlay(typeof(SceneView), visualID, "Prefabs")]
    public class SceneHudBar : Overlay
    {
        public const string visualID = "scene-hud.button-bar.root";
        private VisualElement root;
        private VisualElement barContainer;
        private int selected;
        private IBarContent currentContent;
        public override void OnCreated()
        {
            IBarContent.Add += (b) =>
            {
                currentContent = b;
                root.schedule.Execute(() => BuildButtons(b));
            };
        }
        public override void OnWillBeDestroyed()
        {
        }
        public override VisualElement CreatePanelContent()
        {
            Debug.Log("Panel Content");
            root = new VisualElement()
            {
                name = visualID,
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            barContainer = new VisualElement()
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };
            root.Add(barContainer);
            root.schedule.Execute(() =>
            {
                var bar = IBarContent.All.FirstOrDefault();
                if (bar != null)
                {
                    BuildButtons(bar);
                }
            });
            return root;
        }
        private void BuildButtons(IBarContent bar)
        {
            barContainer.Clear();
            for (var i = 0; i < bar.Length; i++)
            {
                var button = new Button()
                {
                    style =
                    {
                        width = 64,
                        height = 64,
                        unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.LowerLeft),
                        backgroundImage = bar.Icon(i)
                    },
                    text = $"{i+1}",
                    tooltip = bar.Tooltip(i),
                    clickable = new Clickable( OnButtonClicked)
                    {
                        activators = { 
                            new ManipulatorActivationFilter(){clickCount = 1}, 
                            new ManipulatorActivationFilter() {clickCount = 2},
                            new ManipulatorActivationFilter() {modifiers = EventModifiers.Control}
                        }
                    }
                };
                barContainer.Add(button);
            }
        }
        public void Hook(SceneViewInput input)
        {
            input.keyDownListeners.Add(KeyCode.Alpha0, (_) => selected = 9);
            input.keyDownListeners.Add(KeyCode.Alpha1, (_) => selected = 0);
            input.keyDownListeners.Add(KeyCode.Alpha2, (_) => selected = 1);
            input.keyDownListeners.Add(KeyCode.Alpha3, (_) => selected = 2);
            input.keyDownListeners.Add(KeyCode.Alpha4, (_) => selected = 3);
            input.keyDownListeners.Add(KeyCode.Alpha5, (_) => selected = 4);
            input.keyDownListeners.Add(KeyCode.Alpha6, (_) => selected = 5);
            input.keyDownListeners.Add(KeyCode.Alpha7, (_) => selected = 6);
            input.keyDownListeners.Add(KeyCode.Alpha8, (_) => selected = 7);
            input.keyDownListeners.Add(KeyCode.Alpha9, (_) => selected = 8);
        }
        private void OnButtonClicked(EventBase e)
        {
            var button = e.target as Button;
            var buttonNum = button.parent.IndexOf(button);
            if (e.imguiEvent.clickCount == 2)
            {
                EditorGUIUtility.PingObject(currentContent.Get(buttonNum));
            }
            else if (e.imguiEvent.control)
            {
                EditorGUIUtility.PingObject(currentContent.ControlClickTarget(buttonNum));
            }
            else
            {
                selected = buttonNum;
            }
            Debug.Log("clicked on " + e.target + " Now selecting " + buttonNum);
        }
    }
}