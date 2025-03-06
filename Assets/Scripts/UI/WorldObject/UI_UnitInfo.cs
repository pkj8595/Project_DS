using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_UnitInfo : MonoBehaviour
{
    [SerializeField] private UI_ToolTip _uiToolTip;

    [SerializeField] private List<GameObject> _title;
    [SerializeField] private Image _unitIcon;
    [SerializeField] private Image _pawnIcon;
    [SerializeField] private Text _unitName;
    [SerializeField] private Text _unitDesc;
    [SerializeField] private List<Text> _stat;
    [SerializeField] private List<UI_PropertySlot> _propertyName;
    [SerializeField] private List<UI_SkillSlot> _skill;
    //[SerializeField] private List<UI_ImageText> _rune;
    [SerializeField] private List<UI_RuneSlot> _rune;
    [SerializeField] private UI_ImageText _upgrade;
    [SerializeField] private Text _ignoreAttribute;
    [SerializeField] private UIItem _costAmount;

    public void SetPawnBase(PawnBase pawn)
    {
        SetDescData(pawn);
        SetStatData(pawn.PawnStat);
        SetProperty(pawn.PawnStat);
        SetSkillData(pawn.PawnSkills.SkillList);
        SetRuneData(pawn);
        SetEtcData(pawn);
        SetCostItem(pawn);

        foreach (var item in _title)
        {
            item.SetActive(true);
        }
        _pawnIcon.gameObject.SetActive(true);
        _unitIcon.gameObject.SetActive(false);
    }

    public void SetBuildingBase(BuildingBase buildingBase)
    {
        SetDescData(buildingBase.BuildingData);
        SetStatData(buildingBase.Stat);
        //SetProperty(buildingBase.PawnStat);
        SetSkillData(buildingBase.Skill.Skills.SkillList);
        //SetRuneData(buildingBase.RuneList);
        SetEtcData(buildingBase);
        SetCostItem(buildingBase);

        foreach (var item in _title)
        {
            item.SetActive(false);
        }
        _pawnIcon.gameObject.SetActive(false);
        _unitIcon.gameObject.SetActive(true);
    }

    private void SetDescData(PawnBase data)
    {
        _pawnIcon.sprite = data.GetIdleSprite();
        _unitName.text = data.CharacterData.name;
        _unitDesc.text = data.CharacterData.desc;
    }
    private void SetDescData(Data.BuildingData data)
    {
        _unitName.text = data.name;
        _unitDesc.text = data.desc;
    }

    private void SetStatData(Stat stat)
    {
        if (stat is PawnStat)
        {
            var data = stat as PawnStat;
            _stat[0].text = $"vitality :{data.CurrentBaseStat.vitality}";
            _stat[1].text = $"strength :{data.CurrentBaseStat.strength}";
            _stat[2].text = $"agility :{data.CurrentBaseStat.agility}";
            _stat[3].text = $"intelligence :{data.CurrentBaseStat.intelligence}";
            _stat[4].text = $"willpower :{data.CurrentBaseStat.willpower}";
            _stat[5].text = $"accuracy :{data.CurrentBaseStat.accuracy}";

            _stat[5].gameObject.SetActive(true);
        }
        else
        {
            var data = stat as BuildingStat;
            _stat[0].text = $"Hp :{data.Hp}/{data.MaxHp}";
            _stat[1].text = $"Mana :{data.Mana}/{data.MaxMana}";
            _stat[2].text = $"Damage :{data.DamageValue}";
            _stat[3].text = $"Protection :{data.Protection}";
            _stat[4].text = $"ManaRegen :{data.ManaRegeneration}";

            _stat[5].gameObject.SetActive(false);
        }
    }

    private void SetProperty(PawnStat stat)
    {
        for (int i = 0; i < _propertyName.Count; i++)
        {
            if (i < stat.PropertyStatList.Count)
            {
                _propertyName[i].Init(stat.PropertyStatList[i]);
                _propertyName[i].SetToolTip(_uiToolTip);
                _propertyName[i].TxtProperty.text = stat.PropertyStatList[i].name;
            }
            else
                _propertyName[i].gameObject.SetActive(false);
        }
    }

    private void SetSkillData(List<Skill> datas)
    {
        for (int i = 0; i < _skill.Count; i++)
        {
            if (i < datas.Count)
            {
                _skill[i].SetToolTip(_uiToolTip);
                _skill[i].Init(datas[i]);
                _skill[i].gameObject.SetActive(true);
            }
            else
            {
                _skill[i].gameObject.SetActive(false);
            }
        }
        
    }

    private void SetRuneData(PawnBase pawn)
    {
        for (int i = 0; i < _rune.Count; i++)
        {
            _rune[i].SetToolTip(_uiToolTip);
            _rune[i].Init(pawn, i);
        }
    }
  
    private void SetEtcData(PawnBase pawnBase)
    {
        if (pawnBase.CharacterData.upgradeChar != 0)
        {
            ItemBase itemBase = ItemBase.GetItem(pawnBase.CharacterData.upgradeRequire);
            Data.CharacterData data = Managers.Data.CharacterDict[pawnBase.CharacterData.upgradeChar];
            _upgrade.Name.text = $"Upgrade {data.name}\n{itemBase.Name} : {pawnBase.CharacterData.upgradeRequireAmount}";
            _upgrade.Icon.gameObject.SetActive(true);
        }
        else
        {
            _upgrade.Icon.gameObject.SetActive(false);
            _upgrade.Name.text = string.Empty;
        }

        _ignoreAttribute.text =$"Ignore Att : {pawnBase.CharacterData.ignoreAttributeType.ToStr()}" ;
    }

    private void SetEtcData(BuildingBase building)
    {
        if (building.BuildingData.upgradeNum != 0)
        {
            ItemBase itemBase = ItemBase.GetItem(building.BuildingData.upgrade_goods);
            var data = Managers.Data.BuildingDict[building.BuildingData.upgradeNum];
            _upgrade.Name.text = $"{data.name}\n{itemBase.Name} : {building.BuildingData.upgrade_goods_amount}";
            _upgrade.Icon.gameObject.SetActive(true);
        }
        else
        {
            _upgrade.Icon.gameObject.SetActive(false);
            _upgrade.Name.text = string.Empty;
        }
    }

    private void SetCostItem(Unit unit)
    {
        if (unit is PawnBase)
        {
            PawnBase temp = unit as PawnBase;
            _costAmount.Init(Define.EGoodsType.food.ToInt(), temp.CharacterData.waveCost);
        }
        else
        {
            BuildingBase temp = unit as BuildingBase;
            _costAmount.Init(Define.EGoodsType.gold.ToInt(), temp.BuildingData.waveCost);
        }
        
    }


}

[Serializable]
public class UI_ImageText 
{
    public GameObject Obj;
    public Image Icon;
    public Text Name;
}
