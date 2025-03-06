using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class UI_RuneSlot : UI_Slot, IDropHandler
{
    [SerializeField] private Image _imgSlot;
    [SerializeField] private Text _txtName;
    private Data.RuneData _runeData;
    private int _index;
    private PawnBase _pawnBase;

    public void Init(PawnBase pawn, int index)
    {
        _pawnBase = pawn;
        _index = index;
        _runeData = pawn.RuneList[index];

        if (_runeData != null)
        {
            HasData = true;
            _imgSlot.sprite = Managers.Resource.Load<Sprite>($"{Define.Path.UIIcon}{_runeData.imageStr}");
            _txtName.text = _runeData.name;
            _imgSlot.gameObject.SetActive(true);
        }
        else
        {
            HasData = false;
            _txtName.text = string.Empty;
            _imgSlot.gameObject.SetActive(false);
        }
    }

    public override string SetTitleStr()
    {
        return _runeData.name;
    }

    public override string SetDescStr()
    {
        StringBuilder ret = new();
        ret.Append(_runeData.desc);
        ret.Append("\n");
        ret.Append("\n");
        if (_runeData.statTableNum != 0)
        {
            ret.Append(Utils.GetStatStr(Managers.Data.StatDict[_runeData.statTableNum]));
            ret.Append("\n");
        }
        if (_runeData.skillTableNum != 0)
        {
            var skillData = Managers.Data.SkillDict[_runeData.skillTableNum];
            ret.Append(skillData.name);
            ret.Append(Utils.GetSkillStr(skillData));
            ret.Append("\n");
        }

        return ret.ToString();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropItem = eventData.pointerDrag;
        if (dropItem == null)
            return;

        if (dropItem.TryGetComponent(out UICard card))
        {
            if (card.Item is RuneItem)
            {
                _pawnBase.SetRuneData(card.Item.TableBase as Data.RuneData, _index);
                Managers.UI.GetUI<UIUnitPopup>().UpdateUI();
                card.UseComplete(true);
            }
            else
            {
                card.UseComplete(false);
            }
        }
    }
}
