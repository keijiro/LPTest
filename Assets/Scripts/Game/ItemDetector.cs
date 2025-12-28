using UnityEngine;
using UnityEngine.LowLevelPhysics2D;

public sealed class ItemDetector : MonoBehaviour
{
    public ItemController DetectedItem { get; private set; }

    public void ResetDetection()
      => DetectedItem = null;

    void OnEnable()
      => PhysicsEvents.PostSimulate += OnPostSimulate;

    void OnDisable()
      => PhysicsEvents.PostSimulate -= OnPostSimulate;

    void OnPostSimulate(PhysicsWorld world, float timeStep)
    {
        if (DetectedItem != null) return;

        foreach (var e in world.triggerBeginEvents)
        {
            var go = e.visitorShape.body.userData.objectValue as GameObject;
            if (go == null) continue;

            var item = go.GetComponent<ItemController>();
            if (item == null) continue;

            DetectedItem = item;
            break;
        }
    }
}
