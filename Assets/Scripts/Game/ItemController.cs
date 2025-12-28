using UnityEngine;

public enum ItemType { Bomb, Gem1, Gem2, Gem3, Gem4, Gem5 }

public sealed class ItemController : MonoBehaviour
{
    [field:SerializeField] public ItemType Type { get; set; }
}
