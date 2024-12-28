using UnityEngine;
using static Define;

[DefaultExecutionOrder(0)]
public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일한 인스턴스를 담을 변수.
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 인스턴스를 참조하는 property

    private PoolManager _poolManager = new PoolManager();
    private UIManager _uiManager = new UIManager();
    private ResourceManger _resourceManger = new ResourceManger();
    private SceneManagerEx _sceneManager = new SceneManagerEx();
    private GameManager _game = new GameManager();
    private AudioManager _audio = new AudioManager();

    public static PoolManager Pool => Instance._poolManager;
    public static UIManager UI => Instance._uiManager;
    public static ResourceManger Resource => Instance._resourceManger;
    public static SceneManagerEx Scene => Instance._sceneManager;
    public static GameManager Game => Instance._game;
    public static AudioManager Audio => Instance._audio;

    void Start()
    {
        Init();
    }


    static void Init()
    {
        // s_instance가 null일 때만 Managers를 찾아 Instance에 할당
        if (s_instance != null) return;

        GameObject go = GameObject.Find("@Managers");
        if (go == null)
        {
            go = new GameObject { name = "@Managers" };
            go.AddComponent<Managers>();
        }
        DontDestroyOnLoad(go);
        s_instance = go.GetComponent<Managers>();

        
    }

    /// <summary>
    /// Scene을 이동할 때 호출해야 하는 함수.
    /// </summary>
    public static void Clear()
    {
        Audio.Clear();

        UI.Clear();
        Scene.Clear();

        Pool.Clear();       // 의도적으로 마지막에 Clear. 왜? 다른 Manager에서 pool 오브젝트를 사용할 수 있기 때문.
    }

    private void OnApplicationQuit()
    {
        
    }
}
