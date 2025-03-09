using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Managers : MonoBehaviour
{
    static Managers s_Instance;
    static Managers Instance { get { InitManagers(); return s_Instance; } }

    #region Contens
    GameManager _game = new GameManager();
    
    public static GameManager Game { get { return Instance._game; } }
    #endregion
    
    #region Core
    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();
    EffectManager _effect = new EffectManager();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static EffectManager Effect { get => Instance._effect; }
    #endregion

    static bool _isFirstInit = false;

    private void Awake()
    {
        
    }

    void Start()
    {

    }

    void Update()
    {
        _input.OnUpdate();
    }

    public static void InitManagers()
    {
        if(s_Instance == null && !_isFirstInit)
        {
            _isFirstInit = true;
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
            }
            DontDestroyOnLoad(go);
            s_Instance = go.GetOrAddComponent<Managers>();

            s_Instance._scene.Init();
            s_Instance._data.Init();
            s_Instance._resource.Init();
            s_Instance._pool.Init();
            s_Instance._sound.Init();
            s_Instance._input.Init(go);
            s_Instance._ui.Init(go);
            s_Instance._effect.Init();
            s_Instance._game.Init();

            DOTween.Init(true, true, LogBehaviour.Default).SetCapacity(300,40);
            //Application.targetFrameRate = 30;
        }
    }

    public static void Clear()
    {
        Scene.Clear();
        Data.Clear();
        Resource.Clear();
        Input.Clear();
        Sound.Clear();
        UI.Clear();
        Pool.Clear();
        Effect.Clear();
        UI.Clear();
        Effect.Clear();
    }

    
}
