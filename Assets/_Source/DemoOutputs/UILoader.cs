using UnityEngine;

namespace DemoOutputs
{
    public class UILoader : MonoBehaviour
    {
        [SerializeField] private SceneGroupLoader facilitiesLoader;
        [SerializeField] private SceneGroupLoader natureLoader;

        public void OnLoadFacilities()
        {
            _ = facilitiesLoader.LoadGroupAsync();
        }

        public void OnUnloadFacilities()
        {
            facilitiesLoader.UnloadGroup();
        }

        public void OnLoadNature()
        {
            _ = natureLoader.LoadGroupAsync();
        }

        public void OnUnloadNature()
        {
            natureLoader.UnloadGroup();
        }
    }
}