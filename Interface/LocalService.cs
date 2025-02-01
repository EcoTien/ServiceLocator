using EcoMine.Service;
using UnityEngine;

namespace EcoMine.Service
{
    public abstract class LocalService<T> : MonoBehaviour, IService where T : class, IService
    {
        public void RegisterService()
        {
            ServiceLocator.RegisterLocalService(this as T);
        }
    }
}