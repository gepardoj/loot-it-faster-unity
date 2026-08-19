using UnityEngine;

public class Chest : MonoBehaviour
{
    [field: SerializeField] public ChestLock ChestLock { get; private set; }
    [field: SerializeField] public int Width { get; private set; } = 8;
    [field: SerializeField] public int Height { get; private set; } = 5;
    private Storage _storage;
    public bool Locked { get; set; }


    private void Start()
    {
        _storage = new(Width, Height);
        _storage.GenerateLoot();
    }

    public void Open()
    {
        if (!Locked) InventoryManager.Instance.OpenExternalStorage(_storage);
    }

    public void BreakLockpick()
    {
        ChestLock.BreakLockpick();
    }
}
