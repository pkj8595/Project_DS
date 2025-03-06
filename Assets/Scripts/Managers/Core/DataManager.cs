using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataManager : ManagerBase
{
    public Dictionary<int, Data.CharacterData> CharacterDict { get; private set; } = new ();
    public Dictionary<int, Data.StatData> StatDict { get; private set; } = new ();
    public List<Data.StatData> PositivePropertyList { get; private set; } = new ();
    public List<Data.StatData> NagativePropertyList { get; private set; } = new ();
    public Dictionary<int, Data.StatConversionData> StatConversionDict { get; private set; } = new ();
    public Dictionary<int, Data.WaveData> WaveDict { get; private set; } = new ();
    public Dictionary<int, Data.BuildingData> BuildingDict { get; private set; } = new ();
    public Dictionary<int, Data.GoodsData> GoodsDict { get; private set; } = new ();
    public Dictionary<int, Data.RuneData> RuneDict { get; private set; } = new ();
    public Dictionary<int, Data.SkillData> SkillDict { get; private set; } = new ();
    public Dictionary<int, Data.SkillAffectData> SkillAffectDict { get; private set; } = new ();
    public Dictionary<int, Data.BuffData> BuffDict { get; private set; } = new ();
    public Dictionary<int, Data.ProductionData> ProductionDict { get; private set; } = new ();


    public override void Init()
    {
        base.Init();
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/DefenceTable");
        Data.TableGroupData tableGroupData = JsonUtility.FromJson<Data.TableGroupData>(textAsset.text);

        tableGroupData.MakeTableData(CharacterDict);
        tableGroupData.MakeTableData(StatDict, PositivePropertyList, NagativePropertyList);
        tableGroupData.MakeTableData(StatConversionDict);
        tableGroupData.MakeTableData(WaveDict);
        tableGroupData.MakeTableData(BuildingDict);
        tableGroupData.MakeTableData(GoodsDict);
        tableGroupData.MakeTableData(RuneDict);
        tableGroupData.MakeTableData(SkillDict);
        tableGroupData.MakeTableData(SkillAffectDict);
        tableGroupData.MakeTableData(BuffDict);
        tableGroupData.MakeTableData(ProductionDict);

    }

    public Data.TableBase GetTableData(int tableNum)
    {
        Data.TableBase ret = null;
        switch (Utils.CalculateTableNum(tableNum))
        {
            case Data.CharacterData.Table:
                Data.CharacterData characterTable;
                CharacterDict.TryGetValue(tableNum,out characterTable);
                ret = characterTable;
                break;
            case Data.StatData.Table:
                Data.StatData statTable;
                StatDict.TryGetValue(tableNum, out statTable);
                ret = statTable;
                break;
            case Data.StatConversionData.Table:
                Data.StatConversionData statConversionTable;
                StatConversionDict.TryGetValue(tableNum, out statConversionTable);
                ret = statConversionTable;
                break;
            
            case Data.BuildingData.Table:
                Data.BuildingData buildingTable;
                BuildingDict.TryGetValue(tableNum, out buildingTable);
                ret = buildingTable;
                break;
            case Data.GoodsData.Table:
                Data.GoodsData goodsTable;
                GoodsDict.TryGetValue(tableNum, out goodsTable);
                ret = goodsTable;
                break;
            case Data.RuneData.Table:
                Data.RuneData itemTable;
                RuneDict.TryGetValue(tableNum, out itemTable);
                ret = itemTable;
                break;
            case Data.SkillData.Table:
                Data.SkillData skillTable;
                SkillDict.TryGetValue(tableNum, out skillTable);
                ret = skillTable;
                break;
            case Data.SkillAffectData.Table:
                Data.SkillAffectData skillAffectTable;
                SkillAffectDict.TryGetValue(tableNum, out skillAffectTable);
                ret = skillAffectTable;
                break;
        }

        if(ret == null)
            Debug.LogError($"{tableNum} 데이터를 가져오는데 실패했습니다.");

        return ret;
    }

}
