using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryGrid : MonoBehaviour
{
  [SerializeField] private int numberSlots = 104;

  [SerializeField] private RectTransform inventoryPopup;
  [SerializeField] private InventorySlot slotPrefab;
  [SerializeField] private InputActionAsset keyAction;

  private InputAction _toggleInventoryAction;

  private void Awake()
  {
    _toggleInventoryAction = keyAction.FindAction("UI/InventoryToggleKey");
  }

  private void OnEnable()
  {
    keyAction.Enable();
    _toggleInventoryAction.Enable();
    _toggleInventoryAction.performed += OnToggleInventoryPerformed;
  }

  private void OnDisable()
  {
    _toggleInventoryAction.performed -= OnToggleInventoryPerformed;
    _toggleInventoryAction.Disable();
  }

  private void Start()
  {
    for (var x = 0; x < numberSlots; x++)
    {
      Instantiate(slotPrefab, inventoryPopup);
    }
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
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
  }

  private void CloseInventory()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }
}
