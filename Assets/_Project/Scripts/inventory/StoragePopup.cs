using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class StoragePopup : MonoBehaviour
{
    [field: SerializeField] public RectTransform Wrapper { get; private set; }
    [field: SerializeField] public GridLayoutGroup SlotsContainer { get; private set; }
    [field: SerializeField] public RectTransform ItemsContainer { get; private set; }

    [field: SerializeField] public Vector2 Padding { get; private set; } = new(20, 20);


    public void Init(int width)
    {
        SlotsContainer.constraintCount = width;
    }

    public void Resize(int width, int height)
    {
        var size = new Vector2(width * InventoryManager.SLOT_SIZE, height * InventoryManager.SLOT_SIZE);
        Wrapper.sizeDelta = size;
        GetComponent<RectTransform>().sizeDelta = size + Padding;
    }
}
