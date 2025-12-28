using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class StaticBodyBridge : MonoBehaviour
{
    #region Editable Fields

    [SerializeField] bool _isKinematic = false;
    [SerializeField] Categories _category = Categories.Default;
    [SerializeField] Categories _ignore = Categories.None;

    #endregion

    #region Public Properties

    public PhysicsBody Body { get; private set; }

    #endregion

    #region Physics Body Management

    PhysicsRotate RotationFromXform
      => new PhysicsRotate(transform.eulerAngles.z * Mathf.Deg2Rad);

    void CreateBody()
    {
        var bodyDef = PhysicsBodyDefinition.defaultDefinition;
        bodyDef.type = _isKinematic ? PhysicsBody.BodyType.Kinematic :
                                      PhysicsBody.BodyType.Static;
        bodyDef.position = transform.position;
        bodyDef.rotation = RotationFromXform;

        Body = PhysicsWorld.defaultWorld.CreateBody(bodyDef);

        var shapeDef = PhysicsShapeDefinition.defaultDefinition;

        var category = new PhysicsMask((int)_category);
        var mask = PhysicsMask.All;
        mask.ResetBit((int)_ignore);
        shapeDef.contactFilter = new PhysicsShape.ContactFilter(category, mask);

        GetComponent<CompositeShapeBuilder>().CreateShapes(Body, shapeDef);
    }

    #endregion

    #region MonoBehaviour Implementation

    void OnEnable()
    {
        if (!Body.isValid) CreateBody();
    }

    void OnDisable()
    {
        if (Body.isValid) Body.Destroy();
    }

    void FixedUpdate()
    {
        if (_isKinematic)
        {
            var xform = new PhysicsTransform(transform.position, RotationFromXform);
            Body.SetTransformTarget(xform, Time.fixedDeltaTime);
        }
    }

    #endregion
}
