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
    [field: SerializeField] public ItemFactory ItemFactory { get; private set; }

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
        var lockpick = ItemFactory.CreateItem(ItemType.LOCKPICK);
        var lockpick2 = ItemFactory.CreateItem(ItemType.LOCKPICK);
        items.Add(lockpick);
        items.Add(lockpick2);
        CreateItem(lockpick, new Vector2Int(0, 0));
        CreateItem(lockpick2, new Vector2Int(1, 0));
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

    public void MoveItemTo(int id, Vector2Int pos)
    {
        var item = items.Find(item => item.Id == id);
        // this two actions of clearing and setting item, it has to be separated, otherwise it can clear each other, if we move the item bellow by 1 step
        if (item.Position.x != -1)
        {   //TODO:we need to use inventory origin pointer. if item came from other source
            foreach (var offset in item.Shape)
            {
                grid[item.Position.x + offset.x, item.Position.y + offset.y] = EMPTY;
            }
        }
        foreach (var offset in item.Shape)
        {
            grid[pos.x + offset.x, pos.y + offset.y] = item.Id;
        }
        item.SetPosition(pos);
    }

    public bool CanPutItem(int id, Vector2Int newPosition)
    {
        var item = items.Find(item => item.Id == id);
        if (item == null) return false;
        foreach (var offset in item.Shape)
        {
            var x = newPosition.x + offset.x;
            var y = newPosition.y + offset.y;
            if (x < 0 || x >= width || y < 0 || y >= height
            || (grid[x, y] != item.Id && grid[x, y] != EMPTY)) return false;
        }
        return true;
    }

    private void Update()
    {
        var a = 1;
    }
}
