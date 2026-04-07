// _Source/AssetSystem/AssetHandle.cs

using System;

namespace AssetSystem
{
    public sealed class AssetHandle<T> : IDisposable where T : UnityEngine.Object
    {
        private readonly AssetManager _manager;
        private readonly AssetKey _key;
        private bool _disposed;

        public T Asset { get; private set; }

        internal AssetHandle(AssetManager manager, AssetKey key, T asset)
        {
            _manager = manager;
            _key = key;
            Asset = asset;
        }

        public void Release()
        {
            if (_disposed) return;
            _manager.ReleaseInternal(_key, Asset);
            _disposed = true;
            Asset = null;
        }

        public void Dispose()
        {
            Release();
        }
    }
}