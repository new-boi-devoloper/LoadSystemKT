// _Source/AssetSystem/IAssetLoader.cs

using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace AssetSystem
{
    public interface IAssetLoader
    {
        Task<T> LoadAsync<T>(string key, CancellationToken ct = default) where T : Object;
        void Release<T>(T asset) where T : Object;
        bool CanLoad<T>(string key);
    }
}