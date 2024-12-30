using System;
using System.Collections.Generic;
using System.Linq;
using EcoMine.ServiceLocator.Disposable;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EcoMine.ServiceLocator
{
    internal sealed class ServiceFilter : DisposableBase
    {
        private List<IService> _services;

        public List<IService> Filter()
        {
            _services = Object.FindObjectsOfType<MonoBehaviour>().OfType<IService>().ToList();
            return _services;
        }

        protected override void OnDispose()
        {
            _services.Clear();
        }
    }
}