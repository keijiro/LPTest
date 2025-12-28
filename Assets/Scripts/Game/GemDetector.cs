using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public sealed class GemDetector : MonoBehaviour
{
    public int? DetectedGemType { get; private set; }

    public void ResetDetection()
      => DetectedGemType = null;

    void OnEnable()
      => PhysicsEvents.PostSimulate += OnPostSimulate;

    void OnDisable()
      => PhysicsEvents.PostSimulate -= OnPostSimulate;

    void OnPostSimulate(PhysicsWorld world, float timeStep)
    {
        if (DetectedGemType.HasValue) return;

        foreach (var e in world.triggerBeginEvents)
        {
            var go = e.visitorShape.body.userData.objectValue as GameObject;
            if (go == null) continue;

            var gem = go.GetComponent<GemController>();
            if (gem == null) continue;

            DetectedGemType = gem.GemType;
            break;
        }
    }
}
