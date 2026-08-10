using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryStorage : MonoBehaviour
{
    public event Action<Item> AddItemEvent;
    [field: SerializeField] public int width { get; private set; } = 8;
    [field: SerializeField] public int height { get; private set; } = 10;
    private int[,] grid;
    private List<Item> items = new();
    [SerializeField] private ItemConfig lockpickItemConfig;

    void Start()
    {
        grid = new int[width, height];
        var lockpick = lockpickItemConfig.CreateItem();
        items.Add(lockpick);
        PutItem(lockpick, new Vector2Int(0, 0));
    }

    void PutItem(Item item, Vector2Int position)
    {
        item.SetPosition(position);
        foreach (var offset in item.Shape)
        {
            grid[position.x + offset.x, position.y + offset.y] = item.Id;
        }
        AddItemEvent?.Invoke(item);
    }
}
