using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace net.thewired.SceneHud
{
    [Overlay(typeof(SceneView), visualID, "SceneHudBar")]
    public class SceneHudBar : Overlay
    {
        public const string visualID = "scene-hud.button-bar.root";
        public static Color activeColor = new Color(0.2f, 0.4f, 0.5f);
        public static Color inactiveColor = new Color(0.2f, 0.2f, 0.2f);
        public static SceneHudBar Instance => instance;
        public static IBarContent Content => currentContent;
        public static int Selected => instance.selected;
        private static SceneHudBar instance;
        private VisualElement panel;
        private VisualElement barContainer;
        private int selected;
        private static IBarContent currentContent;
        public void SetBarContent(IBarContent barContent)
        {
            currentContent = barContent;
            if (Instance != null)
            {
               Rebuild();
            }
        }
        public override void OnCreated()
        {
            if (instance != null)
                return;
            instance = this;
            if (currentContent != null)
            {
                Rebuild();
            }
        }
        public override void OnWillBeDestroyed()
        {
            instance = null;
            //SceneHud.input.OnKeyDown -= HandleKey;
        }
        public override VisualElement CreatePanelContent()
        {
            if (instance != this)
            {
                return new VisualElement();
            }
            panel = new VisualElement()
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
            panel.Add(barContainer);
            Rebuild();
            return panel;
        }
        private void BuildButtons(IBarContent bar)
        {
            barContainer.Clear();
            for (var i = 0; i < bar?.Length; i++)
            {
                var button = new Button()
                {
                    style =
                    {
                        width = 64,
                        height = 64,
                        borderTopWidth = 5,
                        borderBottomWidth = 5,
                        borderLeftWidth = 5,
                        borderRightWidth = 5,
                        unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.LowerLeft),
                        backgroundImage = bar?.Icon(i)
                    },
                    text = $"{i + 1}",
                    tooltip = bar?.Tooltip(i),
                    clickable = new Clickable(OnButtonClicked)
                    {
                        activators =
                        {
                            new ManipulatorActivationFilter()
                            {
                                clickCount = 1
                            },
                            new ManipulatorActivationFilter()
                            {
                                clickCount = 2
                            },
                            new ManipulatorActivationFilter()
                            {
                                modifiers = EventModifiers.Control
                            }
                        }
                    }
                };
                barContainer.Add(button);
            }
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
                Select(buttonNum);
            }
            Debug.Log("clicked on " + e.target + " Now selecting " + buttonNum);
        }
        private void Select(int index)
        {
            CheckButtonGraphics();
            index = Mathf.Clamp(index, 0, barContainer.childCount - 1);
            selected = Mathf.Clamp(selected, 0, barContainer.childCount - 1);
            barContainer[selected].style.borderBottomColor = inactiveColor;
            barContainer[selected].style.borderLeftColor = inactiveColor;
            barContainer[selected].style.borderRightColor = inactiveColor;
            barContainer[selected].style.borderTopColor = inactiveColor;

            selected = index;

            barContainer[index].style.borderBottomColor = activeColor;
            barContainer[index].style.borderLeftColor = activeColor;
            barContainer[index].style.borderRightColor = activeColor;
            barContainer[index].style.borderTopColor = activeColor;
        }
        private bool HandleKey(Event @event)
        {
            var k = @event.keyCode;
            if (k < KeyCode.Alpha0 || k > KeyCode.Alpha9)
                return false;
            Select(k - KeyCode.Alpha1);
            return true;
        }
        private void CheckButtonGraphics()
        {
            if (currentContent != null)
            {
                for (var i = 0; i < currentContent.Length; i++)
                {
                    var icon = currentContent.Icon(i);
                    if (icon != null)
                    {
                        barContainer[i].style.backgroundImage = icon;
                    }
                }
            }
        }
        private void RegisterHotkeys()
        {
            for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9; i++)
            {
                SceneHud.RegisterHotkey((KeyCode)i , HandleKey);
            }
        }
        public void Rebuild()
        {
            panel?.schedule.Execute(() =>
            {
                BuildButtons(currentContent);
                SceneHud.RegisterBar(this);
                RegisterHotkeys();
            });
        }
    }
}