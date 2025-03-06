using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PropertySlot : UI_Slot
{
    public Text TxtProperty;
    Data.StatData _data;
    public void Init(Data.StatData data)
    {
        HasData = true;
        _data = data;
    }
    public override string SetTitleStr()
    {
        return _data.name;
    }
    public override string SetDescStr()
    {
        return Utils.GetStatStr(_data);
    }

}
