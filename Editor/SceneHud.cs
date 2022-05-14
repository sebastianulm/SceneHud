using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace net.thewired.SceneHud
{
    [InitializeOnLoad]
    public static class SceneHud
    {
        public static SceneViewInput input = new SceneViewInput();
        private static List<IRaycastSelectable> selectables;
        static SceneHud()
        {
            SceneView.beforeSceneGui += HudPhase;
            SceneView.duringSceneGui += ScenePhase;
            RegisterSelectable();
        }
        private static void ScenePhase(SceneView sceneView)
        {
            input.consumeKeys = true;
        }
        private static void HudPhase(SceneView sceneView)
        {
            input.ProcessEvent(Event.current);
        }
        private static void RegisterSelectable()
        {
            var interfaceType = typeof(IRaycastSelectable);
            selectables = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => Activator.CreateInstance(x))
                .Cast<IRaycastSelectable>()
                .ToList();
            Debug.Log("Registered ISelectables: " + string.Join(",", selectables.Select(x => x.GetType().Name)));
            input.OnClick += OnClick;
        }
        private static void OnClick(int i, Vector2 vector2)
        {
            if (i != 0) 
                return;
            var results = new List<(float, GameObject)>();
            Vector2 lastClick = Vector2.zero;
            float lastDistance = 0f;
            var comparer = new CompareRaycastTargets();
            var cams = SceneView.GetAllSceneCameras();
            foreach (var camera in cams)
            {
                var sceenSize = camera.pixelRect;
                var mousePos = vector2;
                mousePos.y = sceenSize.height - mousePos.y;
                var ray = camera.ScreenPointToRay(mousePos);
                Debug.DrawLine(camera.transform.position, ray.origin, Color.black, 2);
                foreach (var o in selectables)
                {
                    if (o.Cast(ray, mousePos, out float distance, out var selectedObjet))
                    {
                        results.Add((distance, selectedObjet));
                    }
                }
            }
            static int SortByDistance((float, GameObject) x, (float, GameObject) y)
            {
                return x.Item1.CompareTo(y.Item1);
            }
            results.Sort(SortByDistance);
            if (lastClick == vector2)
            {
                var result = results.BinarySearch((lastDistance, null), comparer);
                if (result >= 0)
                {
                    result = (result + 1) % results.Count;
                    var selected = results[result];
                    Selection.activeGameObject = selected.Item2.gameObject;
                    lastDistance = selected.Item1;
                }
            }
            else
            {
                var selected = results.FirstOrDefault();
                Selection.activeGameObject = selected.Item2 != null ? selected.Item2.gameObject : null;
                lastDistance = selected.Item1;
            }
            results.Clear();
        }
    }
}