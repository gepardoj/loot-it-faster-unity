using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
  [SerializeField] private Image _cursorBase;
  [SerializeField] private Image _cursorTake;
  [SerializeField] private Image _cursorGrab;
  [SerializeField] private Image _cursorLock;


  private void Awake()
  {
    Base();
  }

  private void Update()
  {
    if (InventoryManager.Instance.Open)
    {
      DisableAll();
      return;
    }
    Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
    var found = Physics.Raycast(ray, out RaycastHit hit, PlayerInteractor.Instance.InteractDistance, PlayerInteractor.Instance.InteractLayer);
    if (!found)
    {
      Base();
      return;
    }
    var hold = InputManager.Instance.InputActions.UI.Click.ReadValue<float>();
    var isLock = hit.collider.TryGetComponent<ChestLock>(out var chestLock);
    var isLockpick = hit.collider.TryGetComponent<Lockpick>(out var lockpick);
    if ((isLock && chestLock.HasLockpick) || isLockpick)
    {
      Grab(hold);
      return;
    }
    var isChest = hit.collider.TryGetComponent<Chest>(out var chest);
    if (isChest && chest.Locked)
    {
      Lock(hold);
      return;
    }
    Take();
  }

  private void DisableAll()
  {
    _cursorBase.gameObject.SetActive(false);
    _cursorTake.gameObject.SetActive(false);
    _cursorGrab.gameObject.SetActive(false);
    _cursorLock.gameObject.SetActive(false);
  }

  private void Base()
  {
    DisableAll();
    _cursorBase.gameObject.SetActive(true);

  }

  private void Take()
  {
    DisableAll();
    _cursorTake.gameObject.SetActive(true);

  }

  private void Grab(float hold)
  {
    DisableAll();
    _cursorGrab.gameObject.SetActive(true);
    _cursorGrab.color = hold > 0 ? Color.white : Color.gray;
  }

  private void Lock(float hold)
  {
    DisableAll();
    _cursorLock.gameObject.SetActive(true);
  }
}
