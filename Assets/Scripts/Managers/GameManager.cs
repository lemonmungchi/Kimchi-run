using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public Define.ThisGameis thisGameis { get; set; }

    // 플레이어는 하나뿐이니까
    private GameObject _player;

    // 빌딩 프리팹 리스트
    private List<GameObject> _buildings;

    // 스폰된 빌딩을 관리하는 리스트 
    private List<GameObject> _spawnedBuildings = new List<GameObject>();

    // 적 프리팹 리스트
    private List<GameObject> _enemies;

    // 스폰된 적을 관리하는 리스트 
    private List<GameObject> _spawnedEnemies = new List<GameObject>();

    // SpawningPool 등에서 빌딩이 추가/삭제되는 것을 알리기 위한 이벤트
    public Action<int> OnSpawnEvent;

    // 빌딩 스폰 간격 (초)
    private float minSpawnDelay = 3f;

    private float maxSpawnDelay = 7f;

    // 스폰 상태 플래그
    private bool _isSpawning_Building = false;
    private bool _isSpawning_Enemy = false;

    /// <summary>
    /// 플레이어를 찾기 위한 함수
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayer()
    {
        return _player;
    }

    /// <summary>
    /// GameObject(빌딩, 플레이어 등)를 생성하는 함수
    /// </summary>
    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        // 경로를 이용해 오브젝트 생성 (기존 로직)
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch (type)
        {
            case Define.WorldObject._buildings:
                _spawnedBuildings.Add(go);
                break;

            case Define.WorldObject.Player:
                _player = go;
                break;
        }
        return go;
    }

    /// <summary>
    /// 랜덤으로 빌딩을 풀에서 가져와 생성하는 함수
    /// </summary>
    public void SpawnRandomEnemy()
    {
        // 빌딩 프리팹이 비어 있는 경우 경고
        if (_enemies == null || _spawnedEnemies.Count == 0)
        {
            Debug.LogWarning("적 프리팹이 없습니다!");
            return;
        }

        // 랜덤 인덱스를 선택해 빌딩 프리팹 가져오기
        int randomIndex = UnityEngine.Random.Range(0, _spawnedEnemies.Count);
        GameObject prefab = _enemies[randomIndex];

        // 풀에서 빌딩 생성
        Poolable poolable = Managers.Pool.Pop(prefab);
        GameObject newEnemy = poolable.gameObject;

        // 생성된 빌딩의 위치 설정
        newEnemy.transform.position = new Vector3(13.64f, -3.9f, 20);

        // 생성된 빌딩 리스트에 추가 (옵션)
        _spawnedEnemies.Add(newEnemy);

        Debug.Log($"적 생성: {newEnemy.name}");
    }

    /// <summary>
    /// 랜덤으로 적을 풀에서 가져와 생성하는 함수
    /// </summary>
    public void SpawnRandomBuilding()
    {
        if (_buildings == null || _buildings.Count == 0)
        {
            Debug.LogWarning("빌딩 프리팹이 없습니다!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, _buildings.Count);
        GameObject prefab = _buildings[randomIndex];

        // 풀에서 빌딩 생성
        Poolable poolable = Managers.Pool.Pop(prefab, Managers.Pool.GetRoot());
        GameObject newBuilding = poolable.gameObject;

        // 위치 설정
        newBuilding.transform.position = new Vector3(13.64f, -3.9f, 50);
        Debug.Log($"빌딩 생성: {newBuilding.name}");
    }


    /// <summary>
    /// 인자로 받은 GameObject의 타입 반환
    /// </summary>
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        // 예시: 실제 구현 시, 
        // BaseController 등으로부터 WorldObjectType을 가져오거나,
        // 프리팹 태그, 레이어 등을 통해 식별 가능
        return Define.WorldObject.Unknown;
    }

    /// <summary>
    /// 빌딩이나 플레이어 등의 GameObject를 삭제하는 함수
    /// </summary>
    public void Despawn(GameObject go)
    {
        // 오브젝트 타입 확인
        Define.WorldObject type = GetWorldObjectType(go);

        switch (type)
        {
            case Define.WorldObject._buildings:
                // 풀로 반환
                Poolable poolable = go.GetComponent<Poolable>();
                if (poolable != null)
                {
                    Managers.Pool.Push(poolable);
                }
                else
                {
                    // 풀링되지 않은 경우 Destroy 처리
                    Managers.Resource.Destroy(go);
                }
                break;

            case Define.WorldObject.Player:
                if (_player == go)
                    _player = null;
                break;

            default:
                Managers.Resource.Destroy(go);
                break;
        }
    }

    /// <summary>
    /// GameManager 초기화 함수
    /// </summary>
    public void Init()
    {
        // 씬에 존재하는 "Player" 오브젝트 찾기
        _player = GameObject.Find("Player");

        // 1) 풀 프리팹들을 모두 로드
        GameObject[] buildingPrefabs = Resources.LoadAll<GameObject>("Prefabs/Buildings");
        _buildings = new List<GameObject>(buildingPrefabs);

        GameObject[] enemyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Enemy");
        _enemies = new List<GameObject>(enemyPrefabs);

        // 2) 오브젝트 풀 생성
        //    (모든 빌딩 프리팹을 PoolManager에 등록, 초기 수량 3개씩)
        foreach (GameObject buildingPrefab in buildingPrefabs)
        {
            Managers.Pool.CreatePool(buildingPrefab, 3);
        }

        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            Managers.Pool.CreatePool(enemyPrefab, 3);
        }

        Debug.Log($"[GameManager] Init complete! " +
                  $"Loaded {_buildings.Count} building prefabs from 'Resources/Prefabs/'.");

        StartBuildingSpawn();
        StartEnemySpawn();
    }

    /// <summary>
    /// 주기적으로 빌딩을 스폰하는 코루틴 시작
    /// </summary>
    public void StartBuildingSpawn()
    {
        if (!_isSpawning_Building)
            CoroutineHelper.StartCoroutine(SpawnBuildingCoroutine());
    }

    public void StartEnemySpawn()
    {
        if (!_isSpawning_Enemy)
            CoroutineHelper.StartCoroutine(SpawnEnemyCoroutine());
    }

    /// <summary>
    /// 빌딩 스폰을 담당하는 코루틴
    /// </summary>
    private IEnumerator SpawnBuildingCoroutine()
    {
        _isSpawning_Building = true;

        while (true)
        {
            // 랜덤 빌딩 생성
            SpawnRandomBuilding();

            // 다음 스폰까지 대기
            float randomInterval = UnityEngine.Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(randomInterval);
        }
    }

    /// <summary>
    /// 빌딩 스폰을 담당하는 코루틴
    /// </summary>
    private IEnumerator SpawnEnemyCoroutine()
    {
        _isSpawning_Enemy = true;

        while (true)
        {
            // 랜덤 빌딩 생성
            SpawnRandomEnemy();

            // 다음 스폰까지 대기
            float randomInterval = UnityEngine.Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(randomInterval);
        }
    }
}
