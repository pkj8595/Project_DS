using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class UICard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private static bool _isAlreadyStart = false;
    private Action<UICard> _OnUseAction;
    private ItemBase _item;
    private Vector3 originalPosition;
    private Vector3 startDragPosition;
    private float originalScale;
    private int originSortingOrder;
    private bool isDragging = false;

    [SerializeField] private RectTransform _rect;
    public Text _txtTitle;
    public Text _txtDesc;
    public Image _imgMain;
    public Image _imgBack;
    public CanvasGroup canvasGroup;
    public Image _imgCopyMain;

    public RectTransform Rect => _rect;
    public ItemBase Item { get => _item; set => _item = value; }

    private void Start()
    {
        originalPosition = _rect.anchoredPosition;
        originalScale = _rect.localScale.x;
    }

    public void Init(Data.ShopData data, Action<UICard> onUseAction)
    {
        _rect.localScale = Vector2.one;
        _txtTitle.text = data.name;
        _txtDesc.text = data.desc;
        _OnUseAction = onUseAction;

        if (data.minTableRange == data.maxTableRange)
        {
            _item = ItemBase.GetItem(data.minTableRange);
        }
        else
        {
            _item = ItemBase.GetItem(data.tableNum);
        }
        _imgMain.sprite = Managers.Resource.Load<Sprite>(Define.Path.UIIcon + _item.ImgStr);
        _imgCopyMain.sprite = _imgMain.sprite;
        originSortingOrder = _imgCopyMain.canvas.sortingOrder;
    }
 

    public void Clear()
    {
        _txtTitle.text = string.Empty;
        _txtDesc.text = string.Empty;
    }

    public void OnClickCard()
    {
        //_OnUseAction?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isAlreadyStart)
            return;

        _rect.DOAnchorPosY(originalPosition.y + 30f, 0.2f).SetEase(Ease.OutQuad);
        _rect.DOScale(originalScale + 0.1f, 0.2f).SetEase(Ease.OutQuad);

        if (isDragging && Input.GetMouseButton(0))
        {
            CancelDrag();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isAlreadyStart)
            return;

        _rect.DOAnchorPosY(originalPosition.y, 0.2f).SetEase(Ease.OutQuad);
        _rect.DOScale(originalScale, 0.2f).SetEase(Ease.OutQuad);

        if (!isDragging && Input.GetMouseButton(0))
        {
            StartDrag();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            switch (_item)
            {
                case CharacterItem:
                    _imgCopyMain.transform.position = eventData.position;
                    break;
                case BuildingItem:
                case TileItem:
                    break;
                case RuneItem:
                    _imgCopyMain.transform.position = eventData.position;
                    break;
                case ShopItem:
                    break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            EndDrag(eventData);
        }
    }

   
    public void UseComplete(bool isSuccess)
    {
        if(isSuccess)
            _OnUseAction?.Invoke(this);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        _isAlreadyStart = false;
        ResetCopyImage();
    }

    private void StartDrag()
    {
        _isAlreadyStart = true;
        isDragging = true;
        startDragPosition = _rect.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        switch (_item)
        {
            case CharacterItem:
                _imgCopyMain.gameObject.SetActive(true);
                break;
            case BuildingItem:
            case TileItem:
                BoardManager.Instance.ExcuteCardBuilding(_item);
                break;
            case RuneItem:
                _imgCopyMain.gameObject.SetActive(true);
                originSortingOrder = _imgCopyMain.canvas.sortingOrder;
                _imgCopyMain.canvas.sortingOrder = 10000;
                break;
            case ShopItem:
                break;
        }
    }

    private void EndDrag(PointerEventData eventData)
    {
        isDragging = false;
        switch (_item)
        {
            case CharacterItem:
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = true;
                    UseComplete(true);
                }
                break;
            case BuildingItem:
            case TileItem:
                if (!BoardManager.Instance.RotationStep(this))
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = true;
                    UseComplete(false);
                }
                break;
            case RuneItem:
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = true;
                    UseComplete(false);
                }
                break;
            case ShopItem:
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = true;

                    var data = Managers.Data.ShopDict[_item.GetTableNum];
                    var cardData = UnityEngine.Random.Range(data.minTableRange, data.maxTableRange + 1);
                    Managers.Game.Inven.AddCard(Managers.Data.ShopDict[cardData]);

                    UseComplete(true);
                }
                break;
        }
    }

    private void CancelDrag()
    {
        _isAlreadyStart = false;
        isDragging = false;
        _rect.anchoredPosition = startDragPosition;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        switch (_item)
        {
            case CharacterItem:
                {
                    ResetCopyImage();
                }
                break;
            case BuildingItem:
            case TileItem:
                {
                    BoardManager.Instance.OnCancelCard();
                }
                break;
            case RuneItem:
                {
                    ResetCopyImage();
                }
                break;
            case ShopItem:
                {

                }
                break;
        }
    }

    private void ResetCopyImage()
    {
        _imgCopyMain.gameObject.SetActive(false);
        _imgCopyMain.rectTransform.anchoredPosition = _imgMain.rectTransform.anchoredPosition;
        _imgCopyMain.canvas.sortingOrder = originSortingOrder;
    }
}
