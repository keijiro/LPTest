using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public class PaydirtManager : MonoBehaviour
{
    #region Editable Fields

    [Space]
    [SerializeField] float _radius = 0.2f;
    [SerializeField] float _density = 1;
    [Space]
    [SerializeField] float _pourRate = 512;
    [SerializeField] int _targetBodyCount = 1024;
    [Space]
    [SerializeField] SpoutPositionProvider _spout = null;
    [SerializeField] float _recycleY = -10;

    #endregion

    #region Public Methods

    public void RequestInjection()
    {
        for (var i = 0; i < _poolBodies.Count; ++i)
            _pendingBodies.Enqueue(_poolBodies[i]);

        _poolBodies.Clear();
        ConsoleManager.AddLine("Paydirt injection queued.");
    }

    #endregion

    #region MonoBehaviour Implementation

    void Start()
      => CreatePool();

    void OnDestroy()
      => DestroyPool();

    void Update()
    {
        SpawnDirtBodies();
        RecycleDirtBodies();
    }

    #endregion

    #region Body Pool Lifecycle

    readonly List<PhysicsBody> _poolBodies = new();
    readonly List<PhysicsBody> _activeBodies = new();
    readonly Queue<PhysicsBody> _pendingBodies = new();

    void CreatePool()
    {
        var bodyDef = PhysicsBodyDefinition.defaultDefinition;
        bodyDef.type = PhysicsBody.BodyType.Dynamic;

        var shapeDef = PhysicsShapeDefinition.defaultDefinition;
        shapeDef.density = _density;

        var categories = new PhysicsMask((int)Categories.Dirt);
        var contacts = PhysicsMask.All;
        shapeDef.contactFilter = new PhysicsShape.ContactFilter(categories, contacts);

        var geometry = new CircleGeometry { radius = _radius };

        for (var i = 0; i < _targetBodyCount; ++i)
        {
            var body = PhysicsWorld.defaultWorld.CreateBody(bodyDef);
            body.enabled = false;
            body.CreateShape(geometry, shapeDef);
            _poolBodies.Add(body);
        }
    }

    void DestroyPool()
    {
        _poolBodies.ForEach(body => body.Destroy());
        _activeBodies.ForEach(body => body.Destroy());
        _pendingBodies.ToList().ForEach(body => body.Destroy());

        _poolBodies.Clear();
        _pendingBodies.Clear();
        _activeBodies.Clear();
    }

    #endregion

    #region Physics Body Lifecycle

    float _spawnAccumulator;

    void SpawnDirtBodies()
    {
        if (_pendingBodies.Count == 0) return;

        _spawnAccumulator += _pourRate * Time.deltaTime;

        while (_spawnAccumulator >= 1 && _pendingBodies.Count > 0)
        {
            _spawnAccumulator -= 1;

            var xform = new PhysicsTransform(_spout.GetPosition());

            var body = _pendingBodies.Dequeue();
            body.enabled = true;
            body.SetAndWriteTransform(xform);
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;

            _activeBodies.Add(body);
        }
    }

    void RecycleDirtBodies()
    {
        for (var i = _activeBodies.Count - 1; i >= 0; --i)
        {
            var body = _activeBodies[i];
            if (body.transform.position.y >= _recycleY) continue;

            body.enabled = false;
            _poolBodies.Add(body);
            _activeBodies.RemoveAt(i);
        }
    }

    #endregion
}
