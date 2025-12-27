using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    #region Editable Fields

    [Space]
    [SerializeField] SpoutPositionProvider _spout = null;
    [Space]
    [SerializeField] GameObject _bombPrefab = null;
    [SerializeField] GameObject[] _gemPrefabs = null;

    #endregion

    #region Public Methods

    public async Awaitable StartSpawnBombs(int count, float duration)
      => await StartSpawn(_bombPrefab, count, duration);

    public async Awaitable StartSpawnGems(int prefabIndex, int count, float duration)
      => await StartSpawn(_gemPrefabs[prefabIndex], count, duration);

    #endregion

    #region Spawn Implementation

    async Awaitable StartSpawn(GameObject prefab, int count, float duration)
    {
        var step = duration / count;
        for (var i = 0; i < count; ++i)
        {
            var r = Random.value;
            await Awaitable.WaitForSecondsAsync(step * r);
            Spawn(prefab);
            await Awaitable.WaitForSecondsAsync(step * (1 - r));
        }
    }

    void Spawn(GameObject prefab)
      => Instantiate(prefab, _spout.GetPosition(), Quaternion.identity);

    #endregion
}
