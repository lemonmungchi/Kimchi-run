using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner
{
    private List<GameObject> _objectsToSpawn;  // 스폰할 오브젝트 리스트
    private string _resourcePath;             // Resources 폴더 내 경로
    private string _spawnType;                // 스폰 타입 (예: "Food", "GoldenFood")

    public Spawner(string resourcePath, string spawnType)
    {
        _resourcePath = resourcePath;
        _spawnType = spawnType;
        _objectsToSpawn = new List<GameObject>();
    }

    /// <summary>
    /// Resources 폴더에서 프리팹 로드 및 오브젝트 풀 생성
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
    /// 랜덤 오브젝트를 스폰
    /// </summary>
    public GameObject SpawnRandom(Vector3 position)
    {
        if (_objectsToSpawn == null || _objectsToSpawn.Count == 0)
        {
            Debug.LogWarning($"{_spawnType} 프리팹이 없습니다!");
            return null;
        }

        // 랜덤 오브젝트 선택
        int randomIndex = Random.Range(0, _objectsToSpawn.Count);
        GameObject prefab = _objectsToSpawn[randomIndex];

        // 풀에서 오브젝트 가져오기
        Poolable poolable = Managers.Pool.Pop(prefab, Managers.Pool.GetRoot());
        GameObject spawnedObject = poolable.gameObject;

        // 위치 설정
        spawnedObject.transform.position = position;

        Debug.Log($"{_spawnType} 생성: {spawnedObject.name}");
        return spawnedObject;
    }

    /// <summary>
    /// 주기적으로 오브젝트를 스폰하는 코루틴
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
