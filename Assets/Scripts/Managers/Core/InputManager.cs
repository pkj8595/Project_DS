using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : ManagerBase
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;
    bool _pressed = false;
    float _pressedTime = 0;

    bool _rPressed = false;
    float _rPressedTime1 = 0;

    public void Init(GameObject managerObj)
    {
        base.Init();
        // todo : 나중에 풀기
        //if (!managerObj.TryGetComponent(out EventSystem eventSystem))
        //{
        //    managerObj.AddComponent<EventSystem>();
        //    managerObj.AddComponent<StandaloneInputModule>();
        //}
    }

    public override void OnUpdate()
    {
        //ui가 클릭된 상태라면
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.anyKey && KeyAction != null)
        {
            KeyAction.Invoke();
        }

        if(MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_pressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.LPointerDown);
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.LPress);
                _pressed = true;
            }
            else
            {
                if (_pressed)
                {
                    if(Time.time < _pressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.LClick);
                    MouseAction.Invoke(Define.MouseEvent.LPointerUp);
                }
                _pressed = false;
                _pressedTime = 0;
            }

            if (Input.GetMouseButton(1))
            {
                if (!_rPressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.RPointerDown);
                    _rPressedTime1 = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.RPress);
                _rPressed = true;
            }
            else
            {
                if (_rPressed)
                {
                    if (Time.time < _rPressedTime1 + 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.RClick);
                    MouseAction.Invoke(Define.MouseEvent.RPointerUp);
                }
                _rPressed = false;
                _rPressedTime1 = 0;
            }
        }
    }

    public override void Clear()
    {
        KeyAction = null;
        MouseAction = null;
        _pressed = false;
    }

   
}

