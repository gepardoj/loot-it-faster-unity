using UnityEngine;

public class ChestLock : MonoBehaviour, IItemDropReceiver
{
#nullable enable
  private WorldItem? _lockpick;

  public bool HasLockpick { get => _lockpick != null; }


  public bool OnDrop(ItemImage itemImage)
  {
    if (_lockpick != null) return false; // it's already has a lockpick inside it then abort it
    var item = InventoryManager.Instance.FindItemById(itemImage.ItemId);
    if (item != null && item.Type != ItemType.Lockpick) return false; // it's not lockpick then abort it
    var result = InventoryManager.Instance.TryTransferItemFromInventoryToWorld(itemImage, transform, out var worldItem);
    if (result && worldItem != null) // success, item should be consumed
    {
      _lockpick = worldItem;
      InventoryManager.Instance.CloseInventory();
    }
    return result;
  }
}