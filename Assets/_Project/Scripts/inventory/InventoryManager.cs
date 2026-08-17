using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
  public const int SLOT_SIZE = 50;
  public static InventoryManager Instance { get; private set; }
  [field: SerializeField] public Canvas Canvas { get; private set; }
  [SerializeField] private StorageSlot slotPrefab;
  // Player's Storage
  [field: SerializeField] public int Width { get; private set; } = 8;
  [field: SerializeField] public int Height { get; private set; } = 10;
  [SerializeField] private StoragePopup _playerStoragePopup;
  private Storage PlayerStorage { get; set; }
  // External Storage (like chest)
  [SerializeField] private StoragePopup _externalStoragePopup;
#nullable enable
  public Storage? ExternalStorage { get; private set; }

  private List<WorldItem> WorldItems { get; set; } = new();
  private Item? _bufferItem;


  private void Awake()
  {
    Instance = this;
    PlayerStorage = new(Width, Height);
  }

  public ItemImage CreateBufferItem(ItemType itemType)
  {
    var item = ItemFactory.Instance.CreateItem(itemType);
    _bufferItem = item;
    var itemImage = ItemFactory.Instance.InstantiateItemImage(item, Canvas.transform, OnItemImgDrop);
    return itemImage;
  }

  public bool TryTransferItemFromInventoryToWorld(ItemImage itemImage, Transform parent, out WorldItem? worldItem)
  {
    var item = FindItemById(itemImage.ItemId);
    if (item != null)
    {
      PlayerStorage.RemoveItem(item);
      worldItem = ItemFactory.Instance.InstantiateWorldItem(item, parent);
      WorldItems.Add(worldItem);
      return true;
    }
    worldItem = null;
    return false;
  }

  public Item? FindItemById(int id)
  {
    return PlayerStorage.FindItemById(id);
  }

  private void InitStorage(Storage storage, StoragePopup storagePopup)
  {
    storagePopup.Init(storage.Width);
    // cleanup old slots
    foreach (Transform child in storagePopup.SlotsContainer.transform) Destroy(child.gameObject);
    foreach (Transform child in storagePopup.ItemsContainer.transform) Destroy(child.gameObject);
    // init new slots
    for (var y = 0; y < storage.Height; y++)
    {
      for (var x = 0; x < storage.Width; x++)
      {
        StorageSlot slot = Instantiate(slotPrefab, storagePopup.SlotsContainer.transform);
        slot.Constructor(new Vector2Int(x, y), storagePopup);
        var item = storage.FindItemByPosition(new Vector2Int(x, y));
        if (item != null)
        {
          ItemFactory.Instance.InstantiateItemImage(item, storagePopup.ItemsContainer, OnItemImgDrop);
        }
      }
    }
    storagePopup.Resize(storage.Width, storage.Height);
  }

  public void OpenExternalStorage(Storage storage)
  {
    if (_externalStoragePopup.gameObject.activeSelf)
    {
      CloseInventory();
      return;
    }
    ExternalStorage = storage;
    InitStorage(ExternalStorage, _externalStoragePopup);
    _externalStoragePopup.gameObject.SetActive(true);
    OpenInventory();
  }

  private void OnEnable()
  {
    InputManager.Instance.InputActions.UI.Inventory.performed += OnToggleInventory;
    InputManager.Instance.InputActions.UI.Cancel.performed += OnCloseInventory;
  }

  private void OnDisable()
  {
    InputManager.Instance.InputActions.UI.Inventory.performed -= OnToggleInventory;
    InputManager.Instance.InputActions.UI.Cancel.performed -= OnCloseInventory;
  }

  private void Start()
  {
    PlayerStorage.GenerateLoot();
    InitStorage(PlayerStorage, _playerStoragePopup);
  }

  private bool OnItemImgDrop(ItemImage itemImage, StorageSlot slot)
  {
    var inventoryItem = PlayerStorage.FindItemById(itemImage.ItemId);
    var externalItem = ExternalStorage?.FindItemById(itemImage.ItemId);

    if (slot.StoragePopup == _playerStoragePopup) // item was dropped at Player's Inventory slot
    {
      if (inventoryItem != null) // moves inside Player's Inventory
      {
        return PlayerStorage.TryMoveItemTo(itemImage.ItemId, slot.Position);
      }
      else if (externalItem != null)
      {
        if (ExternalStorage == null) throw new System.Exception($"ExternalStorage should be not null");
        return PlayerStorage.TryTransferItem(ExternalStorage, itemImage.ItemId, slot.Position);
      }
      else if (_bufferItem != null)
      {
        var result = PlayerStorage.TryTransferItemFromBuffer(_bufferItem, slot.Position);
        _bufferItem = null;
        return result;
      }
      else throw new System.Exception($"couldn't find any item");
    }
    else if (slot.StoragePopup == _externalStoragePopup) // item was dropped at External Storage, like Chest
    {
      if (ExternalStorage == null) throw new System.Exception($"ExternalStorage should be not null");
      if (inventoryItem != null) // moves from Player's Inventory to External Storage
      {
        return ExternalStorage.TryTransferItem(PlayerStorage, itemImage.ItemId, slot.Position);
      }
      else if (externalItem != null) // moves inside External Storage
      {
        return ExternalStorage.TryMoveItemTo(itemImage.ItemId, slot.Position);
      }
      else if (_bufferItem != null)
      {
        var result = ExternalStorage.TryTransferItemFromBuffer(_bufferItem, slot.Position);
        _bufferItem = null;
        return result;
      }
      else throw new System.Exception($"couldn't find any item");
    }
    else throw new System.Exception($"couldn't determine where item was dropped");
  }

  private void OnToggleInventory(InputAction.CallbackContext context)
  {
    bool isWindowActive = !_playerStoragePopup.gameObject.activeSelf;

    if (isWindowActive)
    {
      OpenInventory();
    }
    else
    {
      CloseInventory();
    }
  }

  private void OnCloseInventory(InputAction.CallbackContext context)
  {
    CloseInventory();
  }

  private void OpenInventory()
  {
    _playerStoragePopup.gameObject.SetActive(true);
    InputManager.Instance.InputActions.Player.Disable();
    Cursor.lockState = CursorLockMode.None;
  }

  public void CloseInventory()
  {
    _playerStoragePopup.gameObject.SetActive(false);
    _externalStoragePopup.gameObject.SetActive(false);
    InputManager.Instance.InputActions.Player.Enable();
    Cursor.lockState = CursorLockMode.Locked;
  }
}
