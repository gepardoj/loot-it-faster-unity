using UnityEngine;

public class InventorySlot : MonoBehaviour
{
  public Vector2Int Position { get; private set; }
  public InventoryPopup InventoryPopup { get; private set; }

  public void Constructor(Vector2Int pos, InventoryPopup invPopup)
  {
    Position = pos;
    InventoryPopup = invPopup;
    name = $"{invPopup.name} - slot x = {pos.x} y = {pos.y}";
  }
}
