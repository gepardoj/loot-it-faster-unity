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
    public GameObject ImagePrefab { get; private set; }

    public Item(ItemType type, Vector2Int[] shape, GameObject imagePrefab)
    {
        Id = idCounter++;
        Type = type;
        Shape = shape;
        ImagePrefab = imagePrefab;
    }

    public void SetPosition(Vector2Int position)
    {
        Position = position;
    }

}
