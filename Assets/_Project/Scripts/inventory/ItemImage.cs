using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class ItemImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
  public RectTransform rectTransform;
  private CanvasGroup canvasGroup;
  private Transform originalParent;
  public int ItemId { get; set; }

  private void Awake()
  {
    rectTransform = GetComponent<RectTransform>();
    canvasGroup = GetComponent<CanvasGroup>();
  }

  public void OnBeginDrag(PointerEventData eventData)
  {
    originalParent = transform.parent;
    transform.SetParent(transform.root);
    canvasGroup.blocksRaycasts = false;
    canvasGroup.alpha = .6f;
  }

  public void OnDrag(PointerEventData eventData)
  {
    rectTransform.anchoredPosition += eventData.delta / transform.root.localScale.x;
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    canvasGroup.blocksRaycasts = true;
    canvasGroup.alpha = 1;
    if (transform.root == transform.parent)
    {
      transform.SetParent(originalParent);
      rectTransform.anchoredPosition = Vector2.zero;
    }
  }
}
