using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        // этот класс делает Instantiate Fader'а ЛИШЬ 1 РАЗ после запуска игры,
        // а затем следит чтобы на новой сцене Fader не удалялся

        [SerializeField] GameObject persistentObjectPrefab;
        static bool hasSpawned = false;     // после Instantiate на любой сцене оставайся static true 

        private void Awake()
        {            
            if (hasSpawned) return;
            SpawnPersistentObject();
            hasSpawned = true;

        }

        private void SpawnPersistentObject()
        {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}
