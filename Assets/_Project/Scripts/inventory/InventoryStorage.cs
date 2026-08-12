using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryStorage : MonoBehaviour
{
    public const int EMPTY = -1;
    public event Action<Item> AddItemEvent;
    [field: SerializeField] public int width { get; private set; } = 8;
    [field: SerializeField] public int height { get; private set; } = 10;
    private int[,] grid;
    private List<Item> items = new();
    [SerializeField] private ItemConfig lockpickItemConfig;

    void Start()
    {
        grid = new int[width, height];
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                grid[x, y] = EMPTY;
            }
        }
        var lockpick = lockpickItemConfig.CreateItem();
        items.Add(lockpick);
        CreateItem(lockpick, new Vector2Int(0, 0));
    }

    public void CreateItem(Item item, Vector2Int position)
    {
        PutItem(item.Id, position);
        AddItemEvent?.Invoke(item);
    }

    public void PutItem(int id, Vector2Int position)
    {
        var item = items.Find(item => item.Id == id);
        item.SetPosition(position);
        foreach (var offset in item.Shape)
        {
            grid[position.x + offset.x, position.y + offset.y] = item.Id;
        }
    }

    public bool CanPutItem(int id, Vector2Int newPosition)
    {
        var item = items.Find(item => item.Id == id);
        if (item == null) return false;
        foreach (var offset in item.Shape)
        {
            var x = newPosition.x + offset.x;
            var y = newPosition.y + offset.y;
            print($"x = {x} y = {y}");
            if (x < 0 || x >= width || y < 0 || y >= height
            || (grid[x, y] != item.Id && grid[x, y] != EMPTY)) return false;
        }
        return true;
    }
}
