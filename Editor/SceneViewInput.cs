using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace net.thewired.SceneHud
{
    public class SceneViewInput
    {
        private VisualElement root;
        public SceneViewInput(VisualElement root)
        {
            while (root.parent != null)
            {
                root = root.parent;
            }
            
            root.RegisterCallback<WheelEvent>(HandleWheel, TrickleDown.TrickleDown);
            root.RegisterCallback<ClickEvent>(HandleClick);
            root.RegisterCallback<MouseMoveEvent>(HandleMouseMove);
            this.root = root;
        }
        private void HandleWheel(WheelEvent wheel)
        {
            Debug.Log("Wheel event: " + wheel.mousePosition + " " + wheel.delta);
        }
        private void HandleMouseMove(MouseMoveEvent evt)
        {
            var targetVis = evt.target as VisualElement;
            if (targetVis == null || targetVis.name != "unity-scene-view-camera-rect")
            {
                return;
            }
            var result = FindSelectable(evt.mousePosition, targetVis);
            foreach (var o in SceneHud.Selectables)
            {
                o.HidePreview();
            }
            if (result.Item2 == null)
            {
                return;
            }
            result.Item3.DrawPreview(result.Item2, result.Item4, SceneHudBar.Content, SceneHudBar.Selected);
        }
        
        private void HandleClick(ClickEvent clickEvent)
        {
            var targetVis = clickEvent.target as VisualElement;
            if (targetVis == null || targetVis.name != "unity-scene-view-camera-rect")
            {
                return;
            }
            var result = FindSelectable(clickEvent.position, targetVis);
            foreach (var o in SceneHud.Selectables)
            {
                o.HidePreview();
            }
            if (result.Item2 == null)
            {
                return;
            }
            result.Item3.DrawPreview(result.Item2, result.Item4, SceneHudBar.Content, SceneHudBar.Selected);
            result.Item3.Click(result.Item4, SceneHudBar.Content, SceneHudBar.Selected);
        }
        private (float, GameObject, IRaycastSelectable, object) FindSelectable(Vector3 pos, VisualElement element)
        {
            pos.x -= element.worldBound.position.x;
            pos.y -= element.worldBound.position.y;
            pos.x /= element.worldBound.width;
            pos.y /= element.worldBound.height;
            var results = new List<(float, GameObject, IRaycastSelectable, object)>();
            var cams = SceneView.GetAllSceneCameras();
            foreach (var camera in cams)
            {
                var sceenSize = camera.pixelRect;
                var mousePos = new Vector3
                {
                    x = pos.x * sceenSize.width,
                    y = (1- pos.y) * sceenSize.height
                };
                var ray = camera.ScreenPointToRay(mousePos);
                foreach (var o in SceneHud.Selectables)
                {
                    try
                    {
                        if (o.Cast(ray, mousePos, out float distance, out var selectedObjet, out object context))
                        {
                            results.Add((distance, selectedObjet, o, context));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
            static int SortByDistance((float, GameObject, IRaycastSelectable, object) x, (float, GameObject, IRaycastSelectable, object) y)
            {
                return x.Item1.CompareTo(y.Item1);
            }
            results.Sort(SortByDistance);
            var result = results.FirstOrDefault();
            return result;
        }
    }
}