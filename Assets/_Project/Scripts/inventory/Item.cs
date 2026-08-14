using System;
using UnityEngine;

[Serializable]
public enum ItemType
{
    LOCKPICK
}

public class Item
{
    private static int _idCounter;
    public int Id { get; private set; }
    public ItemType Type { get; private set; }
    public Vector2Int Position { get; private set; }
    // TODO: not necessary to keep `Shape` in each instance, we can look for a config in ItemFactory by item's Type
    public Vector2Int[] Shape { get; private set; }

    public Item(ItemType type, Vector2Int[] shape)
    {
        Id = _idCounter++;
        Type = type;
        Shape = shape;
    }

    public void SetPosition(Vector2Int pos)
    {
        Position = pos;
    }
}
