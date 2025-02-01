using EcoMine.Service;
using UnityEngine;

namespace EcoMine.Service
{
    public abstract class GlobalService<T> : MonoBehaviour, IService where T : class, IService
    {
        private bool _isRegister;
        
        public void RegisterService()
        {
            if(_isRegister) return;
            if (ServiceLocator.IsRegistered<T>())
            {
                Destroy(this);
            }
            else
            {
                _isRegister = true;
                transform.SetParent(null);
                DontDestroyOnLoad(this);
               ServiceLocator.RegisterGlobalService(this as T);
            }
        }
    }
}