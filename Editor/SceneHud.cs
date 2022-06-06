using System;
using System.Collections.Generic;
using System.Linq;
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
        static SceneHud()
        {
            RegisterSelectable();
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
                Debug.Log("Is Key event: " + Event.current.keyCode);
            }
        }
    }
}