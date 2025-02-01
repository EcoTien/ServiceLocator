using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Script table object services are services that are shared across all scenes.
        /// </summary>
        private static readonly Dictionary<Type, IService> _scriptTableObjectServices = new Dictionary<Type, IService>();
        
        /// <summary>
        /// Local services are services that are only available in the current scene.
        /// </summary>
        private static readonly Dictionary<Scene, Dictionary<Type, IService>> _localServices = new Dictionary<Scene, Dictionary<Type, IService>>();
        
        /// <summary>
        /// Initialize Service Locator.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            IService[] services = Resources.LoadAll<ScriptableObject>("").OfType<IService>().ToArray();
            for (var i = 0; i < services.Length; i++)
                services[i].RegisterService();
            Debug.Log("Service Locator Initialized.");
        }
        
        /// <summary>
        /// Register Script table object Service to Service Locator.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="service">IService</param>
        public static void RegisterScriptTableObjectService<T>(T service) where T : class, IService
        {
            var type = typeof(T);
            if (!_scriptTableObjectServices.TryAdd(type, service))
            {
                Debug.LogWarning($"Service of type {type} is already registered as script table object service.");
            }
            else
            {
                Debug.Log($"Service of type {type} registered successfully.");
            }
        }

        /// <summary>
        /// Register Global Service to Service Locator.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="service">IService</param>
        public static void RegisterGlobalService<T>(T service) where T : class, IService
        {
            var type = typeof(T);
            if (!_globalServices.TryAdd(type, service))
            {
                Debug.LogWarning($"Service of type {type} is already registered as global service.");
            }
            else
            {
                Debug.Log($"Service of type {type} registered successfully.");
            }
        }

        /// <summary>
        /// Register Local Service to Service Locator.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="service">IService</param>
        public static void RegisterLocalService<T>(T service) where T : class, IService
        {
            var type = typeof(T);
            var scene = SceneManager.GetActiveScene();
            
            if(_localServices.ContainsKey(scene) && _localServices[scene].ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} is already registered as local service.");
            }
            else
            {
                _localServices.TryAdd(scene, new Dictionary<Type, IService>());
                _localServices[scene][type] = service;
                Debug.Log($"Service of type {type} registered successfully.");
            }
        }

        /// <summary>
        /// UnRegister Global Service to Service Locator.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public static void UnregisterGlobalService<T>() where T : class, IService
        {
            var type = typeof(T);

            if (!_globalServices.ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} is not registered.");
            }
            else
            {
                _globalServices.Remove(type);
                Debug.Log($"Service of type {type} unregistered successfully.");
            }
        }
        
        /// <summary>
        /// UnRegister Local Service to Service Locator.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        public static void UnregisterLocalService<T>(MonoBehaviour monoBehaviour) where T : class, IService
        {
            var type = typeof(T);
            var scene = monoBehaviour.gameObject.scene;
            
            if (!_localServices.ContainsKey(scene) && !_localServices[scene].ContainsKey(type))
            {
                Debug.LogWarning($"Service of type {type} is not registered.");
            }
            else
            {
                _localServices[scene].Remove(type);
                Debug.Log($"Service of type {type} unregistered successfully.");
            }
        }

        /// <summary>
        /// UnRegister All Local Service to Service Locator.
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
            _scriptTableObjectServices.Clear();
            Debug.Log($"All Service unregistered successfully.");
        }

        /// <summary>
        /// Get Global or Local Service from Service Locator.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Service</returns>
        public static T GetService<T>() where T : class, IService
        {
            var type = typeof(T);
            var scene = SceneManager.GetActiveScene();

            if (_globalServices.TryGetValue(type, out var service))
                return service as T;

            if (_localServices.ContainsKey(scene) && _localServices[scene].TryGetValue(type, out service))
                return service as T;

            if (_scriptTableObjectServices.TryGetValue(type, out service))
                return service as T;
            
            throw new NullReferenceException($"Service of type {type} is not registered.");
        }

        /// <summary>
        /// Check if Service is Registered.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>True is service registered, False is not.</returns>
        public static bool IsRegistered<T>() where T : class, IService
        {
            var scene = SceneManager.GetActiveScene();
            if (_globalServices.ContainsKey(typeof(T)))
                return true;
            if(_localServices.ContainsKey(scene) && _localServices[scene].ContainsKey(typeof(T)))
                return true;
            if (_scriptTableObjectServices.ContainsKey(typeof(T)))
                return true;
            return false;
        }
    }
}