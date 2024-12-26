using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner
{
    private List<GameObject> _objectsToSpawn;  // ������ ������Ʈ ����Ʈ
    private string _resourcePath;             // Resources ���� �� ���
    private string _spawnType;                // ���� Ÿ�� (��: "Food", "GoldenFood")

    public Spawner(string resourcePath, string spawnType)
    {
        _resourcePath = resourcePath;
        _spawnType = spawnType;
        _objectsToSpawn = new List<GameObject>();
    }

    /// <summary>
    /// Resources �������� ������ �ε� �� ������Ʈ Ǯ ����
    /// </summary>
    public void InitializePool(int defaultPoolCount, System.Predicate<GameObject> filter = null)
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>(_resourcePath);

        foreach (GameObject prefab in prefabs)
        {
            if (filter == null || filter(prefab))
            {
                _objectsToSpawn.Add(prefab);
                Managers.Pool.CreatePool(prefab, defaultPoolCount);
            }
        }

        Debug.Log($"[{_spawnType}] Initialized with {_objectsToSpawn.Count} prefabs from '{_resourcePath}'");
    }

    /// <summary>
    /// ���� ������Ʈ�� ����
    /// </summary>
    public GameObject SpawnRandom(Vector3 position)
    {
        if (_objectsToSpawn == null || _objectsToSpawn.Count == 0)
        {
            Debug.LogWarning($"{_spawnType} �������� �����ϴ�!");
            return null;
        }

        // ���� ������Ʈ ����
        int randomIndex = Random.Range(0, _objectsToSpawn.Count);
        GameObject prefab = _objectsToSpawn[randomIndex];

        // Ǯ���� ������Ʈ ��������
        Poolable poolable = Managers.Pool.Pop(prefab, Managers.Pool.GetRoot());
        GameObject spawnedObject = poolable.gameObject;

        // ��ġ ����
        spawnedObject.transform.position = position;

        Debug.Log($"{_spawnType} ����: {spawnedObject.name}");
        return spawnedObject;
    }

    /// <summary>
    /// �ֱ������� ������Ʈ�� �����ϴ� �ڷ�ƾ
    /// </summary>
    public IEnumerator SpawnCoroutine(float minDelay, float maxDelay, Vector3 position)
    {
        while (true)
        {
            SpawnRandom(position);
            float randomDelay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(randomDelay);
        }
    }
}
