using UnityEngine;


[CreateAssetMenu(fileName = "item config", menuName = "game/items/item config")]
public class ItemConfig : ScriptableObject
{
  [SerializeField] private ItemType type;
  [SerializeField] private Vector2Int[] shape;
  [SerializeField] private ItemImage imgPrefab;

  public virtual Item CreateItem()
  {
    return new Item(type, shape, imgPrefab);
  }
}