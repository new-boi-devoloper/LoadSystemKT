using System.Collections.Generic;

namespace AssetSystem
{
    public class AssetRepository : IAssetRepository
    {
        private readonly Dictionary<AssetKey, object> _cache = new();

        public bool TryGet(AssetKey key, out object asset)
        {
            return _cache.TryGetValue(key, out asset);
        }

        public void Add(AssetKey key, object asset)
        {
            _cache[key] = asset;
        }

        public bool Remove(AssetKey key)
        {
            return _cache.Remove(key);
        }

        public bool Contains(AssetKey key)
        {
            return _cache.ContainsKey(key);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public IEnumerable<KeyValuePair<AssetKey, object>> GetAll()
        {
            return _cache;
        }
    }
}