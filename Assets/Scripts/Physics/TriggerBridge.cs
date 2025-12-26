using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class TriggerBridge : MonoBehaviour
{
    #region Editable Fields

    [SerializeField] Categories _detect = Categories.Default;

    #endregion

    #region Transform Cache and Checker

    (Vector2 position, float rotation) _lastXform;

    bool IsPositionChanged
      => _lastXform.position != (Vector2)transform.position;

    bool IsRotationChanged
      => !Mathf.Approximately(Mathf.DeltaAngle(_lastXform.rotation, transform.eulerAngles.z), 0);

    void CacheTransform()
    {
        _lastXform.position = transform.position;
        _lastXform.rotation = transform.eulerAngles.z;
    }

    #endregion

    #region Physics Body Management

    CompositeShapeBuilder _shapeBuilder;
    PhysicsBody _body;

    void CreateBody()
    {
        var bodyDef = PhysicsBodyDefinition.defaultDefinition;
        bodyDef.position = transform.position;
        bodyDef.rotation = new PhysicsRotate(transform.eulerAngles.z * Mathf.Deg2Rad);

        _body = PhysicsWorld.defaultWorld.CreateBody(bodyDef);

        var definition = PhysicsShapeDefinition.defaultDefinition;
        definition.isTrigger = true;
        definition.triggerEvents = true;

        var category = new PhysicsMask((int)Categories.Trigger);
        var mask = new PhysicsMask((int)_detect);
        definition.contactFilter = new PhysicsShape.ContactFilter(category, mask);

        _shapeBuilder.CreateShapes(_body, definition);
    }

    void ApplyTransform()
    {
        var rot = new PhysicsRotate(transform.eulerAngles.z * Mathf.Deg2Rad);
        var xform = new PhysicsTransform(transform.position, rot);
        _body.SetAndWriteTransform(xform);
    }

    #endregion

    #region MonoBehaviour Implementation

    void Awake()
      => _shapeBuilder = GetComponent<CompositeShapeBuilder>();

    void Start()
    {
        if (_shapeBuilder == null)
            return;

        CreateBody();
        ApplyTransform();
        CacheTransform();
    }

    void OnDestroy()
    {
        if (_body.isValid)
            _body.Destroy();
    }

    void FixedUpdate()
    {
        if (!_body.isValid)
            return;

        if (IsPositionChanged || IsRotationChanged)
        {
            ApplyTransform();
            CacheTransform();
        }
    }

    #endregion
}
