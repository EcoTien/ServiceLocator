using System;
using UnityEngine;

namespace EcoMine.ServiceLocator
{
    public abstract class GlobalService<T> : MonoBehaviour, IService where T : class, IService
    {
        void IService.RegisterService()
        {
            ServiceLocator.RegisterGlobalService(this, this as T);
        }
    }
}