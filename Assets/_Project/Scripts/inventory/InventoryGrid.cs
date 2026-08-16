using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryGrid : MonoBehaviour
{
  public const int SLOT_SIZE = 50;
  public static InventoryGrid Instance { get; private set; }
  private InputAction _toggleInventoryKey;
  private InputAction _closeInventoryKey;
  [field: SerializeField] public Canvas Canvas { get; private set; }
  [SerializeField] private InputActionAsset _keyActions;
  [SerializeField] private FirstPersonController _fpsController;
  [SerializeField] private StarterAssetsInputs input;
  [SerializeField] private InventorySlot slotPrefab;
  // Inventory Storage
  [field: SerializeField] public int Width { get; private set; } = 8;
  [field: SerializeField] public int Height { get; private set; } = 10;
  [SerializeField] private InventoryPopup _inventoryPopup;
  public Storage InventoryStorage { get; private set; }
  // External Storage
  [SerializeField] private InventoryPopup _externalStoragePopup;
#nullable enable
  public Storage? ExternalStorage { get; private set; }


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
    _externalStoragePopup.Init(storage.Width);
    // cleanup old slots
    foreach (Transform child in _externalStoragePopup.SlotsContainer.transform) Destroy(child.gameObject);
    foreach (Transform child in _externalStoragePopup.ItemsContainer.transform) Destroy(child.gameObject);
    // init new slots
    for (var y = 0; y < storage.Height; y++)
    {
      for (var x = 0; x < storage.Width; x++)
      {
        InventorySlot slot = Instantiate(slotPrefab, _externalStoragePopup.SlotsContainer.transform);
        slot.Constructor(new Vector2Int(x, y), _externalStoragePopup);
        var item = storage.Items.Find(item => item.Position.Equals(new Vector2Int(x, y)));
        if (item != null)
        {
          var itemImg = ItemFactory.Instance.InstantiateItemImage(item, _externalStoragePopup.ItemsContainer);
          itemImg.OnDropEvent += OnItemImgDrop;
        }
      }
    }
    _externalStoragePopup.Resize(storage.Width, storage.Height);
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
    _inventoryPopup.Init(Width);
    for (var y = 0; y < Height; y++)
    {
      for (var x = 0; x < Width; x++)
      {
        InventorySlot slot = Instantiate(slotPrefab, _inventoryPopup.SlotsContainer.transform);
        slot.Constructor(new Vector2Int(x, y), _inventoryPopup);
        var item = InventoryStorage.Items.Find(item => item.Position.Equals(new Vector2Int(x, y)));
        if (item != null)
        {
          var itemImg = ItemFactory.Instance.InstantiateItemImage(item, _inventoryPopup.ItemsContainer);
          itemImg.OnDropEvent += OnItemImgDrop;
        }
      }
    }
    _inventoryPopup.Resize(Width, Height);
  }

  private bool? OnItemImgDrop(ItemImage itemImage, InventorySlot slot)
  {
    if (slot.InventoryPopup == _inventoryPopup) // item was dropped at Player's Inventory slot
    {
      return OnInventorySlotDrop(itemImage, slot.Position);
    }
    else if (slot.InventoryPopup == _externalStoragePopup) // item was dropped at External Storage, like Chest
    {
      return OnExternalSlotDrop(itemImage, slot.Position);
    }
    else throw new System.Exception($"couldn't determine where item was dropped");
  }

  private bool? OnInventorySlotDrop(ItemImage itemImage, Vector2Int pos)
  {
    var externalItem = ExternalStorage?.Items.Find(item => item.Id == itemImage.ItemId);
    if (externalItem != null) // moves from External Storage to Player's Inventory
    {
      return InventoryStorage.TryTransferItem(ExternalStorage, itemImage.ItemId, pos);
    }
    else // moves inside Player's Inventory
    {
      return InventoryStorage.TryMoveItemTo(itemImage.ItemId, pos);
    }
  }
  private bool? OnExternalSlotDrop(ItemImage itemImage, Vector2Int pos)
  {
    if (ExternalStorage == null) throw new System.Exception();
    var inventoryItem = InventoryStorage.Items.Find(item => item.Id == itemImage.ItemId);
    if (inventoryItem != null) // moves from Player's Inventory to External Storage
    {
      return ExternalStorage.TryTransferItem(InventoryStorage, itemImage.ItemId, pos);
    }
    else // moves inside External Storage
    {
      return ExternalStorage.TryMoveItemTo(itemImage.ItemId, pos);
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
