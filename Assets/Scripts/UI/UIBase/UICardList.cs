using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UICardList : UIBase
{
    private List<Data.ShopData> _cardDataList => Managers.Game.Inven.GetCardDataList;
    [SerializeField] private Transform _cardListParent;
    [SerializeField] private List<UICard> _instantiatedCardList = new();
    
    [SerializeField] private Stack<UICard> _stackCardPool = new();

    [Header("prefab")]
    [SerializeField] private UICard _cardPrefab;

    [Header("sort setting")]
    public float cardSpacing = 20f;  // 카드 간격
    public float cardWidth = 230f;   // 카드의 너비
    public float moveDuration = 0.5f; // 카드 이동 시간

    public override void Init(UIData uiData)
    {
        base.Init(uiData);
        
    }
    
    public override void UpdateUI()
    {
        base.UpdateUI();

        
    }
    public override void Close()
    {
        base.Close();
    }

    public void AddCard(Data.ShopData data)
    {
        //pool에서 가져옴
        UICard card = GetUICardPool();
        card.Init(data, RemoveCard);
        card.gameObject.SetActive(true);
        card.Rect.anchoredPosition = new Vector2(Screen.width, 0);
        // 카드의 위치를 다시 정렬
        ArrangeCards();
    }

    public UICard GetUICardPool()
    {
        UICard card;
        if (0 < _stackCardPool.Count)
            card = _stackCardPool.Pop();
        else
            card = Instantiate(_cardPrefab, _cardListParent);

        card.gameObject.SetActive(true);
        _instantiatedCardList.Add(card);
        return card;
    }

    public void RemoveCard(UICard card)
    {
        card.Clear();
        card.gameObject.SetActive(false);
        _stackCardPool.Push(card);
        _instantiatedCardList.Remove(card);

        // 카드의 위치를 다시 정렬
        ArrangeCards();
    }

    // 카드 위치를 중앙 기준으로 재배치하는 함수
    public void ArrangeCards()
    {
        int cardCount = _instantiatedCardList.Count;
        if (cardCount == 0) return;

        // 카드의 중앙 정렬을 위해 시작 위치 계산
        float totalWidth = ((cardWidth + cardSpacing) * 0.5f) * (cardCount - 1);
        float startX = (Screen.width / 2) - totalWidth;

        for (int i = 0; i < cardCount; i++)
        {
            UICard card = _instantiatedCardList[i];

            // 카드의 목표 위치 계산
            float targetX = startX + i * (cardWidth + cardSpacing);

            // DOTween을 사용해 카드의 위치를 이동
            card.Rect.DOAnchorPos(new Vector2(targetX, 0), moveDuration).SetEase(Ease.OutQuad);

            // 카드 크기 조정 (카드가 많아지면 크기를 줄임)
            //float scale = Mathf.Clamp(1f - 0.05f * (cardCount - 1), 0.5f, 1f);
            //card.Rect.DOScale(scale, moveDuration).SetEase(Ease.OutQuad);
        }
    }
}
