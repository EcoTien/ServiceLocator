using UnityEngine;
using UnityEngine.SceneManagement;

namespace EcoMine.ServiceLocator
{
    internal sealed class ServiceLocatorRuntime
    {
        public delegate void SceneLoadedEvent(Scene scene, LoadSceneMode loadSceneMode);
        public delegate void SceneUnLoadedEvent(Scene scene);
        
        public SceneLoadedEvent OnSceneLoadedHandler;
        public SceneUnLoadedEvent OnSceneUnLoadedHandler;
        
        public void Initialized()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            InitializedUnityEditor();
            Debug.Log("Service Locator Runtime Initialized.");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            ServiceFilter serviceFilter = new ServiceFilter();
            serviceFilter.Filter().ForEach(service => service.RegisterService());
            serviceFilter.Dispose();
            
            OnSceneLoadedHandler?.Invoke(scene, loadSceneMode);
        }

        private void OnSceneUnloaded(Scene scene)
        {
            ServiceLocator.UnregisterAllLocalService(scene);
            OnSceneUnLoadedHandler?.Invoke(scene);
        }
        
        private void InitializedUnityEditor()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }
#if UNITY_EDITOR
        private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                ServiceLocator.UnregisterAllService();
                UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            }
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
#endif
    }
}