using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTextureController : MonoBehaviour
{
    public Camera _mainCamera;
    private Camera _thisCamera;
    private void Awake()
    {
        _thisCamera = GetComponent<Camera>();
    }

    public void LateUpdate()
    {
        _thisCamera.orthographicSize = _mainCamera.orthographicSize;
    }


}
