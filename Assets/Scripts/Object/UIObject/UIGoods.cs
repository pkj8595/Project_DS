using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIGoods : MonoBehaviour
{
    public Image _imgGoods;
    public Define.EGoodsType goodsType;
    public Text _txtAmount;

    public void Init(int goodsNum)
    {
        var goods = Managers.Data.GoodsDict[goodsNum];
        _imgGoods.sprite = Managers.Resource.Load<Sprite>($"Sprites/UI/Icon/{goods.imageStr}");
        _txtAmount.text = Managers.Game.Inven.GetItem(goodsNum).ToString();
        Managers.Game.Inven.OnItemAmountChanged -= UpdateAmount;
        Managers.Game.Inven.OnItemAmountChanged += UpdateAmount;
    }

    public void UpdateAmount(int itemNum)
    {
        if (itemNum == goodsType.ToInt())
            _txtAmount.text = Managers.Game.Inven.GetItem(goodsType.ToInt()).ToString();
    }

    public void OnEnable()
    {
        Init((int)goodsType);
    }

    public void OnDisable()
    {
        Managers.Game.Inven.OnItemAmountChanged -= UpdateAmount;
    }

}
