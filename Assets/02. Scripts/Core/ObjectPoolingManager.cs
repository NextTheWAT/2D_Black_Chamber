using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPoolingManager : Singleton<ObjectPoolingManager>
{
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new();
    private Dictionary<GameObject, GameObject> instanceToPrefab = new();
    [SerializeField] private bool clearOnSceneChange = true;

    protected override void Initialize()
    {
        base.Initialize();
        poolDictionary = new();
        instanceToPrefab = new();

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    // 씬이 변경될 때 호출되는 메서드
    private void OnSceneChanged(Scene current, Scene next)
    {
        if (clearOnSceneChange)
            Clear();
    }

    // 오브젝트 풀에서 오브젝트를 가져오는 메서드
    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        if (!poolDictionary.ContainsKey(prefab))
            poolDictionary[prefab] = new();

        GameObject instanceObject = null;

        while (poolDictionary[prefab].Count > 0)
        {
            instanceObject = poolDictionary[prefab].Dequeue();
            if (instanceObject && instanceObject.activeSelf)
                break;
        }

        if (instanceObject == null)
            instanceObject = Instantiate(prefab, position, rotation);
        else
            instanceObject.transform.SetPositionAndRotation(position, rotation);

        instanceObject.SetActive(true);

        instanceToPrefab[instanceObject] = prefab;
        return instanceObject;
    }

    public GameObject Get(GameObject prefab, Vector3 position) => Get(prefab, position, Quaternion.identity);

    // 오브젝트 풀에 오브젝트를 반환하는 메서드
    public void Return(GameObject instance)
    {
        if (instance == null) return;

        if (instanceToPrefab.TryGetValue(instance, out GameObject prefab))
        {
            if (!poolDictionary.ContainsKey(prefab))
                poolDictionary[prefab] = new();

            // 중복 방지
            if (!poolDictionary[prefab].Contains(instance))
            {
                instance.SetActive(false);
                poolDictionary[prefab].Enqueue(instance);
            }
        }
        else
        {
            Destroy(instance);
        }
    }

    // 오브젝트 풀을 비우는 메서드
    private void Clear()
    {
        foreach (var queue in poolDictionary.Values)
        {
            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj != null)
                    Destroy(obj);
            }
        }
        poolDictionary.Clear();
        instanceToPrefab.Clear();
    }

}
