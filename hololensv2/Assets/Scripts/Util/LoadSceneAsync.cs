/// <summary>
/// Lädt eine Szene asynchron und unterstützt das Entladen anderer Szenen.
/// Kann Szenen additiv laden und bestimmte Szenen vom Entladen ausnehmen.
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AMI.Util
{
    /// <summary>
    /// Loads a Scene asynchronous
    /// </summary>
    public class LoadSceneAsync : MonoBehaviour
    {
        /// <summary>
        /// Name of the scene to load
        /// </summary>
        [Tooltip("Name of the scene to load")] [SerializeField]
        string sceneName;

        public string SceneName
        {
            get => sceneName;
            set => sceneName = value;
        }

        /// <summary>
        /// List of scenes (names) to not unload when scene is loaded
        /// </summary>
        [Tooltip("List of scenes (names) to not unload when scene is loaded")] [SerializeField]
        List<string> scenesToNotUnload;

        /// <summary>
        /// Should all other scenes be unloaded on scene load? (except scenesToNotUnload)
        /// </summary>
        [Tooltip("Should all other scenes be unloaded on scene load? (except scenesToNotUnload)")] [SerializeField]
        bool unloadAllOnLoadScene = false;

        /// <summary>
        /// Load the Scene asynchronously
        /// </summary>
        public void LoadScene()
        {
            if (unloadAllOnLoadScene)
            {
                UnloadAllScenes();
            }

            var sceneCount = SceneManager.sceneCount;
            if (scenesToNotUnload.Count == 0)
            {
                SceneManager.LoadScene(sceneName);
                return;
            }

            for (int i = 0; i < sceneCount; i++)
            {
                var currentScene = SceneManager.GetSceneAt(i);
                if (currentScene.isLoaded)
                {
                    if (currentScene.name == sceneName)
                    {
                        Console.Log("LoadSceneAsync", $"Scene {sceneName} already loaded");
                        return;
                    }
                    else if (!scenesToNotUnload.Contains(currentScene.name))
                    {
                        Console.Log("LoadSceneAsync", $"Unload Scene {currentScene.name}");
                        SceneManager.UnloadSceneAsync(currentScene);
                    }
                }
            }

            Console.Log("LoadSceneAsync", $"Lade Scene: {sceneName}");
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        /// <summary>
        /// Unload all scenes (except scenesToNotUnload)
        /// </summary>
        public void UnloadAllScenes()
        {
            var sceneCount = SceneManager.sceneCount;
            for (int i = 1; i < sceneCount; i++)
            {
                var currentScene = SceneManager.GetSceneAt(i);
                if (!scenesToNotUnload.Contains(currentScene.name))
                {
                    Console.Log("LoadSceneAsync", $"Unload Scene {currentScene.name}");
                    SceneManager.UnloadSceneAsync(currentScene);
                }
                else
                {
                    Console.Log("LoadSceneAsync",
                        $"Don't Unload Scene {currentScene.name} because it is contained in ScenesToNotUnload");
                }
            }

            Console.Log("LoadSceneAsync", "Unloaded all Scenes");
        }
    }
}