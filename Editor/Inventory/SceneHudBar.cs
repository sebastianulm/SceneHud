using System;
using UnityEditor;
using UnityEngine;

namespace net.thewired.SceneHud
{
    public class SceneHudBar
    {
        private GameObject[] templateObj = new GameObject[9];
        private Texture2D[] previews = new Texture2D[9];
        private Texture2D empty;
        public Vector2 buttonSize = new Vector2(50,50);
        public RectOffset buttonBorder = new RectOffset(2, 2, 2, 2);

        public SceneHudBar()
        {
            empty = new Texture2D((int)buttonSize.x, (int)buttonSize.y);
        }

        public float BarWidth => templateObj.Length * buttonSize.x;
        public GameObject this[int index]
        {
            get => templateObj[index];
            set
            {
                templateObj[index] = value;
                previews[index] = AssetPreview.GetAssetPreview(value);
            }
        }
        public void Render(SceneView sceneView)
        {
            var rect = new Rect(
                sceneView.position.width / 2 - BarWidth / 2f,
                (sceneView.position.height - buttonSize.y - 40),
                BarWidth, 
                buttonSize.y
            );
            
            GUILayout.BeginArea(rect);
            GUILayout.BeginHorizontal();
            // Draw buttons for inventory items.
            for (var index = 0; index < templateObj.Length; index++)
            {
                DrawItem(index);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        private void DrawItem(int index)
        {
            GameObject item = templateObj[index];
            if (item == null)
            {
                // Just show a blank non-clickable button.
                GUI.enabled = false;
                var style = new GUIStyle()
                {
                    fixedWidth = buttonSize.x,
                    fixedHeight = buttonSize.y,
                    padding = buttonBorder
                };
                item.JustRender(Camera.current, Matrix4x4.identity);
                GUILayout.Button(empty, style);
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = false;
                var style = new GUIStyle()
                {               
                    fixedWidth = buttonSize.x,
                    fixedHeight = buttonSize.y,
                    padding = buttonBorder
                };
                GUILayout.Button(previews[index], style);
                GUI.enabled = true;
            }
        }
    }
}