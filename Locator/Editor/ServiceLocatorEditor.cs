using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace EcoMine.Service.Editor
{
    [InitializeOnLoad]
    internal sealed class ServiceLocatorEditor
    {
        private static string currentScenePath;
        static ServiceLocatorEditor()
        {
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
            EditorApplication.hierarchyChanged += HierarchyChanged;
            if(EditorApplication.isPlayingOrWillChangePlaymode) return;
            currentScenePath = SceneManager.GetActiveScene().path;
            InitializeServiceLocatorRuntime();
        }

        private static void HierarchyChanged()
        {
            string scenePath = SceneManager.GetActiveScene().path;
            if (CurrentSceneHasIService() && !CurrentSceneHasServiceLocatorRuntime())
                OpenSceneAndCreateServiceLocatorRuntime(scenePath, false);
        }

        private static void InitializeServiceLocatorRuntime()
        {
            string[] guids = AssetDatabase.FindAssets("t:Scene");
            bool isChangeScene = false;
            for (var i = 0; i < guids.Length; i++)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (HasIService(scenePath) && !HasPrefabServiceLocatorRuntime(scenePath))
                {
                    isChangeScene = true;
                    OpenSceneAndCreateServiceLocatorRuntime(scenePath);
                }
            }
            if(isChangeScene) OpenPersonalScene();
        }

        private static void OpenSceneAndCreateServiceLocatorRuntime(string scenePath, bool loadScene = true)
        {
            Scene scene = default;
            if(loadScene) scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            PrefabUtility.InstantiatePrefab(Resources.Load<ServiceLocatorRuntime>("ServiceLocatorRuntime"));
            if(loadScene) EditorSceneManager.SaveScene(scene);
        }
        
        private static void OpenPersonalScene()
        {
            if (!SceneManager.GetActiveScene().path.Equals(currentScenePath))
                EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
        }

        private static void PlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if(playModeStateChange == PlayModeStateChange.ExitingPlayMode)
                ServiceLocator.UnregisterAllService();
        }
        
        private static bool HasPrefabServiceLocatorRuntime(string scenePath)
        {
            string[] lines = File.ReadAllLines(scenePath);
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Resources.Load("ServiceLocatorRuntime")));
            return lines
                .Where(HasM_SourcePrefab)
                .Select(GetGuid)
                .Any(prefabGuid => prefabGuid.Equals(guid));
        }
        
        private static bool HasIService(string scenePath)
        {
            string[] lines = File.ReadAllLines(scenePath);
            return lines
                .Where(HasM_Script)
                .Select(GetGuid)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
                .Any(script => script != null && script.GetClass() != null && typeof(IService).IsAssignableFrom(script.GetClass()));
        }

        private static bool CurrentSceneHasIService()
        {
            return Object.FindObjectsOfType<MonoBehaviour>().OfType<IService>().Any();
        }

        private static bool CurrentSceneHasServiceLocatorRuntime()
        {
            return Object.FindObjectOfType<ServiceLocatorRuntime>() != null;
        }

        private static bool HasM_Script(string line)
        {
            return line.Contains("m_Script");
        }

        private static bool HasM_SourcePrefab(string line)
        {
            return line.Contains("m_SourcePrefab");
        }

        private static string GetGuid(string guid)
        {
            string pattern = @"guid:\s([a-f0-9]{32})";
            Regex regex = new Regex(pattern);
            
            Match match = regex.Match(guid);
            return match.Groups[1].Value;
        }
    }
}