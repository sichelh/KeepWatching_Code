using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Compilation;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public enum PoolType
{

}

public class ObjectPoolManager : SceneOnlySingleton<ObjectPoolManager>
{
    [SerializeField] List<GameObject> poolObjectList = new List<GameObject>();
    private List<IPoolObject> pools = new List<IPoolObject>();
    private Dictionary<PoolType, Queue<GameObject>> poolObjects = new Dictionary<PoolType, Queue<GameObject>>();
    private Dictionary<PoolType, GameObject> registeredObj = new Dictionary<PoolType, GameObject>();
    private Dictionary<PoolType, Transform> parentCache = new Dictionary<PoolType, Transform>();

#if UNITY_EDITOR
    public void AutoAssignObject()
    {
        poolObjectList.Clear();
        string[] guids =
            UnityEditor.AssetDatabase.FindAssets("t:GameObject", new[] { "Assets/03. Prefabs/Pool" });

        foreach (string guid in guids)
        {
            string path  = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var    asset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (asset.TryGetComponent<IPoolObject>(out var poolObject))
            {
                if (poolObject != null && !poolObjectList.Contains(poolObject.GameObject))
                {
                    poolObjectList.Add(poolObject.GameObject);
                }
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
    protected override void Awake()
    {
        foreach (var obj in poolObjectList)
        {
            if (obj.TryGetComponent<IPoolObject>(out var ipool))
            {
                pools.Add(ipool);
            }
            else
            {
                Debug.LogError($"오브젝트에 IPoolObject이 상속 되어 있지 않습니다. {obj.name}");
            }
        }

        foreach (var pool in pools)
        {
            CreatePool(pool, pool.PoolSize);
        }
    }

    /// <summary>
    /// 풀에 오브잭트를 등록해주는 메서드
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="poolSize"></param>
    public void CreatePool(IPoolObject iPoolObject, int poolSize)
    {
        if (poolObjects.ContainsKey(iPoolObject.PoolType))
        {
            Debug.LogWarning($"등록된 풀이 있습니다. : {iPoolObject.PoolType}");
            return;
        }

        string     poolname   = iPoolObject.PoolType.ToString();
        PoolType   poolType   = iPoolObject.PoolType;
        GameObject poolObject = iPoolObject.GameObject;

        Queue<GameObject> newPool   = new Queue<GameObject>();
        GameObject        parentObj = new GameObject(poolname) { transform = { parent = transform } };
        parentCache[poolType] = parentObj.transform;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(poolObject, parentObj.transform);
            obj.name = poolname;
            obj.SetActive(false);
            newPool.Enqueue(obj);
        }

        poolObjects[poolType] = newPool;
        registeredObj[poolType] = poolObject;
    }

    /// <summary>
    /// 풀에서 오브젝트를 꺼내는 메서드
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject GetObject(PoolType poolType)
    {
        string poolName = poolType.ToString();
        if (!poolObjects.TryGetValue(poolType, out Queue<GameObject> pool))
        {
            Debug.LogWarning($"등록된 풀이 없습니다. : {poolType}");
            return null;
        }

        if (pool.Count > 0)
        {
            GameObject go = pool.Dequeue();
            go.SetActive(true);
            return go;
        }
        else
        {
            GameObject prefab = registeredObj[poolType];
            GameObject newObj = Instantiate(prefab);
            newObj.name = poolName;
            newObj.transform.SetParent(parentCache[poolType]);
            newObj.SetActive(true);
            return newObj;
        }
    }

    /// <summary>
    /// 오브젝트를 풀에 반환하는 함수
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="returnTime"></param>
    /// <param name="action"></param>
    public void ReturnObject(IPoolObject obj, float returnTime = 0, UnityAction action = null)
    {
        StartCoroutine(DelayedReturnObject(obj, action, returnTime));
    }

    IEnumerator DelayedReturnObject(IPoolObject obj, UnityAction action, float returnTime)
    {
        if (!poolObjects.ContainsKey(obj.PoolType))
        {
            Debug.LogWarning($"등록된 풀이 없습니다. : {obj.PoolType}");
            CreatePool(obj, 1);
        }

        yield return new WaitForSeconds(returnTime);
        obj.GameObject.SetActive(false);
        obj.GameObject.transform.position = Vector3.zero;
        action?.Invoke();
        poolObjects[obj.PoolType].Enqueue(obj.GameObject);
        obj.GameObject.transform.SetParent(parentCache[obj.PoolType]);
    }


    public void RemovePool(PoolType poolType)
    {
        Destroy(parentCache[poolType].gameObject);
        parentCache.Remove(poolType);
        poolObjects.Remove(poolType);
        registeredObj.Remove(poolType);
    }
}