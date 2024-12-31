using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EcoMine.Service.Editor
{
    [InitializeOnLoad]
    internal sealed class ServiceLocatorEditor
    {
        private static string currentScenePath;
        static ServiceLocatorEditor()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
            
            if(EditorApplication.isPlayingOrWillChangePlaymode) return;
            
            currentScenePath = SceneManager.GetActiveScene().path;
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            bool needInitialize = false;
            
            for (int i = 0; i < scenes.Length; i++)
            {
                string scenePath = scenes[i].path;
                
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                if (IsHasComponent<ServiceLocatorRuntime>(scene)) continue;
                needInitialize = true;
            }
            if (needInitialize)
            {
                bool userChoice = EditorUtility.DisplayDialog(
                    "Service Locator Initialize", 
                    "Do you want to initialize the service locator runtime?", 
                    "Yes", "No");

                if (userChoice) InitializeServiceLocatorRuntime();
                else OpenPersionalScene();
            }
            else
            {
                OpenPersionalScene();
            }
        }

        private static void InitializeServiceLocatorRuntime()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < scenes.Length; i++)
            {
                string scenePath = scenes[i].path;
                
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                if (IsHasComponent<ServiceLocatorRuntime>(scene)) continue;
                
                PrefabUtility.InstantiatePrefab(Resources.Load<ServiceLocatorRuntime>("ServiceLocatorRuntime"));
                EditorSceneManager.SaveScene(scene);
            }
            OpenPersionalScene();
        }
        
        private static void OpenPersionalScene()
        {
            if (!SceneManager.GetActiveScene().path.Equals(currentScenePath))
                EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
        }

        private static void PlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if(playModeStateChange == PlayModeStateChange.ExitingPlayMode)
                ServiceLocator.UnregisterAllService();
        }

        private static bool IsHasComponent<T>(Scene scene)
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            for (int i = 0; i < rootGameObjects.Length; i++)
            {
                T component = rootGameObjects[i].GetComponentInChildren<T>();
                if (component != null)
                    return true;
            }
            return false;
        }
    }
}