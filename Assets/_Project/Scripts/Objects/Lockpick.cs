using UnityEngine;
using UnityEngine.EventSystems;

public class Lockpick : WorldItem, IBeginDragHandler, IDragHandler
{
  public ChestLock ChestLock { get => transform.parent.GetComponent<ChestLock>(); }

  public void OnBeginDrag(PointerEventData eventData)
  {
    var itemImage = InventoryManager.Instance.CreateBufferItem(ItemType.Lockpick);
    var rectTransform = itemImage.GetComponent<RectTransform>();
    rectTransform.pivot = new Vector2(.5f, .5f);
    rectTransform.position = eventData.position;
    eventData.pointerDrag = itemImage.gameObject;
    itemImage.WorldItem = this;
    gameObject.SetActive(false);
    itemImage.OnBeginDrag(eventData);
  }

  public void OnDrag(PointerEventData eventData) { } // is required to work dragging
}