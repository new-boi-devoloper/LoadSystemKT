// _Source/AssetSystem/ResourcesLoader.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetSystem
{
    public class ResourcesLoader : IAssetLoader
    {
        public async Task<T> LoadAsync<T>(string key, CancellationToken ct = default) where T : Object
        {
            var request = Resources.LoadAsync<T>(key);
            while (!request.isDone)
            {
                if (ct.IsCancellationRequested)
                    throw new OperationCanceledException();
                await Task.Yield();
            }

            var asset = request.asset as T;
            if (asset == null)
                throw new Exception($"Asset not found: {key} of type {typeof(T)}");
            return asset;
        }

        public void Release<T>(T asset) where T : Object
        {
            // в случае префабов ничего не unleash 
            if (asset is GameObject || asset is Component)
                return;

            // для Texture2D, Material, AudioClip )
            if (asset != null)
                Resources.UnloadAsset(asset);
        }

        public bool CanLoad<T>(string key)
        {
            return true;
        }
    }
}