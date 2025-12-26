using UnityEngine;

public class BucketController : MonoBehaviour
{
    [SerializeField] float _openAngle = 60;
    [SerializeField] float _moveSpeed = 180;

    Bucket _bucket;
    float _currentAngle;
    bool _isOpening;

    public void Open() => _isOpening = true;

    public void Close() => _isOpening = false;

    void Start()
    {
        _bucket = GetComponent<Bucket>();
        _currentAngle = _bucket.BottomAngle;
    }

    void Update()
    {
        var delta = (_isOpening ? 1 : -1) * _moveSpeed * Time.deltaTime;
        _currentAngle = Mathf.Clamp(_currentAngle + delta, 0, _openAngle);
        _bucket.BottomAngle = _currentAngle;
    }
}
