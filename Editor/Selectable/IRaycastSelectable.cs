using UnityEngine;

namespace net.thewired.SceneHud
{
    public interface IRaycastSelectable
    {
        public bool Cast(Ray ray, Vector2 pos, out float distance, out GameObject selectedObject);
    }
}