using UnityEngine;

namespace net.thewired.SceneHud
{
    public interface IRaycastSelectable
    {
        public bool Cast(Ray ray, Vector2 pos, out float distance, out GameObject selectedObject, out object context);
        public void Click(object context, IBarContent barContent, int barSelected);
        public void DrawPreview(GameObject selected, object context, IBarContent barContent, int barSelected);
        public void HidePreview();
    }
}