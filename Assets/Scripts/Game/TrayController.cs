using UnityEngine;

public sealed class TrayController : MonoBehaviour
{
    #region Editable Fields

    [SerializeField] Vector2 _inPoint = Vector2.zero;
    [SerializeField] Vector2 _outPoint = Vector2.one;
    [SerializeField] float _moveDuration = 1;

    #endregion

    #region Public Methods

    public void MoveIn() => _direction = 1;

    public void MoveOut() => _direction = -1;

    #endregion

    #region Motion

    float _parameter;
    float _direction;

    #endregion

    #region MonoBehaviour Implementation

    void FixedUpdate()
    {
        var delta = _direction * Time.fixedDeltaTime / _moveDuration;
        _parameter = Mathf.Clamp01(_parameter + delta);

        var cosParam = Mathf.Cos(_parameter * Mathf.PI) * 0.5f + 0.5f;
        var position = Vector2.Lerp(_inPoint, _outPoint, cosParam);

        transform.position = position;
    }

    #endregion
}
