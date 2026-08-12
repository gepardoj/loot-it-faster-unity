using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
  public Vector2Int Position { get; private set; }
  public event Func<ItemImage, Vector2Int, bool?> OnDropItemImage;

  public void Constructor(Vector2Int pos)
  {
    Position = pos;
    name = $"slot x = {pos.x} y = {pos.y}";
  }

  public void OnDrop(PointerEventData eventData)
  {
    if (eventData.pointerDrag != null
    && eventData.pointerDrag.TryGetComponent<ItemImage>(out var draggedItem)
    )
    {
      print($"{draggedItem.name} was dropped at x = {Position.x} y = {Position.y}");
      if (OnDropItemImage?.Invoke(draggedItem, Position) ?? false)
      {
        draggedItem.transform.SetParent(transform);
        draggedItem.rectTransform.anchoredPosition = Vector2.zero;
      }
    }
    print("on drop has ended");
  }
}
