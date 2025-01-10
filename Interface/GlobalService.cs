using System;
using UnityEngine;

namespace EcoMine.Service
{
    public abstract class GlobalService<T> : MonoBehaviour, IService where T : class, IService
    {
        private bool _isRegister;
        
        void IService.RegisterService()
        {
            if(_isRegister) return;
            if (ServiceLocator.IsRegistered<T>(this))
            {
                Destroy(this);
            }
            else
            {
                _isRegister = true;
                DontDestroyOnLoad(this);
                ServiceLocator.RegisterGlobalService(this, this as T);
            }
        }
    }
}