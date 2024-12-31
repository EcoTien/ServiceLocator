using System;
using UnityEngine;

namespace EcoMine.Service
{
    public abstract class GlobalService<T> : MonoBehaviour, IService where T : class, IService
    {
        void IService.RegisterService()
        {
            if (ServiceLocator.IsRegistered<T>(this))
            {
                Destroy(this);
            }
            else
            {
                DontDestroyOnLoad(this);
                ServiceLocator.RegisterGlobalService(this, this as T);
            }
        }
    }
}