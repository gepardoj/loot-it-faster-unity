using System;
using UnityEngine;

[Serializable]
public enum ItemType
{
    LOCKPICK
}

public class Item
{
    static int idCounter;
    public int Id { get; private set; }
    public ItemType Type { get; private set; }
    public Vector2Int Position { get; private set; }
    public Vector2Int[] Shape { get; private set; }
    // TODO: replace to Id
    public ItemImage ImagePrefab { get; private set; }

    public Item(ItemType type, Vector2Int[] shape, ItemImage imagePrefab)
    {
        Id = idCounter++;
        Type = type;
        Shape = shape;
        ImagePrefab = imagePrefab;
        ImagePrefab.Id = Id;
    }

    public void SetPosition(Vector2Int position)
    {
        Position = position;
    }

}
