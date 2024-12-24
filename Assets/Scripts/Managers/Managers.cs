using UnityEngine;

[DefaultExecutionOrder(0)]
public class Managers : MonoBehaviour
{
    static Managers s_instance; // ������ �ν��Ͻ��� ���� ����.
    static Managers Instance { get { Init(); return s_instance; } } // ������ �ν��Ͻ��� �����ϴ� property

    private PoolManager _poolManager = new PoolManager();
    private UIManager _uiManager = new UIManager();
    private ResourceManger _resourceManger = new ResourceManger();
    private SceneManagerEx _sceneManager = new SceneManagerEx();
    private GameManager _game = new GameManager();

    public static PoolManager Pool => Instance._poolManager;
    public static UIManager UI => Instance._uiManager;
    public static ResourceManger Resource => Instance._resourceManger;
    public static SceneManagerEx Scene => Instance._sceneManager;
    public static GameManager Game => Instance._game;

    void Start()
    {
        Init();
    }


    static void Init()
    {
        // s_instance�� null�� ���� Managers�� ã�� Instance�� �Ҵ�
        if (s_instance != null) return;

        GameObject go = GameObject.Find("@Managers");
        if (go == null)
        {
            go = new GameObject { name = "@Managers" };
            go.AddComponent<Managers>();
        }
        DontDestroyOnLoad(go);
        s_instance = go.GetComponent<Managers>();

        s_instance._game.Init();
    }

    /// <summary>
    /// Scene�� �̵��� �� ȣ���ؾ� �ϴ� �Լ�.
    /// </summary>
    public static void Clear()
    {

    }

    private void OnApplicationQuit()
    {
        
    }
}
