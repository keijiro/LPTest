using UnityEngine;

[CreateAssetMenu(menuName = "Scooper/Item Prefab Set")]
public sealed class ItemPrefabSet : ScriptableObject
{
    [SerializeField] GameObject[] _items = null;

    public GameObject GetItemPrefab(ItemType itemType) => _items[(int)itemType];
    public GameObject GetBombPrefab() => _items[0];
    public GameObject GetGemPrefab(int index) => _items[1 + index];
}
