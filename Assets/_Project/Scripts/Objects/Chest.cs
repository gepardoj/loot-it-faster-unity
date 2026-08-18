using UnityEngine;

public class Chest : MonoBehaviour
{
    [field: SerializeField] public int Width { get; private set; } = 8;
    [field: SerializeField] public int Height { get; private set; } = 5;

    public Storage Storage { get; private set; }

    public bool Locked { get => false; }


    private void Start()
    {
        Storage = new(Width, Height);
        Storage.GenerateLoot();
    }

    public void Open()
    {
        InventoryManager.Instance.OpenExternalStorage(Storage);
    }
}
