using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShopCard : MonoBehaviour
{
    public UICard _uiCard;
    public List<UIItem> _goodsList;
    private Data.ShopData _data;
    public GameObject _soldOut;

    public void Init(Data.ShopData data)
    {
        _data = data;
        _uiCard.Init(data, null);
        for (int i = 0; i < _goodsList.Count; i++)
        {
            if(data.arr_goods[i] != 0)
            {
                _goodsList[i].Init(data.arr_goods[i], data.arr_goods_amount[i]);
                _goodsList[i].gameObject.SetActive(true);
            }
            else
                _goodsList[i].gameObject.SetActive(false);
        }
        _soldOut.SetActive(false);
    }

    public void OnClickPurchase()
    {
        bool isPurchaseable = true;
        for (int i = 0; i < _data.arr_goods.Length; i++)
        {
            if(_data.arr_goods[i] != 0)
            {
                if (!Managers.Game.Inven.CheckItem(_data.arr_goods[i], _data.arr_goods_amount[i]))
                {
                    isPurchaseable = false;
                }
            }
        }

        if (isPurchaseable)
        {
            for (int i = 0; i < _data.arr_goods.Length; i++)
            {
                if (_data.arr_goods[i] != 0)
                {
                    Managers.Game.Inven.SpendItem(_data.arr_goods[i], _data.arr_goods_amount[i]);
                }
            }

            Managers.Game.Inven.AddCard(_data);
            Managers.UI.ShowToastMessage("구매 성공!");
            _soldOut.SetActive(true);
        }
        else
        {
           Managers.UI.ShowToastMessage("재화가 부족합니다.");
        }
    }
}
