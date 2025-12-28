using UnityEngine;

public class FallDestroyer : MonoBehaviour
{
    [SerializeField] float _destroyY = -15f;

    void Update()
    {
        if (transform.position.y < _destroyY) Destroy(gameObject);
    }
}
