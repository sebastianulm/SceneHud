using System.Runtime.InteropServices;
using UnityEngine;

namespace net.thewired.SceneHud
{
    public static class GameObjectExtensions
    {
        public static string GetPath(this GameObject obj)
        {
            if (obj == null)
                return "null";

            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return obj.scene.name + ":" + path;
        }
        public static void JustRender(this GameObject obj, Camera camera, Matrix4x4 matrix, int layer = 0)
        {
            if (obj == null)
                return;
            var properties = new MaterialPropertyBlock();
            var renderers = obj.GetComponentsInChildren<MeshRenderer>();
            foreach (var meshRenderer in renderers)
            {
                var meshFilter = meshRenderer.GetComponent<MeshFilter>();
                for (var index = 0; index < meshRenderer.materials.Length; index++)
                {
                    var mat = meshRenderer.materials[index];
                    Graphics.DrawMesh(meshFilter.sharedMesh, matrix, mat, layer, camera, index, properties);
                }
            }
        }
    }
    public static class Textur2DExtensions
    {
        public static Texture2D Fill(this Texture2D tex, Color color)
        {
            for (var y = 0; y < tex.height; y++)
            {
                for (var x = 0; x < tex.height; x++)
                {
                    tex.SetPixel(x,y, color);
                }
            }
            tex.Apply();
            return tex;
        }
    }
}