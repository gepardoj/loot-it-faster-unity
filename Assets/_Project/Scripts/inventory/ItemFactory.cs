using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    public static ItemFactory Instance { get; private set; }
    [SerializeField] private ItemConfig[] _configs;
    private Dictionary<ItemType, ItemConfig> _configMap = new();


    private void Awake()
    {
        Instance = this;
        foreach (var config in _configs)
        {
            _configMap.Add(config.Type, config);
        }
    }

    public ItemConfig GetItemConfig(ItemType type)
    {
        var found = _configMap.TryGetValue(type, out var config);
        if (found == false) throw new System.Exception($"The config has not found of type {type}");
        return config;
    }

    public Item CreateItem(ItemType type)
    {
        var config = GetItemConfig(type);
        return new Item(config.Type, config.Shape);
    }

    public ItemImage InstantiateItemImage(Item item, Transform parentTransform, Func<ItemImage, StorageSlot, bool> onDropEvent)
    {
        var config = GetItemConfig(item.Type);
        var itemImg = Instantiate(config.ImgPrefab, parentTransform);
        itemImg.SetPosition(item.Position);
        itemImg.ItemId = item.Id;
        itemImg.OnDropEvent += onDropEvent;
        return itemImg;
    }

    public WorldItem InstantiateWorldItem(Item item, Transform parentTransform)
    {
        var config = GetItemConfig(item.Type);
        var worldItem = Instantiate(config.WorldItemPrefab, parentTransform);
        worldItem.SetItem(item);
        return worldItem;
    }
}
