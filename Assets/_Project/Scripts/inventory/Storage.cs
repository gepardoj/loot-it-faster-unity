using System;
using System.Collections.Generic;
using UnityEngine;

public class Storage
{
    public const int EMPTY = -1;

    private readonly int[,] _grid;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public List<Item> Items { get; private set; } = new();

    public Storage(int width, int height)
    {
        Width = width;
        Height = height;
        _grid = new int[width, height];
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
        var lockpick = ItemFactory.Instance.CreateItem(ItemType.LOCKPICK);
        var lockpick2 = ItemFactory.Instance.CreateItem(ItemType.LOCKPICK);
        Items.Add(lockpick);
        Items.Add(lockpick2);
        CreateItem(lockpick, new Vector2Int(0, 0));
        CreateItem(lockpick2, new Vector2Int(1, 0));
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
        var item = Items.Find(item => item.Id == id);
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
        var item = origin.Items.Find(item => item.Id == id);
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
            var item = origin.Items.Find(item => item.Id == id);
            origin.Items.Remove(item);
            origin.CleanGridAfterItem(item);
            Items.Add(item);
            PutItemTo(item, pos);
            return true;
        }
        return false;
    }
}
