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
  public event Func<ItemImage, StorageSlot, bool> OnDropEvent;


  private void Awake()
  {
    _rectTransform = GetComponent<RectTransform>();
    _canvasGroup = GetComponent<CanvasGroup>();
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    _originalPosition = _rectTransform.anchoredPosition;
    _originalParent = transform.parent;
    transform.SetParent(InventoryManager.Instance.Canvas.transform);
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
    if ( // item was dropped on slot
      eventData.pointerEnter != null
      && eventData.pointerEnter.TryGetComponent<StorageSlot>(out var slot)
      && (OnDropEvent?.Invoke(this, slot) ?? false))
    {
      transform.SetParent(slot.StoragePopup.ItemsContainer.transform);
      SetPosition(slot.Position);
    }
    else // item was dropped outside, or slot is occupied
    {
      Ray ray = Camera.main.ScreenPointToRay(eventData.position);
      Physics.Raycast(ray, out RaycastHit hit, PlayerInteractor.InteractDistance);
      // if item was consumed (should disappear from inventory) by the 3d game object
      if (hit.collider != null && hit.collider.TryGetComponent<IItemDropReceiver>(out var obj) && obj.OnDrop(this))
      {
        Destroy(gameObject);
      }
      else
      {
        transform.SetParent(_originalParent);
        ClearPosition();
      }
    }
  }

  public void SetPosition(Vector2Int pos)
  {
    if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
    _rectTransform.anchoredPosition = new Vector2(pos.x * InventoryManager.SLOT_SIZE, -pos.y * InventoryManager.SLOT_SIZE);
  }

  public void ClearPosition()
  {
    _rectTransform.anchoredPosition = _originalPosition;
  }
}
