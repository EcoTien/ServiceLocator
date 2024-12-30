using UnityEngine;
using UnityEngine.SceneManagement;

namespace EcoMine.ServiceLocator
{
    internal sealed class ServiceLocatorRuntime
    {
        public delegate void SceneLoadedEvent(Scene scene);
        public delegate void SceneUnLoadedEvent(Scene scene);
        
        public SceneLoadedEvent OnSceneLoadedHandler;
        public SceneUnLoadedEvent OnSceneUnLoadedHandler;
        
        public void Initialized()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            InitializedUnityEditor();
            Debug.Log("Service Locator Runtime Initialized.");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnSceneLoaded()
        {
            ServiceFilter serviceFilter = new ServiceFilter();
            serviceFilter.Filter().ForEach(service => service.RegisterService());
            serviceFilter.Dispose();
            
            ServiceLocator.ServiceLocatorRuntime.OnSceneLoadedHandler?.Invoke(SceneManager.GetActiveScene());
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
        }
#endif
    }
}