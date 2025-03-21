using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    readonly ResourceManager _resource = new();
    readonly EffectManager _effect = new();
    readonly SoundManager _sound = new();
    readonly DataManager _data = new();
    readonly UIManager _ui = new();
    public static DataManager Data { get => Instance._data; }
    public static ResourceManager Resource { get => Instance._resource; }
    public static SoundManager Sound { get => Instance._sound; }
    public static UIManager UI { get => Instance._ui; }
    public static EffectManager Effect { get => Instance._effect; }

    //network manager
    private ObjectManager _object;
    private MapManager _map;
    public static ObjectManager Object { get => Instance._object; }
    public static MapManager Map { get => Instance._map; }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitManagers();
    }

    void Start()
    {

    }

    void Update()
    {
    }

    private void InitManagers()
    {
        _data.Init();
        _resource.Init();
        _sound.Init();
        _ui.Init(gameObject);
        _effect.Init();

        DOTween.Init(true, true, LogBehaviour.Default).SetCapacity(300,40);

        _object = GetComponentInChildren<ObjectManager>();
        _map = GetComponentInChildren<MapManager>();
    }

    public static void Clear()
    {
        Data.Clear();
        Resource.Clear();
        Sound.Clear();
        UI.Clear();
        Effect.Clear();
    }

    
}
