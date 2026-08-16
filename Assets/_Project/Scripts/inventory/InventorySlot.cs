using UnityEngine;

public class StorageSlot : MonoBehaviour
{
  public Vector2Int Position { get; private set; }
  public StoragePopup StoragePopup { get; private set; }

  public void Constructor(Vector2Int pos, StoragePopup storagePopup)
  {
    Position = pos;
    StoragePopup = storagePopup;
    name = $"{storagePopup.name} - slot x = {pos.x} y = {pos.y}";
  }
}
