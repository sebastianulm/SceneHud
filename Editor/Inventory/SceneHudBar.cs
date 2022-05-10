using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace net.thewired.SceneHud
{
    public class SceneHudBar
    {
        private GameObject[] templateObj = new GameObject[9];
        private Texture2D[] previews = new Texture2D[9];
        private Texture2D empty;
        public Vector2 buttonSize = new Vector2(50, 50);
        public RectOffset buttonBorder = new RectOffset(2, 2, 2, 2);
        public int selected = 3;

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
                var tex = AssetPreview.GetAssetPreview(value);
                if (tex == null)
                {
                    previews[index] = new Texture2D(128,128);
                    previews[index].Fill(Color.red);
                }
                else
                {
                    previews[index] = new Texture2D(tex.width,tex.height, tex.graphicsFormat, tex.mipmapCount,TextureCreationFlags.None);
                    Graphics.CopyTexture(tex, previews[index]);
                }
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
            
            var style = new GUIStyle()
            {
                fixedWidth = buttonSize.x ,
                fixedHeight = buttonSize.y,
                padding = buttonBorder,
            };
            
            GUIStyle gsTest = new GUIStyle();
            gsTest.normal.background = Texture2D.whiteTexture;
            Handles.BeginGUI();
            GUILayout.BeginArea(rect, gsTest);
            GUILayout.BeginHorizontal();
            // Draw buttons for inventory items.
            for (var index = 0; index < templateObj.Length; index++)
            {
               DrawItem(index);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            
            Handles.EndGUI();

        }
        private void DrawItem(int index)
        {
            if (selected == index)
            {
                var lol = GUI.skin.GetStyle("HelpBox");
                var style = new GUIStyle()
                {
                    fixedWidth = buttonSize.x,
                    fixedHeight = buttonSize.y,
                    padding = buttonBorder,
                    margin = buttonBorder,
                    border = buttonBorder,
                    onNormal = new GUIStyleState()
                    {
                        background = Texture2D.redTexture,
                    }
                };
                GUILayout.Button(previews[index], style);
            }
            else
            {
                var style = new GUIStyle()
                {
                    fixedWidth = buttonSize.x ,
                    fixedHeight = buttonSize.y,
                    padding = buttonBorder,
                };
                GUILayout.Button(previews[index], style);
            }
        }
    }
}