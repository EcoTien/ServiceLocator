using UnityEngine;

namespace EcoMine.ServiceLocator
{
    public interface IService
    {
        /// <summary>
        /// Register service to service locator.
        /// </summary>
        void RegisterService();
    }
}