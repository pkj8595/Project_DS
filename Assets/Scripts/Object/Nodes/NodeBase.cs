using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// MonoBehaviour가 들어가는 타일을 제어하는 BASE 클래스
/// </summary>
public abstract class NodeBase : MonoBehaviour
{
    [SerializeField] private Vector3Int _tilePosition;
    [SerializeField] private Vector3Int _tileSize;
    [SerializeField] private float aniDuration = 0.5f;

    public Vector3Int Position { get => _tilePosition; set => _tilePosition = value; }
    public Vector3Int NodeSize { get => _tileSize; set => _tileSize = value; }
    private static Vector3 StartSize = new(0.1f, 0.1f, 0.1f);
    private System.Action CompleteAction;

    public void Init(Vector3Int tilePosition)
    {
        this._tilePosition = tilePosition;
    }

    public void SetActive(bool isActive)
    {
        if (isActive)
            OnActive();
        else
            OnDeActive();
    }

    private void OnActive()
    {
        gameObject.SetActive(true);
        transform.DOScale(Vector3.one, aniDuration)
            .From(StartSize)
            .SetEase(Ease.InOutBack)
            .OnComplete(() => { 
                CompleteAction?.Invoke();
                CompleteAction = null;
            });
    }

    private void OnDeActive()
    {
        transform.DOScale(StartSize, aniDuration)
                .From(Vector3.one)
                .SetEase(Ease.InOutBack)
                .OnComplete(() => {
                    gameObject.SetActive(false);
                    Managers.Resource.Destroy(gameObject); 
                });
    }

    /// <summary>
    /// Node의 회전 설정 회전이 필요한 블록의 경우만 구현
    /// </summary>
    /// <param name="normal"></param>
    public virtual void SetNodeRotation(Vector3 normal)
    {

    }
    /// <summary>
    /// 설치 성공시 건물 셋팅 시작
    /// </summary>
    public virtual void InstallationSuccess()
    {

    }

    public void SetActiveCompleteAction(System.Action ActiveCompleteAction)
    {
        CompleteAction = ActiveCompleteAction;
    }

}
