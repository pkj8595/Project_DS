using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEventTest : MonoBehaviour
{
    public Define.EGoodsType goods;

    private void Awake()
    {
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Test2();
        }

    }
    


    public void Test2()
    {
        var uimain = Managers.UI.GetUI<UIMain>() as UIMain;
        uimain.UIMoveResource.QueueAddItem(transform.position, goods, 100);
    }

    public void Test3()
    {
        var uimain = Managers.UI.GetUI<UIMain>() as UIMain;
        uimain.UIMoveResource.QueueSpendItem(transform, Vector3.zero, goods, 100, (isSpend) =>
        {
            if (isSpend)
                Debug.Log("isSpend true");
            else
                Debug.Log("isSpend false");
        });
    }
   
}
