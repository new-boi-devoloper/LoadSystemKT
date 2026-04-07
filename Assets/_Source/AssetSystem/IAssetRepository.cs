// _Source/AssetSystem/AssetRepository.cs

using System.Collections.Generic;

namespace AssetSystem
{
    public interface IAssetRepository
    {
        bool TryGet(AssetKey key, out object asset);
        void Add(AssetKey key, object asset);
        bool Remove(AssetKey key);
        bool Contains(AssetKey key);
        void Clear();
        IEnumerable<KeyValuePair<AssetKey, object>> GetAll();
    }
}