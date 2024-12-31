using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EcoMine.Service
{
    public static class ServiceLocator
    {
        /// <summary>
        /// Global services are services that are shared across all scenes.
        /// </summary>
        private static readonly Dictionary<Type, IService> _globalServices = new Dictionary<Type, IService>();
        
        /// <summary>
        /// Local services are services that are only available in the current scene.
        /// </summary>
        private static readonly Dictionary<Scene, Dictionary<Type, IService>> _localServices = new Dictionary<Scene, Dictionary<Type, IService>>();
        
        /*
        /// <summary>
        /// Service Locator Runtime.
        /// </summary>
        public static ServiceLocatorRuntime ServiceLocatorRuntime;
        
        /// <summary>
        /// Initialize Service Locator.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            Debug.Log("Service Locator Initialized.");
        }
        */

        /// <summary>
        /// Reigster Global Service to Service Locator.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="monoBehaviour"></param>
        /// <param name="service">IService</param>
        public static void RegisterGlobalService<T>(MonoBehaviour monoBehaviour, T service) where T : class, IService
        {
            var type = typeof(T);
            var scene = monoBehaviour.gameObject.scene;
            
            if (_globalServices.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} is already registered as global service.");
                return;
            }
            
            if(_localServices.ContainsKey(scene) && _localServices[scene].ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} is already registered as local service.");
                return;
            }

            _globalServices[type] = service;
            Debug.Log($"Service of type {type} registered successfully.");
        }

        /// <summary>
        /// Reigster Local Service to Service Locator.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="monoBehaviour"></param>
        /// <param name="service">IService</param>
        public static void RegisterLocalService<T>(MonoBehaviour monoBehaviour, T service) where T : class, IService
        {
            var type = typeof(T);
            var scene = monoBehaviour.gameObject.scene;
            
            if(_localServices.ContainsKey(scene) && _localServices[scene].ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} is already registered as local service.");
                return;
            }
            
            if (_globalServices.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} is already registered as global service.");
                return;
            }
            
            _localServices.TryAdd(scene, new Dictionary<Type, IService>());
            _localServices[scene][type] = service;
            Debug.Log($"Service of type {type} registered successfully.");
        }

        /// <summary>
        /// UnReigster Global Service to Service Locator.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public static void UnregisterGlobalService<T>() where T : class, IService
        {
            var type = typeof(T);

            if (!_globalServices.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} is not registered.");
                return;
            }

            _globalServices.Remove(type);
            Debug.Log($"Service of type {type} unregistered successfully.");
        }
        
        /// <summary>
        /// UnReigster Local Service to Service Locator.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public static void UnregisterLocalService<T>(MonoBehaviour monoBehaviour) where T : class, IService
        {
            var type = typeof(T);
            var scene = monoBehaviour.gameObject.scene;
            
            if (!_localServices.ContainsKey(scene) && !_localServices[scene].ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} is not registered.");
                return;
            }

            _localServices[scene].Remove(type);
            Debug.Log($"Service of type {type} unregistered successfully.");
        }

        /// <summary>
        /// UnReigster All Local Service to Service Locator.
        /// </summary>
        public static void UnregisterAllLocalService(Scene scene)
        {
            _localServices.Remove(scene);
            Debug.Log($"Service on scene {scene.name} unregistered successfully.");
        }
        
        /// <summary>
        /// UnReigster All Service to Service Locator.
        /// </summary>
        public static void UnregisterAllService()
        {
            _localServices.Clear();
            _globalServices.Clear();
            Debug.Log($"All Service unregistered successfully.");
        }

        /// <summary>
        /// Get Global or Local Service from Service Locator.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Service</returns>
        public static T GetService<T>(MonoBehaviour monoBehaviour = null) where T : class, IService
        {
            var type = typeof(T);
            var scene = SceneManager.GetActiveScene();
            if(monoBehaviour != null) scene = monoBehaviour.gameObject.scene;

            if (_globalServices.TryGetValue(type, out var service))
            {
                return service as T;
            }

            if (_localServices.ContainsKey(scene) && _localServices[scene].TryGetValue(type, out service))
            {
                return service as T;
            }
            
            Debug.LogError($"Service of type {type} is not registered.");
            return null;
        }

        /// <summary>
        /// Check if Service is Registered.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>True is service registered, False is not.</returns>
        public static bool IsRegistered<T>(MonoBehaviour monoBehaviour) where T : class, IService
        {
            var scene = monoBehaviour.gameObject.scene;
            if (_globalServices.ContainsKey(typeof(T)))
                return true;
            if(_localServices.ContainsKey(scene) && _localServices[scene].ContainsKey(typeof(T)))
                return true;
            return false;
        }
    }
}