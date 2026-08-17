using UnityEngine;


[CreateAssetMenu(fileName = "item config", menuName = "game/items/item config")]
public class ItemConfig : ScriptableObject
{
  [field: SerializeField] public ItemType Type { get; private set; }
  [field: SerializeField] public Vector2Int[] Shape { get; private set; }
  [field: SerializeField] public ItemImage ImgPrefab { get; private set; }
  [field: SerializeField] public WorldItem WorldItemPrefab { get; private set; }
}