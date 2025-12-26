using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class DynamicObjectBridge : MonoBehaviour
{
    #region Editable Fields

    [SerializeField] float _radius = 0.2f;
    [SerializeField] int _sides = 0;
    [SerializeField] float _density = 1;
    [SerializeField] Categories _category = Categories.Default;

    #endregion

    #region Public Properties

    public PhysicsBody Body => _body;

    #endregion

    #region Physics Body Management

    PhysicsBody _body;

    void CreateBody()
    {
        var bodyDef = PhysicsBodyDefinition.defaultDefinition;
        bodyDef.type = PhysicsBody.BodyType.Dynamic;
        bodyDef.position = transform.position;
        bodyDef.rotation = new PhysicsRotate(transform.eulerAngles.z * Mathf.Deg2Rad);

        _body = PhysicsWorld.defaultWorld.CreateBody(bodyDef);

        var shapeDef = PhysicsShapeDefinition.defaultDefinition;
        shapeDef.density = _density;
        shapeDef.triggerEvents = true;

        var category = new PhysicsMask((int)_category);
        var mask = PhysicsMask.All;
        shapeDef.contactFilter = new PhysicsShape.ContactFilter(category, mask);

        if (_sides < 3)
        {
            var geometry = new CircleGeometry { radius = _radius };
            _body.CreateShape(geometry, shapeDef);
        }
        else
        {
            var sides = Mathf.Clamp(_sides, 3, PhysicsConstants.MaxPolygonVertices);
            var vertices = new Vector2[sides];
            var step = Mathf.PI * 2f / sides;
            for (var i = 0; i < sides; ++i)
            {
                var angle = step * i;
                vertices[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _radius;
            }

            var geometry = PolygonGeometry.Create(vertices, 0f);
            _body.CreateShape(geometry, shapeDef);
        }
    }

    void SyncTransform()
    {
        var xform = _body.transform;
        var position = xform.position;
        var angle = xform.rotation.angle * Mathf.Rad2Deg;
        transform.SetPositionAndRotation(
            new Vector3(position.x, position.y, transform.position.z),
            Quaternion.Euler(0f, 0f, angle));
    }

    #endregion

    #region MonoBehaviour Implementation

    void Start()
      => CreateBody();

    void OnDestroy()
      => _body.Destroy();

    void FixedUpdate()
      => SyncTransform();

    #endregion
}
