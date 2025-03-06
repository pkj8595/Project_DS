using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupShop : UIPopup
{
    public List<UIShopCard> _shopCardList;

    public override void Init(UIData uiData)
    {
        base.Init(uiData);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();

        List<Data.ShopData> shopDataList = new List<Data.ShopData>();
        var iter = Managers.Data.ShopDict.GetEnumerator();
        while (iter.MoveNext()) 
        {
            shopDataList.Add(iter.Current.Value);
        }

        for (int i = 0; i < 12; i++)
        {
            int randomIndex = Random.Range(0, shopDataList.Count);
            _shopCardList[i].Init(shopDataList[randomIndex]);
        }


    }



}
