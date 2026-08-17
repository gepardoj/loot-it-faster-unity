using System.Collections.Generic;
using UnityEngine;

public class Storage
{
    public const int EMPTY = -1;

    public int Width { get; private set; }
    public int Height { get; private set; }
    private int[,] _grid;
    private List<Item> _items;

    public Storage(int width, int height)
    {
        Width = width;
        Height = height;
        _grid = new int[width, height];
        _items = new();
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _grid[x, y] = EMPTY;
            }
        }
    }

    public void GenerateLoot()
    {
        var lockpick = ItemFactory.Instance.CreateItem(ItemType.Lockpick);
        var lockpick2 = ItemFactory.Instance.CreateItem(ItemType.Lockpick);
        _items.Add(lockpick);
        _items.Add(lockpick2);
        CreateItem(lockpick, new Vector2Int(0, 0));
        CreateItem(lockpick2, new Vector2Int(1, 0));
    }

    public void RemoveItem(Item item)
    {
        CleanGridAfterItem(item);
        _items.Remove(item);
    }

#nullable enable
    public Item? FindItemById(int id)
    {
        return _items.Find(item => item.Id == id);
    }

    public Item? FindItemByPosition(Vector2Int pos)
    {
        return _items.Find(item => item.Position.Equals(pos));
    }

    private void CleanGridAfterItem(Item item)
    {
        foreach (var offset in item.Shape)
        {
            _grid[item.Position.x + offset.x, item.Position.y + offset.y] = EMPTY;
        }
    }

    private void CreateItem(Item item, Vector2Int pos)
    {
        PutItemTo(item, pos);
    }

    /** only for new, as it doesnt clean the old positions, compared to `MoveItemTo` */
    private void PutItemTo(Item item, Vector2Int pos)
    {
        item.SetPosition(pos);
        foreach (var offset in item.Shape)
        {
            _grid[pos.x + offset.x, pos.y + offset.y] = item.Id;
        }
    }

    private void MoveItemTo(int id, Vector2Int pos)
    {
        var item = _items.Find(item => item.Id == id);
        // this two actions of clearing and setting item, it has to be separated, otherwise it can clear each other, if we move the item bellow by 1 step
        if (item.Position.x != -1)
        {   //TODO:we need to use inventory origin pointer. if item came from other source
            foreach (var offset in item.Shape)
            {
                _grid[item.Position.x + offset.x, item.Position.y + offset.y] = EMPTY;
            }
        }
        foreach (var offset in item.Shape)
        {
            _grid[pos.x + offset.x, pos.y + offset.y] = item.Id;
        }
        item.SetPosition(pos);
    }

    private bool CanMoveItemTo(Storage origin, int id, Vector2Int pos)
    {
        var item = origin._items.Find(item => item.Id == id);
        if (item == null) return false;
        foreach (var offset in item.Shape)
        {
            var x = pos.x + offset.x;
            var y = pos.y + offset.y;
            if (x < 0 || x >= Width || y < 0 || y >= Height
            || (_grid[x, y] != item.Id && _grid[x, y] != EMPTY)) return false;
        }
        return true;
    }

    // moving item inside one storage
    public bool TryMoveItemTo(int id, Vector2Int pos)
    {
        if (CanMoveItemTo(this, id, pos))
        {
            MoveItemTo(id, pos);
            return true;
        }
        return false;
    }

    // moving item between two storages
    public bool TryTransferItem(Storage origin, int id, Vector2Int pos)
    {
        if (CanMoveItemTo(origin, id, pos))
        {
            var item = origin._items.Find(item => item.Id == id);
            origin._items.Remove(item);
            origin.CleanGridAfterItem(item);
            _items.Add(item);
            PutItemTo(item, pos);
            return true;
        }
        return false;
    }
}
