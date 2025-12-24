using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

[CreateAssetMenu(menuName = "Scooper/Dirt Body Definition")]
public class DirtBodyDefinition : ScriptableObject
{
    [field:SerializeField] public float Radius { get; set; } = 0.2f;
    [field:SerializeField] public float Density { get; set; } = 1f;

    public PhysicsBody CreateBody(PhysicsWorld world, PhysicsBodyDefinition bodyDefinition, Vector2 position)
    {
        bodyDefinition.position = position;
        var body = world.CreateBody(bodyDefinition);

        var shapeDefinition = PhysicsShapeDefinition.defaultDefinition;
        shapeDefinition.density = Density;

        var material = shapeDefinition.surfaceMaterial;
        shapeDefinition.surfaceMaterial = material;

        var geometry = new CircleGeometry { radius = Radius };
        body.CreateShape(geometry, shapeDefinition);

        return body;
    }
}
