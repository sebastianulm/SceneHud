using UnityEngine;

namespace net.thewired.SceneHud
{
    public class PickPhysicsPrefabRoot : IPickable
    {
        public bool Pick(Ray ray, Vector2 pos, out GameObject selectedObject, out GameObject prefab)
        {
            selectedObject = null;
            prefab = null;
            return false;
        }
    }
}