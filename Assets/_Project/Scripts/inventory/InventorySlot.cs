using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Vector2Int Position { get; private set; }

    public void Constructor(Vector2Int pos)
    {
        Position = pos;
        name = $"slot x = {pos.x} y = {pos.y}";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("hey");
    }
}
