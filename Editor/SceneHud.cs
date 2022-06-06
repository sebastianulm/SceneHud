using System;
using System.Collections.Generic;
using System.Linq;
using net.thewired.SceneHud.Hotkeys;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace net.thewired.SceneHud
{
    [InitializeOnLoad]
    public static class SceneHud
    {
        private static Dictionary<VisualElement, SceneViewInput> viewInputs = new();
        private static List<IRaycastSelectable> selectables;
        internal static IReadOnlyList<IRaycastSelectable> Selectables => selectables;
        internal static Dictionary<KeyCode, List<Func<Event, bool>>> KeyboardIntercept = new();
        static SceneHud()
        {
            RegisterSelectable();
            RegisterHotkeys();
            SceneView.beforeSceneGui += HudPhase;
        }
        private static void RegisterSelectable()
        {
            var interfaceType = typeof(IRaycastSelectable);
            selectables = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Where(x => !typeof(MonoBehaviour).IsAssignableFrom(x))
                .Select(x => Activator.CreateInstance(x))
                .Cast<IRaycastSelectable>()
                .ToList();
            Debug.Log("Registered ISelectables: " + string.Join(",", selectables.Select(x => x.GetType().Name)));
        }
        private static void RegisterHotkeys()
        {
            var interfaceType = typeof(IHotKey);
            var hotkeys = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Where(x => !typeof(MonoBehaviour).IsAssignableFrom(x))
                .Select(x => Activator.CreateInstance(x))
                .Cast<IHotKey>()
                .ToList();
            foreach (var hotkey in hotkeys)
            {
               RegisterHotkey(hotkey.KeyCode, hotkey.OnHotkey);
            }
        }
        internal static void RegisterHotkey(KeyCode keyCode, Func<Event, bool> func)
        {
            if (!KeyboardIntercept.TryGetValue(keyCode, out var list))
            {
                list = new List<Func<Event, bool>>();
                KeyboardIntercept[keyCode] = list;
            }
            list.Add(func);
        }
        internal static void RegisterBar(SceneHudBar bar)
        {
            var barRoot = bar.containerWindow.rootVisualElement;
            if (viewInputs.TryGetValue(barRoot, out var input))
            {
                return;
            }
            viewInputs[barRoot] = new SceneViewInput(barRoot);
        }
        private static void HudPhase(SceneView sceneView)
        {
            if (Event.current.isKey)
            {
                if (KeyboardIntercept.TryGetValue(Event.current.keyCode, out var list))
                {
                    foreach (var func in list)
                    {
                        try
                        {
                            if (func(Event.current)) 
                                Event.current.Use();
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
                    }
                }
            }
        }
    }
}