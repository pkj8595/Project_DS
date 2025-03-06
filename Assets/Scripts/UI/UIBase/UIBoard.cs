using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoard : UIBase
{
    [SerializeField] private List<Image> _imgList;
    [SerializeField] private TMPro.TextMeshProUGUI _txtState;
    public override void SetUIBaseData()
    {
        base.SetUIBaseData();
    }

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


    public void OnClickNodeList(int value)
    {
        BoardManager.Instance.SetNodeIndex(value);
        Debug.Log($"selected node index : {value}");
        _txtState.text = "EditMode";
    }

    public void OnClickCancel()
    {
        BoardManager.Instance.OnCancelSelectedNode();
        _txtState.text = "Not EditMode";
    }


}
