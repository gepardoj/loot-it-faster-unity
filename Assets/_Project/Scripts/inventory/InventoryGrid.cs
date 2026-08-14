using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{
  public static InventoryGrid Instance { get; private set; }
  private InputAction _toggleInventoryKey;
  private InputAction _closeInventoryKey;
  [SerializeField] private InputActionAsset _keyActions;
  [SerializeField] private FirstPersonController _fpsController;
  [SerializeField] private StarterAssetsInputs input;
  [SerializeField] private InventorySlot slotPrefab;
  // Inventory Storage
  [field: SerializeField] public int Width { get; private set; } = 8;
  [field: SerializeField] public int Height { get; private set; } = 10;
  [SerializeField] private InventoryPopup _inventoryPopup;
  public Storage InventoryStorage { get; private set; }
  private List<InventorySlot> _inventorySlots = new();
  // External Storage
  [SerializeField] private GridLayoutGroup _externalStoragePopup;
#nullable enable
  public Storage? ExternalStorage { get; private set; }
  private List<InventorySlot> _externalSlots = new();


  private void Awake()
  {
    Instance = this;
    InventoryStorage = new(Width, Height);
    _toggleInventoryKey = _keyActions.FindAction("Player/Inventory");
    _closeInventoryKey = _keyActions.FindAction("Player/Cancel");
  }

  public void OpenExternalStorage(Storage storage)
  {
    ExternalStorage = storage;
    _externalStoragePopup.constraintCount = storage.Width;
    // cleanup old slots
    foreach (Transform child in _externalStoragePopup.transform) Destroy(child.gameObject);
    _externalSlots.Clear();
    // init new slots
    for (var y = 0; y < storage.Height; y++)
    {
      for (var x = 0; x < storage.Width; x++)
      {
        InventorySlot slot = Instantiate(slotPrefab, _externalStoragePopup.transform);
        slot.Constructor(new Vector2Int(x, y));
        slot.OnDropItemImage += OnExternalSlotDrop;
        _externalSlots.Add(slot);
        var item = storage.Items.Find(item => item.Position.Equals(new Vector2Int(x, y)));
        if (item != null)
        {
          ItemFactory.Instance.InstantiateItemImage(item, slot.transform);
        }
      }
    }
    _externalStoragePopup.gameObject.SetActive(true);
    OpenInventory();
  }

  private void OnEnable()
  {
    _keyActions.Enable();
    _toggleInventoryKey.Enable();
    _toggleInventoryKey.performed += OnToggleInventory;
    _closeInventoryKey.performed += OnCloseInventory;
  }

  private void OnDisable()
  {
    _toggleInventoryKey.performed -= OnToggleInventory;
    _toggleInventoryKey.Disable();
    _closeInventoryKey.performed -= OnCloseInventory;
    _closeInventoryKey.Disable();
  }

  private void Start()
  {
    InventoryStorage.GenerateLoot();
    _inventoryPopup.SlotsContainer.constraintCount = Width;
    for (var y = 0; y < Height; y++)
    {
      for (var x = 0; x < Width; x++)
      {
        InventorySlot slot = Instantiate(slotPrefab, _inventoryPopup.SlotsContainer.transform);
        slot.Constructor(new Vector2Int(x, y));
        slot.OnDropItemImage += OnInventorySlotDrop;
        _inventorySlots.Add(slot);
        var item = InventoryStorage.Items.Find(item => item.Position.Equals(new Vector2Int(x, y)));
        if (item != null)
        {
          ItemFactory.Instance.InstantiateItemImage(item, slot.transform);
        }
      }
    }
  }

  private bool? OnExternalSlotDrop(ItemImage itemImage, Vector2Int pos)
  {
    if (ExternalStorage == null) throw new System.Exception();
    var inventoryItem = InventoryStorage.Items.Find(item => item.Id == itemImage.ItemId);
    if (inventoryItem != null)
    {
      return ExternalStorage.TryTransferItem(InventoryStorage, itemImage.ItemId, pos);
    }
    else
    {
      return ExternalStorage.TryMoveItemTo(itemImage.ItemId, pos);
    }
  }

  private bool? OnInventorySlotDrop(ItemImage itemImage, Vector2Int pos)
  {
    var externalItem = ExternalStorage?.Items.Find(item => item.Id == itemImage.ItemId);
    if (externalItem != null)
    {
      return InventoryStorage.TryTransferItem(ExternalStorage, itemImage.ItemId, pos);
    }
    else
    {
      return InventoryStorage.TryMoveItemTo(itemImage.ItemId, pos);
    }
  }

  private void OnToggleInventory(InputAction.CallbackContext context)
  {
    bool isWindowActive = !_inventoryPopup.gameObject.activeSelf;

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
    _inventoryPopup.gameObject.SetActive(true);
    _fpsController.CanMove = false;
    _fpsController.CanRotate = false;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }

  private void CloseInventory()
  {
    _inventoryPopup.gameObject.SetActive(false);
    _externalStoragePopup.gameObject.SetActive(false);
    _fpsController.CanMove = true;
    _fpsController.CanRotate = true;
    input.cursorInputForLook = true;
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }
}
