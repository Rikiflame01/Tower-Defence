using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [System.Serializable]
    public struct VFXPrefab
    {
        public string vfxName;
        public GameObject prefab;
        public int initialPoolSize;
        public int maxPoolSize;
    }

    public List<VFXPrefab> vfxPrefabs;

    private Dictionary<string, ObjectPool<GameObject>> vfxPools = new Dictionary<string, ObjectPool<GameObject>>();
    private Dictionary<string, GameObject> vfxPrefabsDictionary = new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        foreach (var vfx in vfxPrefabs)
        {
            if (vfx.prefab != null && !vfxPools.ContainsKey(vfx.vfxName))
            {
                var pool = new ObjectPool<GameObject>(
                    createFunc: () => Instantiate(vfx.prefab),
                    actionOnGet: vfxInstance => vfxInstance.SetActive(true), 
                    actionOnRelease: vfxInstance => vfxInstance.SetActive(false), 
                    actionOnDestroy: Destroy, 
                    collectionCheck: false,
                    defaultCapacity: vfx.initialPoolSize,
                    maxSize: vfx.maxPoolSize
                );

                vfxPools.Add(vfx.vfxName, pool);
                vfxPrefabsDictionary.Add(vfx.vfxName, vfx.prefab);
            }
        }
    }

    public void SpawnVFX(string vfxName, Vector3 position, float uniformScale = 1.0f, int newPoolSize = 0)
    {
        if (vfxPools.TryGetValue(vfxName, out ObjectPool<GameObject> vfxPool))
        {
            GameObject vfxInstance = vfxPool.Get();

            vfxInstance.transform.position = position;
            vfxInstance.transform.localScale = Vector3.one * uniformScale;

            StartCoroutine(ReturnToPoolAfterDelay(vfxName, vfxInstance, 2.0f));
        }
        else
        {
            Debug.LogWarning($"VFX '{vfxName}' not found in VFXManager.");
        }
    }

    private System.Collections.IEnumerator ReturnToPoolAfterDelay(string vfxName, GameObject vfxInstance, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (vfxPools.TryGetValue(vfxName, out ObjectPool<GameObject> vfxPool))
        {
            vfxPool.Release(vfxInstance);
        }
    }
}
