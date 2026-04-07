using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AssetSystem;
using UnityEngine;
using Zenject;

namespace DemoOutputs
{
    public class SceneGroupLoader : MonoBehaviour
    {
        [Header("Group Settings")] [SerializeField]
        private string groupName;

        [SerializeField] private string[] prefabPaths;
        [SerializeField] private Transform parentTransform;

        [Inject] private AssetManager _assetManager;
        private readonly List<AssetHandle<GameObject>> _handles = new();
        private readonly List<GameObject> _instances = new();
        private CancellationTokenSource _cts;

        public async Task LoadGroupAsync()
        {
            if (_instances.Count > 0) UnloadGroup();

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            foreach (var path in prefabPaths)
                try
                {
                    var handle = await _assetManager.LoadAsync<GameObject>(path, _cts.Token);
                    _handles.Add(handle);

                    var instance = Instantiate(handle.Asset, parentTransform);
                    _instances.Add(instance);
                    Debug.Log($"[{groupName}] Loaded: {path}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load {path}: {e.Message}");
                }
        }

        public void UnloadGroup()
        {
            foreach (var instance in _instances)
                Destroy(instance);
            _instances.Clear();

            foreach (var handle in _handles)
                handle.Release();
            _handles.Clear();

            Debug.Log($"[{groupName}] Unloaded");
        }

        private void OnDestroy()
        {
            UnloadGroup();
            _cts?.Cancel();
        }
    }
}