using UnityEditor;
using UnityEngine;

namespace net.thewired.SceneHud
{
    public class PhysicsRaycast : IRaycastSelectable
    {
        public static QueryTriggerInteraction Interaction = QueryTriggerInteraction.Collide;
        public static GameObject currentGameObject;
        public bool Cast(Ray ray, Vector2 pos, out float distance, out GameObject selectedObject, out object context)
        {
            if (Physics.Raycast(ray, out var hitInfo, 1000, -1, Interaction))
            {
                selectedObject = hitInfo.collider.gameObject;
                distance = hitInfo.distance;
                context = new HitInfoHolder() { hit = hitInfo };
                return true;
            }
            distance = float.MaxValue;
            selectedObject = null;
            context = null;
            return false;
        }
        public void Click(object context, IBarContent barContent, int barSelected )
        {
            var hitInfo = ((HitInfoHolder)context).hit;
            Object.Instantiate(barContent.Get(barSelected), hitInfo.point, Quaternion.AngleAxis(0, hitInfo.normal),  hitInfo.collider.transform);
        }
        public void DrawPreview(GameObject selected, object context, IBarContent barContent, int barSelected)
        {
            var hitInfoHolder = context as HitInfoHolder;
            if (hitInfoHolder == null) return;
            var prefab = barContent.Get(barSelected);
            if (currentGameObject == null)
            {
                currentGameObject = Object.Instantiate(prefab, hitInfoHolder.hit.point, Quaternion.AngleAxis(0, hitInfoHolder.hit.normal), selected.transform);    
            }
            else
            {
                currentGameObject.transform.SetParent(selected.transform, true);
                currentGameObject.transform.SetPositionAndRotation(hitInfoHolder.hit.point, Quaternion.AngleAxis(0, hitInfoHolder.hit.normal));
            }
            
        }
        public void HidePreview()
        {
            if (currentGameObject != null)
            {
                Object.Destroy(currentGameObject);
            }
        }

        private class HitInfoHolder
        {
            public RaycastHit hit;
        }
    }
}