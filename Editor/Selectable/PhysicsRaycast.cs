using UnityEditor;
using UnityEngine;

namespace net.thewired.SceneHud
{
    public class PhysicsRaycast : IRaycastSelectable
    {
        public static QueryTriggerInteraction Interaction = QueryTriggerInteraction.Collide;
        public bool Cast(Ray ray, Vector2 pos, out float distance, out GameObject selectedObject)
        {
            if (Physics.Raycast(ray, out var hitInfo, 1000, -1, Interaction))
            {
                selectedObject = hitInfo.collider.gameObject;
                distance = hitInfo.distance;
                return true;
            }
            distance = float.MaxValue;
            selectedObject = null; 
            return false;
        }
    }
}