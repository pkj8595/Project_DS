using UnityEngine;

public class PawnMove : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _stopDistance;
    [SerializeField] private bool _isMove;
    [SerializeField] private Vector3 _destPos;
    

    public bool IsMove { get => _isMove; set => _isMove = value; }

    void Update()
    {
        if (!_isMove)
            return;

        Vector3 direction = _destPos - transform.position;
        direction.z = 0;

        transform.position += direction.normalized * _speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, _destPos) < _stopDistance)
        {
            IsMove = false;
            transform.position = _destPos;
        }
    }

    public void Init(float speed)
    {
        _speed = speed;
    }

    public void Move(Vector3 dest)
    {
        _destPos = dest;
        _destPos.z = 0;
        _isMove = true;
    }

    public void Stop()
    {
        _isMove = false;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }
}
