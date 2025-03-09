using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Data;

public class Inventory 
{
    private readonly Dictionary<int, int> _itemDic = new Dictionary<int, int>();
    public Action<int> OnItemAmountChanged { get; set; }

    private UIMain uiMain;

    private int _currentPopulation;
    private int _maxPopulation;
    public int MaxPopulation 
    {   
        get => _maxPopulation; 
        set 
        {
            _maxPopulation = value;
            UpdatePopulation(); 
        } 
    }
    public int CurrentPopulation
    {
        get => _currentPopulation; 
        set
        {
            _currentPopulation = value;
            UpdatePopulation();
        }
    }

    public void UpdatePopulation()
    {

        string ret = $"{CurrentPopulation}/{MaxPopulation}";

        if (uiMain == null)
            uiMain = Managers.UI.GetUI<UIMain>() as UIMain;
    }



    public void Init()
    {
        foreach (var data in Managers.Data.GoodsDict)
        {
            _itemDic.Add(data.Key, 100);
        }
    }

    public void Clear()
    {
        _itemDic.Clear();
    }

    public void AddItem(int itemNum, int amount)
    {
        if (!_itemDic.ContainsKey(itemNum))
        {
            _itemDic.Add(itemNum, 0);
            return;
        }

        _itemDic[itemNum] += amount;
        OnItemAmountChanged?.Invoke(itemNum);
    }

    public int GetItem(int itemNum)
    {
        if (!_itemDic.ContainsKey(itemNum))
        {
            _itemDic.Add(itemNum, 0);
        }

        return _itemDic[itemNum];
    }

    /// <summary>
    /// 아이템이 있는지 체크
    /// </summary>
    /// <param name="itemNum"></param>
    /// <param name="itemAmount"></param>
    /// <returns>아이템의 수량이 충분하면 true</returns>
    public bool CheckItem(int itemNum, int itemAmount)
    {
        if (_itemDic.ContainsKey(itemNum))
        {
            if (itemAmount <= _itemDic[itemNum])
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 아이템이 있는지 체크 후 소모
    /// </summary>
    /// <param name="itemNum"></param>
    /// <param name="itemAmount"></param>
    /// <returns>아이템과 수량이 충분하면 소모하다 true 리턴</returns>
    public bool SpendItem(int itemNum, int itemAmount)
    {
        if (CheckItem(itemNum, itemAmount))
        {
            _itemDic[itemNum] -= itemAmount;
            OnItemAmountChanged?.Invoke(itemNum);
            return true;
        }
        return false;
    }

    public void SpendMoveItem(Transform transform, Vector3 offset, Define.EGoodsType goodsType, int itemAmount, Action<bool> action)
    {
        var uimain = Managers.UI.GetUI<UIMain>() as UIMain;
        if (uimain != null)
            uimain.UIMoveResource.QueueSpendItem(transform, offset, goodsType, itemAmount, action);
        else
        {
            bool isAble = SpendItem(goodsType.ToInt(), itemAmount);
            action?.Invoke(isAble);
        } 
    }

    public void AddMoveItem(Vector3 worldPosition, Define.EGoodsType goodsType, int amount)
    {
        var uimain = Managers.UI.GetUI<UIMain>() as UIMain;

        if (uimain != null)
            uimain.UIMoveResource.QueueAddItem(worldPosition, goodsType, amount);
        else
            AddItem(goodsType.ToInt(), amount);
    }
    


}
