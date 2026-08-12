using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryGrid : MonoBehaviour
{
  [SerializeField] private InputActionAsset keyAction;
  [SerializeField] private FirstPersonController fpsController;
  [SerializeField] private StarterAssetsInputs input;
  [SerializeField] private InventoryPopup inventoryPopup;
  [SerializeField] private InventorySlot slotPrefab;
  [SerializeField] private InventoryStorage inventoryStorage;

  private InputAction toggleInventoryAction;

  private List<InventorySlot> slots;


  private void Awake()
  {
    toggleInventoryAction = keyAction.FindAction("UI/InventoryToggleKey");
    inventoryPopup.SlotsContainer.constraintCount = inventoryStorage.width;
    slots = new();
    for (var x = 0; x < inventoryStorage.width; x++)
    {
      for (var y = 0; y < inventoryStorage.height; y++)
      {
        InventorySlot slot = Instantiate(slotPrefab, inventoryPopup.SlotsContainer.transform);
        slot.Constructor(new Vector2Int(x, y));
        slots.Add(slot);
      }
    }
  }

  private void OnEnable()
  {
    keyAction.Enable();
    toggleInventoryAction.Enable();
    toggleInventoryAction.performed += OnToggleInventoryPerformed;
    inventoryStorage.AddItemEvent += OnAddItem;
  }

  private void OnDisable()
  {
    toggleInventoryAction.performed -= OnToggleInventoryPerformed;
    inventoryStorage.AddItemEvent -= OnAddItem;
    toggleInventoryAction.Disable();
  }

  private void Start()
  {

  }

  private void OnToggleInventoryPerformed(InputAction.CallbackContext context)
  {
    bool isWindowActive = !inventoryPopup.gameObject.activeSelf;
    inventoryPopup.gameObject.SetActive(isWindowActive);

    if (isWindowActive)
    {
      OpenInventory();
    }
    else
    {
      CloseInventory();
    }
  }

  private void OpenInventory()
  {
    fpsController.canRotate = false;
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }

  private void CloseInventory()
  {
    fpsController.canRotate = true;
    input.cursorInputForLook = true;
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  private void OnAddItem(Item item)
  {
    print($"instantiate {item.ImagePrefab.name}");
    print($"slots leng = {slots.Count}");
    InventorySlot slot = slots.Find((slot) => slot.Position == item.Position);
    Instantiate(item.ImagePrefab, slot.transform);
  }
}
