using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
                .Where(x => !typeof(MonoBehaviour).IsAssignableFrom(x))
                .Select(x => Activator.CreateInstance(x))
                .Cast<IRaycastSelectable>()
                .ToList();
            Debug.Log("Registered ISelectables: " + string.Join(",", selectables.Select(x => x.GetType().Name)));
            input.OnClick += OnClick;
            input.OnMouseMove += OnMouseMove;
        }
        private static void OnMouseMove(Vector2 from, Vector2 to)
        {
            var results = new List<(float, GameObject, IRaycastSelectable, object)>();
            var cams = SceneView.GetAllSceneCameras();
            foreach (var camera in cams)
            {
                var sceenSize = camera.pixelRect;
                var mousePos = to;
                mousePos.y = sceenSize.height - mousePos.y;
                var ray = camera.ScreenPointToRay(mousePos);
                foreach (var o in selectables)
                {
                    if (o.Cast(ray, mousePos, out float distance, out var selectedObjet, out object context))
                    {
                        results.Add((distance, selectedObjet, o, context));
                    }
                }
            }
            static int SortByDistance((float, GameObject, IRaycastSelectable, object) x, (float, GameObject, IRaycastSelectable, object) y)
            {
                return x.Item1.CompareTo(y.Item1);
            }
            results.Sort(SortByDistance);
            var result = results.FirstOrDefault();
            foreach (var o in selectables)
            {
                o.HidePreview();
            }
            if (result.Item2 == null)
            {
                return;
            }
            result.Item3.DrawPreview(result.Item2, result.Item4, SceneHudBar.Content, SceneHudBar.Selected);
        }
        private static bool OnClick(int i, Vector2 vector2)
        {
            var results = new List<(float, GameObject, IRaycastSelectable, object)>();
            var cams = SceneView.GetAllSceneCameras();
            foreach (var camera in cams)
            {
                var sceenSize = camera.pixelRect;
                var mousePos = vector2;
                mousePos.y = sceenSize.height - mousePos.y;
                var ray = camera.ScreenPointToRay(mousePos);
                foreach (var o in selectables)
                {
                    if (o.Cast(ray, mousePos, out float distance, out var selectedObjet, out object context))
                    {
                        results.Add((distance, selectedObjet, o, context));
                    }
                }
            }
            if (results.Count <= 0)
                return false;
            static int SortByDistance((float, GameObject,IRaycastSelectable, object) x, (float, GameObject,IRaycastSelectable, object) y)
            {
                return x.Item1.CompareTo(y.Item1);
            }
            results.Sort(SortByDistance);
            var place = results.FirstOrDefault();
            place.Item3.Click(i, place.Item4, SceneHudBar.Content, SceneHudBar.Selected );
            results.Clear();
            return true;
        }
    }
}