using UnityEngine;

namespace EcoMine.Service
{
    public abstract class ScriptableObjectService<T> : ScriptableObject, IService where T : class, IService
    {
        /// <summary>
        /// Call on first app open
        /// </summary>
        protected abstract void OnServiceRegistered();
        
        public void RegisterService()
        {
            ServiceLocator.RegisterScriptTableObjectService(this as T);
            OnServiceRegistered();
        }
    }
}