using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevelPhysics2D;

public class ScoopController : MonoBehaviour
{
    [SerializeField] StaticBodyBridge _anchorBody = null;
    [SerializeField] Transform _anchorPoint = null;
    [SerializeField] DynamicBodyBridge _scoopBody = null;
    [SerializeField] Transform _mouseAnchor = null;
    [SerializeField] Transform _rimAnchor = null;
    [SerializeField] Camera _targetCamera = null;
    [field:SerializeField] public float MouseSpringFrequency { get; set; } = 8f;
    [field:SerializeField] public float MouseSpringDamping { get; set; } = 0.7f;
    [field:SerializeField] public float RimSpringFrequency { get; set; } = 3f;
    [field:SerializeField] public float RimSpringDamping { get; set; } = 0.9f;

    PhysicsBody _mouseBody;
    PhysicsJoint _mouseJoint;
    PhysicsJoint _rimJoint;

    void Start()
    {
        CreateMouseBody();
        CreateRimJoint();
    }

    void OnDestroy()
    {
        if (_mouseJoint.isValid)
            _mouseJoint.Destroy();

        if (_mouseBody.isValid)
            _mouseBody.Destroy();
    }

    void Update()
    {
        if (!_mouseBody.isValid)
            return;

        var pointer = Pointer.current;
        if (pointer == null)
            return;

        if (pointer.press.isPressed)
        {
            if (!_mouseJoint.isValid)
                CreateMouseJoint();

            var target = (Vector2)_targetCamera.ScreenToWorldPoint(pointer.position.value);
            var mouseTransform = _mouseBody.transform;
            mouseTransform.position = target;
            _mouseBody.transform = mouseTransform;
            _mouseBody.linearVelocity = Vector2.zero;
            _mouseBody.angularVelocity = 0f;
        }
        else if (_mouseJoint.isValid)
            _mouseJoint.Destroy();
    }

    void CreateMouseBody()
    {
        var bodyDef = PhysicsBodyDefinition.defaultDefinition;
        bodyDef.type = PhysicsBody.BodyType.Kinematic;
        bodyDef.position = _scoopBody.Body.transform.position;
        _mouseBody = PhysicsWorld.defaultWorld.CreateBody(bodyDef);
    }

    void CreateMouseJoint()
    {
        var jointDef = PhysicsDistanceJointDefinition.defaultDefinition;
        jointDef.bodyA = _scoopBody.Body;
        jointDef.bodyB = _mouseBody;
        jointDef.localAnchorA = new PhysicsTransform(
            jointDef.bodyA.GetLocalPoint(_mouseAnchor.position));
        jointDef.localAnchorB = PhysicsTransform.identity;
        jointDef.distance = 0f;
        jointDef.enableSpring = true;
        jointDef.springFrequency = MouseSpringFrequency;
        jointDef.springDamping = MouseSpringDamping;
        _mouseJoint = PhysicsWorld.defaultWorld.CreateJoint(jointDef);
    }

    void CreateRimJoint()
    {
        var jointDef = PhysicsDistanceJointDefinition.defaultDefinition;
        jointDef.bodyA = _scoopBody.Body;
        jointDef.bodyB = _anchorBody.Body;
        jointDef.localAnchorA = new PhysicsTransform(
            jointDef.bodyA.GetLocalPoint(_rimAnchor.position));
        jointDef.localAnchorB = new PhysicsTransform(
            jointDef.bodyB.GetLocalPoint(_anchorPoint.position));
        jointDef.distance = 0f;
        jointDef.enableSpring = true;
        jointDef.springFrequency = RimSpringFrequency;
        jointDef.springDamping = RimSpringDamping;
        jointDef.collideConnected = true;
        _rimJoint = PhysicsWorld.defaultWorld.CreateJoint(jointDef);
    }
}
