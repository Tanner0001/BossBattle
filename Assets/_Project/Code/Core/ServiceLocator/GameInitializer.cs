using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using _Project.Code.Gameplay.Scenes;

namespace _Project.Code.Core.ServiceLocator
{
    public class GameInitializer : MonoBehaviour
    {
        [Header("Persistent Services")]
        [SerializeField] private List<MonoBehaviourService> _servicePrefabs = new();

        [Header("Scene Loading")]
        [SerializeField] private bool _loadSceneOnStart = true;
        [SerializeField] private string _sceneToLoad;

        private static bool _isInitialized = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            Application.quitting += ServiceLocator.Clear;
        }

        private void Awake()
        {
            if (_isInitialized)
            {
                Destroy(gameObject);
                return;
            }

            _isInitialized = true;
            DontDestroyOnLoad(gameObject);

            InitializeServices();

            if (_loadSceneOnStart)
            {
                LoadInitalScene();
            }

        }

        private void InitializeServices()
        {
            Debug.Log($"[GameInitializer] Starting service initialization. Found {_servicePrefabs.Count} prefabs");

            foreach (var servicePrefab in _servicePrefabs)
            {
                if (servicePrefab == null)
                {
                    Debug.LogWarning("[GameInitializer] Null service prefab in list");
                    continue;
                }

                Debug.Log($"[GameInitializer] Instantiating {servicePrefab.GetType().Name}");
                var serviceInstance = Instantiate(servicePrefab, transform);
                var serviceType = serviceInstance.GetType();

                Debug.Log($"[GameInitializer] Service instance type: {serviceType.Name}");

                // Use reflection to call ServiceLocator.Register<T>(instance)
                var registerMethod = typeof(ServiceLocator)
                    .GetMethod(nameof(ServiceLocator.Register))
                    .MakeGenericMethod(serviceType);

                registerMethod.Invoke(null, new object[] { serviceInstance });

                Debug.Log($"[GameInitializer] Successfully registered {serviceType.Name}");
            }

            Debug.Log("[GameInitializer] Service initialization complete");
        }

        private void LoadInitalScene()
        {
            var sceneService = ServiceLocator.Get<SceneService>();
            if (sceneService != null)
            {
                sceneService.LoadSceneAsync(_sceneToLoad);

            }
            else
            {
                Debug.Log("Scene wont load, scene service is not found");
            }
        }

    }
}