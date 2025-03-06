using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : Unit, ISelectedable, IWaveEvent
{
    [SerializeField] public GameObject _model;
    [SerializeField] private Collider _collider;
    [field: SerializeField] public Vector3 StateBarOffset { get; set; }
    [field: SerializeField] public Define.ETeam Team { get; set; } = Define.ETeam.Playable;
    public Define.WorldObject WorldObjectType { get; set; } = Define.WorldObject.Building;
    public Data.BuildingData BuildingData { get; private set; }
    protected BuildingProduction _production;
    protected BuildingDamageable _damageable;
    public BuildingStat Stat { get; private set; }
    public BuildingSkill Skill { get; protected set; }
    public Collider Collider { get => _collider; set => _collider = value; }
    public bool IsSelected { get; set; }

    public int BuildingTableNum;

    public void Init()
    {
        Init(BuildingTableNum);
    }

    public virtual void Init(int tableNum)
    {
        Init(Managers.Data.BuildingDict[tableNum]);
    }

    public virtual void Init(Data.BuildingData data)
    {
        if (BuildingData != null)
        {
            Managers.Game.Inven.MaxPopulation -= BuildingData.popluation;
        }

        //todo 건물 테이블 수정하고 구현
        BuildingData = data;
        if (Stat == null)
            Stat = gameObject.GetOrAddComponent<BuildingStat>();
        Stat.Init(data.tableNum, OnDead, OnChagneStatValue, OnDeadTarget);

        if (BuildingData.productionTable != 0)
        {
            _production = gameObject.GetOrAddComponent<BuildingProduction>();
            _production.Init(data.productionTable, this);
        }
        else
        {
            DestroyComponent<BuildingProduction>();
        }

        if (BuildingData.baseSkill != 0)
        {
            Skill = gameObject.GetOrAddComponent<BuildingSkill>();
            Skill.Init(this);
        }
        else
        {
            DestroyComponent<BuildingSkill>();
        }

        if (data.isDamageable)
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();
            _damageable = gameObject.GetOrAddComponent<BuildingDamageable>();
            _damageable.Init(this);

            UIStateBarGroup uiStatebarGroup = Managers.UI.ShowUI<UIStateBarGroup>() as UIStateBarGroup;
            uiStatebarGroup.AddUnit(_damageable);
        }
        else
        {
            var skill = GetComponent<BuildingDamageable>();
            if (skill != null)
            {
                UIStateBarGroup uiStatebarGroup = Managers.UI.ShowUI<UIStateBarGroup>() as UIStateBarGroup;
                uiStatebarGroup.RemoveUnit(_damageable);
                Destroy(skill);
            }
        }

        Managers.Game.Inven.MaxPopulation += BuildingData.popluation;

    }


    private void DestroyComponent<T>() where T : MonoBehaviour
    {
        var skill = GetComponent<T>();
        if (skill != null)
        {
            Destroy(skill);
        }
    }

    public override bool UpgradeUnit()
    {
        if (BuildingData.upgradeNum == 0 || Team == Define.ETeam.Enemy)
        {
            Managers.UI.ShowToastMessage("적은 업그레이드 할 수 없습니다.");
            return false;
        }

        if (Managers.Game.Inven.SpendItem(BuildingData.upgrade_goods, BuildingData.upgrade_goods_amount))
        {
            Init(BuildingData.upgradeNum);
            return true;
        }

        Managers.UI.ShowToastMessage("업그레이드 비용이 부족합니다.");
        return false;
    }

    private void OnChagneStatValue()
    {

    }
   
    public void OnDeadTarget()
    {

    }

    private void OnDestroy()
    {
        if (_damageable != null)
        {
            UIStateBarGroup uiStatebarGroup = Managers.UI.GetUI<UIStateBarGroup>() as UIStateBarGroup;
            uiStatebarGroup.RemoveUnit(_damageable);
        }

    }

    public void OnDead()
    {
        if (_damageable != null)
        {
            UIStateBarGroup uiStatebarGroup = Managers.UI.ShowUI<UIStateBarGroup>() as UIStateBarGroup;
            uiStatebarGroup.SetActive(_damageable, false);
        }

        Managers.Effect.PlayAniEffect("SmallStingHit", transform.position);
        gameObject.SetActive(false);
        if (gameObject.name == "GodTower")
        {
            Debug.Log("<color=red>타워가 파괴되었습니다. 게임오버</color>");
        }

        BoardManager.Instance.RemoveNode(GetComponent<NodeBase>());
    }

    #region ISelectable
    public void OnSelect()
    {
        IsSelected = true;
        UIData data = new UIUnitData { unitGameObject = this };
        Managers.UI.ShowUIPopup<UIUnitPopup>(data);
    }

    public void OnDeSelect()
    {
        IsSelected = false;
    }

    public void EndWave()
    {
        if (_production != null)
        {
            _production.EndWave();
        }

        if (Skill != null)
        {
            Skill.EndWave();
        }

        Managers.Game.Inven.SpendMoveItem(transform, StateBarOffset, Define.EGoodsType.gold, BuildingData.waveCost, (isSpend) =>
        {
            if (isSpend)
                Stat.SpendCost();
            else
                Stat.DontSpendCost();
        });
    }

    public void ReadyWave()
    {
        Stat.Mana = 0;

        if (_production != null)
        {
            _production.ReadyWave();
        }
    }
    #endregion
    private void OnEnable()
    {
        Managers.Game.RegisterWaveObject(this);
        
    }

    private void OnDisable()
    {
        Managers.Game.RemoveWaveObject(this);
        if (BuildingData != null)
        {
            Managers.Game.Inven.MaxPopulation -= BuildingData.popluation;
        }
    }
}
