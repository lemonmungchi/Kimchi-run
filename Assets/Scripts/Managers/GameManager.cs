using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager
{
    public Define.ThisGameis thisGameis { get; set; }

    // 플레이어는 하나뿐이니까
    private GameObject _player;

    //스폰할 오브젝트
    private Spawner _buildingSpawner;
    private Spawner _enemySpawner;
    private Spawner _foodSpawner;
    private Spawner _goldenFoodSpawner;

    private float playTime;
    public int currentScore;
    private int highScore;

    // 점수 변경 이벤트
    public event System.Action<int> OnScoreChanged;
    public event System.Action<int> OnHighScoreChanged;


    
    public float PlayTime
    {
        get => playTime;
        set => playTime = value;    
    }

    public int CurrentScore
    {
        get => currentScore;
        set
        {
            currentScore = value;
            OnScoreChanged?.Invoke(currentScore);
        }
    }

    public int HighScore
    {
        get=> highScore;
        set
        {
            highScore = value;
            OnHighScoreChanged?.Invoke(highScore);
        }
    }
  

    /// <summary>
    /// 점수를 주기적으로 업데이트하는 코루틴
    /// </summary>
    private IEnumerator UpdateScoreCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // 1초마다 실행
            CurrentScore = Mathf.FloorToInt(Time.time - PlayTime);
        }
    }


    public void SaveHighScore()
    {
        int currentHighScore = PlayerPrefs.GetInt("highScore");
        if(currentScore>currentHighScore)
        {
            OnHighScoreChanged?.Invoke(currentScore);
            PlayerPrefs.SetInt("highScore",HighScore);
            PlayerPrefs.Save();
        }
        
    }

    public float CaculateGameSpeed()
    {
        float speed = 3f + (0.5f * MathF.Floor(currentScore / 10f));
        return MathF.Min(speed, 30f);
    }


    // SpawningPool 등에서 빌딩이 추가/삭제되는 것을 알리기 위한 이벤트
    public Action<int> OnSpawnEvent;

    // 빌딩 스폰 간격 (초)
    private float minSpawnDelay = 3f;

    private float maxSpawnDelay = 7f;

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
                //_spawnedBuildings.Add(go);
                break;

            case Define.WorldObject.Player:
                _player = go;
                break;
        }
        return go;
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
            case Define.WorldObject.Enemy:
            case Define.WorldObject.food:
            case Define.WorldObject.goldenfood:
                Poolable poolable = go.GetComponent<Poolable>();
                // 풀로 반환
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
                Debug.LogWarning($"[GameManager] Unknown object type: {type}");
                Managers.Resource.Destroy(go); // 정의되지 않은 경우 Destroy
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

        // Spawner 초기화
        _buildingSpawner = new Spawner("Prefabs/Buildings", "Building");
        _buildingSpawner.InitializePool(3);

        _enemySpawner = new Spawner("Prefabs/Enemy", "Enemy");
        _enemySpawner.InitializePool(3);

        _foodSpawner = new Spawner("Prefabs/Food", "Food");
        _foodSpawner.InitializePool(5, prefab => prefab.GetComponent<Food>().GetFoodType() != FoodType.Golden_Baechu);

        _goldenFoodSpawner = new Spawner("Prefabs/GoldenFood", "GoldenFood");
        _goldenFoodSpawner.InitializePool(2, prefab => prefab.GetComponent<Food>().GetFoodType() == FoodType.Golden_Baechu);

        Debug.Log("[GameManager] Init complete!");
    }

    public void StartBuildingSpawn()
    {
        CoroutineHelper.StartCoroutine(_buildingSpawner.SpawnCoroutine(minSpawnDelay, maxSpawnDelay, new Vector3(16.64f, -3.9f, 50)));
    }

    public void StartEnemySpawn()
    {
        CoroutineHelper.StartCoroutine(_enemySpawner.SpawnCoroutine(minSpawnDelay, maxSpawnDelay, new Vector3(13.64f, -3.9f, 20)));
    }

    public void StartFoodSpawn()
    {
        CoroutineHelper.StartCoroutine(_foodSpawner.SpawnCoroutine(5f, 40f, new Vector3(18f, -1.4f, 40)));
    }

    public void StartGoldenFoodSpawn()
    {
        CoroutineHelper.StartCoroutine(_goldenFoodSpawner.SpawnCoroutine(30f, 40f, new Vector3(30f, -1.2f, 40)));
    }

    /// <summary>
    /// 점수 업데이트 시작
    /// </summary>
    public void StartScoreUpdate()
    {
        CoroutineHelper.StartCoroutine(UpdateScoreCoroutine());
    }
}
