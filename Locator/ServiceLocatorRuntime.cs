using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EcoMine.Service
{
    [DefaultExecutionOrder(-1001)]
    public sealed class ServiceLocatorRuntime : MonoBehaviour
    {
        private Scene _scene;

        private void Awake() => Initialized();

        private void Initialized()
        {
            _scene = SceneManager.GetActiveScene();
            EditorFilterService();
            Debug.Log("Service Locator Runtime Initialized.");
        }
        
        private void EditorFilterService()
        {
            /*#if UNITY_EDITOR*/
            ServiceFilter serviceFilter = new ServiceFilter();
            serviceFilter.Filter().ForEach(service => service.RegisterService());
            serviceFilter.Dispose();
            /*#endif*/
        }

        private void OnDestroy()
        {
            ServiceLocator.UnregisterAllLocalService(_scene);
        }
    }
}