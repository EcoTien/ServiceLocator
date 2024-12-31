using System;
using UnityEngine;

namespace EcoMine.Service
{
    public abstract class LocalService<T> : MonoBehaviour, IService where T : class, IService
    {
        void IService.RegisterService()
        {
            ServiceLocator.RegisterLocalService(this, this as T);
        }
    }
}