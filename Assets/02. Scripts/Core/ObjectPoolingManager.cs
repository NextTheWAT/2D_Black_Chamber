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

    // ���� ����� �� ȣ��Ǵ� �޼���
    private void OnSceneChanged(Scene current, Scene next)
    {
        if (clearOnSceneChange)
            Clear();
    }

    // ������Ʈ Ǯ���� ������Ʈ�� �������� �޼���
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

    // ������Ʈ Ǯ�� ������Ʈ�� ��ȯ�ϴ� �޼���
    public void Return(GameObject instance)
    {
        if (instance == null) return;

        if (instanceToPrefab.TryGetValue(instance, out GameObject prefab))
        {
            if (!poolDictionary.ContainsKey(prefab))
                poolDictionary[prefab] = new();

            // �ߺ� ����
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

    // ������Ʈ Ǯ�� ���� �޼���
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
