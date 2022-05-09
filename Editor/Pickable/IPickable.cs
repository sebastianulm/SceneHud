using UnityEngine;

namespace net.thewired.SceneHud
{
    public interface IPickable
    {
        public bool Pick(Ray ray, Vector2 pos, out GameObject selectedObject, out GameObject prefab);
    }
}