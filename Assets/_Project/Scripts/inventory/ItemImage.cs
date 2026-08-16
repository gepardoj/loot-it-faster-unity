using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class ItemImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
  private RectTransform _rectTransform;
  private CanvasGroup _canvasGroup;
  private Vector2 _originalPosition;
  private Transform _originalParent;
  public int ItemId { get; set; }
  public event Func<ItemImage, InventorySlot, bool?> OnDropEvent;


  private void Awake()
  {
    _rectTransform = GetComponent<RectTransform>();
    _canvasGroup = GetComponent<CanvasGroup>();
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    _originalPosition = _rectTransform.anchoredPosition;
    _originalParent = transform.parent;
    transform.SetParent(InventoryGrid.Instance.Canvas.transform);
    _canvasGroup.blocksRaycasts = false;
    _canvasGroup.alpha = .6f;
  }

  public void OnDrag(PointerEventData eventData)
  {
    _rectTransform.anchoredPosition += eventData.delta / transform.root.localScale.x;
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    _canvasGroup.blocksRaycasts = true;
    _canvasGroup.alpha = 1;
    if (
      eventData.pointerEnter != null
      && eventData.pointerEnter.TryGetComponent<InventorySlot>(out var slot)
      && (OnDropEvent?.Invoke(this, slot) ?? false))
    {
      print($"bingo i've got inventory slot with pos {slot.Position}");
      transform.SetParent(slot.InventoryPopup.ItemsContainer.transform);
      SetPosition(slot.Position);
    }
    else
    {
      transform.SetParent(_originalParent);
      ClearPosition();
    }
  }

  public void SetPosition(Vector2Int pos)
  {
    if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
    _rectTransform.anchoredPosition = new Vector2(pos.x * InventoryGrid.SLOT_SIZE, -pos.y * InventoryGrid.SLOT_SIZE);
  }

  public void ClearPosition()
  {
    _rectTransform.anchoredPosition = _originalPosition;
  }
}
