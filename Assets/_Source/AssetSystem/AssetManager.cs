// _Source/AssetSystem/AssetManager.cs

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using IInitializable = Zenject.IInitializable;

namespace AssetSystem
{
    public class AssetManager : IInitializable, IDisposable
    {
        private readonly IAssetRepository _cache;
        private readonly Dictionary<AssetKey, List<TaskCompletionSource<object>>> _pending = new();
        private readonly Dictionary<AssetKey, int> _refCount = new();
        private readonly List<IAssetLoader> _loaders = new();

        public AssetManager(IAssetRepository cache)
        {
            _cache = cache;
        }

        public void Initialize()
        {
            _loaders.Add(new ResourcesLoader());
            Debug.Log($"{GetType()} Initialized");
        }

        public async Task<AssetHandle<T>> LoadAsync<T>(string path, CancellationToken ct = default)
            where T : UnityEngine.Object
        {
            Debug.Log($"{GetType()} LoadAsync called for path={path}, type={typeof(T)}");
            var key = new AssetKey(path, typeof(T));
            Debug.Log($"{GetType()} Key created: {key}");

            // 1. Проверка кеша
            if (_cache.TryGet(key, out var cached))
            {
                Debug.Log($"{GetType()} Found in cache");
                IncreaseRefCount(key);
                return new AssetHandle<T>(this, key, (T)cached);
            }

            // 2. Дедупликация: если уже грузится, ждём тот же результат
            if (_pending.TryGetValue(key, out var pendingList))
            {
                var tcs = new TaskCompletionSource<object>();
                pendingList.Add(tcs);
                try
                {
                    var result = await tcs.Task;
                    IncreaseRefCount(key);
                    return new AssetHandle<T>(this, key, (T)result);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to load {key}", ex);
                }
            }

            // 3. Новая загрузка
            _pending[key] = new List<TaskCompletionSource<object>>();
            var mainTcs = new TaskCompletionSource<object>();
            _pending[key].Add(mainTcs);

            try
            {
                var loader = GetLoader<T>(path);
                var asset = await loader.LoadAsync<T>(path, ct);
                _cache.Add(key, asset);
                IncreaseRefCount(key);
                LogLoad(key, "loaded");
                mainTcs.SetResult(asset);

                foreach (var waiter in _pending[key])
                    if (waiter != mainTcs)
                        waiter.TrySetResult(asset);
                _pending.Remove(key);

                return new AssetHandle<T>(this, key, asset);
            }
            catch (Exception ex)
            {
                mainTcs.SetException(ex);
                foreach (var waiter in _pending[key])
                    if (waiter != mainTcs)
                        waiter.TrySetException(ex);
                _pending.Remove(key);
                throw;
            }
        }

        internal void ReleaseInternal<T>(AssetKey key, T asset) where T : UnityEngine.Object
        {
            if (!_refCount.ContainsKey(key)) return;
            _refCount[key]--;
            if (_refCount[key] <= 0)
            {
                var loader = GetLoader<T>(key.Path);
                loader.Release(asset);
                _cache.Remove(key);
                _refCount.Remove(key);
                LogUnload(key);
            }
        }

        private void IncreaseRefCount(AssetKey key)
        {
            if (_refCount.ContainsKey(key))
                _refCount[key]++;
            else
                _refCount[key] = 1;
        }

        private IAssetLoader GetLoader<T>(string path) where T : UnityEngine.Object
        {
            if (_loaders == null || _loaders.Count == 0)
                throw new Exception(
                    $"{GetType()} No loaders registered! Did you forget to call Initialize() or bind IInitializable?");

            foreach (var loader in _loaders)
                if (loader.CanLoad<T>(path))
                    return loader;
            return _loaders[0]; // fallback
        }

        private void LogLoad(AssetKey key, string source)
        {
            Debug.Log($"{GetType()} Load {key} ({source})");
        }

        private void LogUnload(AssetKey key)
        {
            Debug.Log($"{GetType()} Unload {key}");
        }

        public void Dispose()
        {
            foreach (var kv in _cache.GetAll())
                if (kv.Value is UnityEngine.Object obj)
                {
                    var loader = GetLoader<UnityEngine.Object>(kv.Key.Path);
                    loader.Release(obj);
                }

            _cache.Clear();
            _refCount.Clear();
            _pending.Clear();
        }
    }
}