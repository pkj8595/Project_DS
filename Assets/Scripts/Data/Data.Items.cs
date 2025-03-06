using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase
{
    Data.TableBase tableBase;
    public int GetTableNum => tableBase.tableNum;
    public virtual string Name { get; }
    public virtual string Desc { get; }
    public virtual string ImgStr { get; }
    public int Amount { get; set; }
    public TableBase TableBase { get => tableBase; set => tableBase = value; }

    protected virtual void Init(Data.TableBase tableBase)
    {

    }

    public ItemBase(int tableNum)
    {
        switch (Utils.CalculateTableNum(tableNum))
        {
            case Data.CharacterData.Table:
                tableBase = Managers.Data.CharacterDict[tableNum];
                break;
            case Data.TileBaseData.Table:
                tableBase = Managers.Data.TileBaseDict[tableNum];
                break;
            case Data.BuildingData.Table:
                tableBase = Managers.Data.BuildingDict[tableNum];
                break;
            case Data.RuneData.Table:
                tableBase = Managers.Data.RuneDict[tableNum];
                break;
            case Data.GoodsData.Table:
                tableBase = Managers.Data.GoodsDict[tableNum];
                break;
            case Data.ShopData.Table:
                tableBase = Managers.Data.ShopDict[tableNum];
                break;
            default:
                Debug.LogError($"{tableNum} 식별되지 않은 케이스");
                break;
        }
        Init(tableBase);
    }

    public static ItemBase GetItem(int tableNum)
    {
        ItemBase ret = null;
        switch (Utils.CalculateTableNum(tableNum))
        {
            case Data.CharacterData.Table:
                ret = new CharacterItem(tableNum);
                break;
            case Data.TileBaseData.Table:
                ret = new TileItem(tableNum);
                break;
            case Data.BuildingData.Table:
                ret = new BuildingItem(tableNum);
                break;
            case Data.RuneData.Table:
                ret = new RuneItem(tableNum);
                break;
            case Data.GoodsData.Table:
                ret = new GoodsItem(tableNum);
                break;
            case Data.ShopData.Table:
                ret = new ShopItem(tableNum);
                break;
            default:
                Debug.LogError($"{tableNum} 식별되지 않은 케이스");
                break;
        }
        return ret;
    }
}




public class CharacterItem : ItemBase
{
    private Data.CharacterData _charData;

    public override string Name => _charData.name;
    public override string Desc => _charData.desc;
    //public override string ImgStr => _charData.imageStr;

    protected override void Init(Data.TableBase tableBase)
    {
        base.Init(tableBase);
        _charData = tableBase as Data.CharacterData;
    }
    public CharacterItem(int tableNum) : base(tableNum) { }

}

public class TileItem : ItemBase
{
    private Data.TileBaseData _data;

    public override string Name => _data.name;
    public override string Desc => _data.desc;
    //public override string ImgStr => _charData.imageStr;

    protected override void Init(Data.TableBase tableBase)
    {
        base.Init(tableBase);
        _data = tableBase as Data.TileBaseData;
    }
    public TileItem(int tableNum) : base(tableNum) { }

}


public class BuildingItem : ItemBase
{
    private Data.BuildingData _data;

    public override string Name => _data.name;
    public override string Desc => _data.desc;
    //public override string ImgStr => _charData.imageStr;

    protected override void Init(Data.TableBase tableBase)
    {
        base.Init(tableBase);
        _data = tableBase as Data.BuildingData;
    }
    public BuildingItem(int tableNum) : base(tableNum) { }

}

public class RuneItem : ItemBase
{
    private Data.RuneData _runeData;

    public override string Name => _runeData.name;
    public override string Desc => _runeData.desc; 
    public override string ImgStr => _runeData.imageStr;

    protected override void Init(Data.TableBase tableBase)
    {
        base.Init(tableBase);
        _runeData = tableBase as Data.RuneData;
    }
    public RuneItem(int tableNum) : base(tableNum) { }

}


public class GoodsItem : ItemBase
{

    private Data.GoodsData _goodsData;
    public override string Name => _goodsData.name;
    public override string Desc => _goodsData.desc;
    public override string ImgStr => _goodsData.imageStr;

    protected override void Init(TableBase tableBase)
    {
        base.Init(tableBase);
        _goodsData = tableBase as GoodsData;
    }
    public GoodsItem(int tableNum) : base(tableNum) { }

}


public class ShopItem : ItemBase
{

    private Data.ShopData _goodsData;
    public override string Name => _goodsData.name;
    public override string Desc => _goodsData.desc;
    public override string ImgStr => GetImg();

    protected override void Init(TableBase tableBase)
    {
        base.Init(tableBase);
        _goodsData = tableBase as ShopData;
    }
    public ShopItem(int tableNum) : base(tableNum) { }

    private string GetImg()
    {
        string ret = string.Empty;
        switch (Utils.CalculateTableNum(_goodsData.minTableRange))
        {
            case Data.CharacterData.Table:
                ret = "IconRandomPawn";
                break;
            case Data.BuildingData.Table:
                ret = "IconRandomBuilding";
                break;
            case Data.RuneData.Table:
                ret = "IconRandomRune";
                break;
        }
        return ret;
    }

}