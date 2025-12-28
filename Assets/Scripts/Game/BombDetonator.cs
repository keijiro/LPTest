using Unity.Collections;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public sealed class BombDetonator : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] float _speedThreshold = 1;
    [SerializeField] float _activationDelay = 3;

    #endregion

    #region Private Members

    PhysicsBody _body;
    float _elapsed;

    float GetMaxNormalSpeed()
    {
        var maxSpeed = 0f;

        using var contacts = _body.GetContacts(Allocator.Temp);

        for (var i = 0; i < contacts.Length; ++i)
        {
            var manifold = contacts[i].manifold;
            var points = manifold.points;
            for (var p = 0; p < manifold.pointCount; ++p)
                maxSpeed = Mathf.Max(maxSpeed, Mathf.Abs(points[p].normalVelocity));
        }

        return maxSpeed;
    }

    #endregion

    #region MonoBehaviour Implementation

    void Start()
      => _body = GetComponent<DynamicBodyBridge>().Body;

    void FixedUpdate()
    {
        _elapsed += Time.fixedDeltaTime;
        if (_elapsed < _activationDelay) return;

        if (GetMaxNormalSpeed() > _speedThreshold)
            GameState.IsBombDetonated = true;
    }

    #endregion
}
