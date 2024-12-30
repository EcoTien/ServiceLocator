using System;
using UnityEngine;

namespace EcoMine.ServiceLocator
{
    public abstract class GlobalService<T> : MonoBehaviour, IService where T : class, IService
    {
        void IService.RegisterService()
        {
            if (ServiceLocator.IsRegistered<T>(this))
            {
                Destroy(this);
                Debug.LogWarning($"Service of type {typeof(T)} is already registered as global service.");
            }
            else
            {
                DontDestroyOnLoad(this);
                ServiceLocator.RegisterGlobalService(this, this as T);
            }
        }
    }
}