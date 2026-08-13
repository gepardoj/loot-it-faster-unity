using System.Collections.Generic;
using UnityEngine;

public class ItemFactory : MonoBehaviour
{
    [SerializeField] private ItemConfig[] _configs;
    private Dictionary<ItemType, ItemConfig> _configMap = new();

    private void Start()
    {
        foreach (var config in _configs)
        {
            _configMap.Add(config.Type, config);
        }
    }

    public ItemConfig Get(ItemType type)
    {
        var found = _configMap.TryGetValue(type, out var config);
        if (found == false) throw new System.Exception($"The config has not found of type {type}");
        return config;
    }

    public Item CreateItem(ItemType type)
    {
        var config = Get(type);
        return new Item(config.Type, config.Shape);
    }

    public void InstantiateItemImage(Item item, Transform parentTransform)
    {
        var config = Get(item.Type);
        var img = Instantiate(config.ImgPrefab, parentTransform);
        img.ItemId = item.Id;
    }
}
