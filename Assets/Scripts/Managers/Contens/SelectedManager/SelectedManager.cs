using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface ISelectedable
{
    public void OnSelect();
    public void OnDeSelect();
    public bool IsSelected { get; set; }
}

public class SelectedManager : MonoBehaviour
{
    private List<ISelectedable> _selectedObjectList = new List<ISelectedable>();
    public RectTransform RtSelectionBox;

    private Vector3 _startMousePos;
    private Vector3 _endMousePos;
    private bool _isDragging;

    public void Init()
    {
        Managers.Input.MouseAction -= SelectMouseAction;
        Managers.Input.MouseAction += SelectMouseAction;
    }

    private void SelectMouseAction(Define.MouseEvent mouse)
    {
        if (mouse == Define.MouseEvent.LClick)
        {
            DeselectAllObject();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = (int)Define.Layer.Pawn | (int)Define.Layer.Building;
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
            {
                if (hit.collider.TryGetComponent(out ISelectedable selectedable))
                {
                    SelectObject(selectedable);
                }
            }
        }

        // 마우스 우클릭으로 유닛 명령 (이동)
        if (mouse == Define.MouseEvent.RClick && _selectedObjectList.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = (int)Define.Layer.Ground;
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
            {
                foreach (ISelectedable selected in _selectedObjectList)
                {
                    PawnBase pawn = selected as PawnBase;
                    if (pawn != null && pawn.Team == Define.ETeam.Playable)
                    {
                        pawn.CommandMoveTo(hit.point);
                    }
                }
            }
            DeselectAllObject();
        }
/*
        if (mouse == Define.MouseEvent.LPointerDown)
        {
            _startMousePos = Input.mousePosition;
            _isDragging = true;
            RtSelectionBox.gameObject.SetActive(true);
        }
        if (mouse == Define.MouseEvent.LPointerUp)
        {
            _isDragging = false;
            _endMousePos = Input.mousePosition;
            RtSelectionBox.gameObject.SetActive(false);
        }

        if (_isDragging)
        {
            _endMousePos = Input.mousePosition;
            UpdateSelectionBox();
        }
*/
    }

    private void UpdateSelectionBox()
    {
        Vector2 boxStart = _startMousePos;
        Vector2 boxSize = _endMousePos - _startMousePos;

        RtSelectionBox.anchoredPosition = boxStart;
        RtSelectionBox.sizeDelta = new Vector2(Mathf.Abs(boxSize.x), Mathf.Abs(boxSize.y));

        if (boxSize.x < 0)
            RtSelectionBox.pivot = new Vector2(1, RtSelectionBox.pivot.y); 
        else
            RtSelectionBox.pivot = new Vector2(0, RtSelectionBox.pivot.y); 

        if (boxSize.y < 0)
            RtSelectionBox.pivot = new Vector2(RtSelectionBox.pivot.x, 1); 
        else
            RtSelectionBox.pivot = new Vector2(RtSelectionBox.pivot.x, 0); 
    }


    private void EndDrawSelectedBox()
    {
        DeselectAllObject();

    }


    public void SelectObject(ISelectedable monster)
    {
        if (!_selectedObjectList.Contains(monster))
        {
            _selectedObjectList.Add(monster);
            monster.OnSelect();
        }
    }

    public void DeselectAllObject()
    {
        foreach (var monster in _selectedObjectList)
        {
            monster.OnDeSelect();
        }
        _selectedObjectList.Clear();
    }

    public List<ISelectedable> GetSelectedMonsters()
    {
        return _selectedObjectList;
    }

    
}
