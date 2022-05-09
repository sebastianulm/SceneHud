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
        public static SceneHudBar bar = new SceneHudBar();
        static SceneHud()
        {
            SceneView.beforeSceneGui += HudPhase;
            SceneView.duringSceneGui += ScenePhase;
            RegisterSelectable();
        }
        private static void ScenePhase(SceneView sceneView)
        {
            Handles.BeginGUI();
            GUILayout.Label("Selection: " + Selection.activeGameObject?.gameObject?.GetPath());
            Handles.EndGUI();
            bar.Render(sceneView);
        }
        private static void HudPhase(SceneView sceneView)
        { 
            SceneViewInput.ProcessEvent(Event.current);
        }
        private static void RegisterSelectable()
        {
            var interfaceType = typeof(IRaycastSelectable);
            var all = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => Activator.CreateInstance(x))
                .Cast<IRaycastSelectable>();
            Debug.Log("Registered ISelectables: " + string.Join(",", all.Select(x => x.GetType().Name)));
            var results = new List<(float, GameObject)>();
            Vector2 lastClick = Vector2.zero;
            float lastDistance = 0f;
            var comparer = new CompareRaycastTargets();
            void OnClick(int i, Vector2 vector2)
            {
                if (i != 0) return;
                var cams = SceneView.GetAllSceneCameras();
                foreach (var camera in cams)
                {
                    var sceenSize = camera.pixelRect;
                    var mousePos = vector2;
                    mousePos.y = sceenSize.height - mousePos.y;
                    var ray = camera.ScreenPointToRay(mousePos);
                    Debug.DrawLine(camera.transform.position, ray.origin, Color.black, 2);
                    foreach (var o in all)
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
            SceneViewInput.OnClick += OnClick;
        }
    }
}
