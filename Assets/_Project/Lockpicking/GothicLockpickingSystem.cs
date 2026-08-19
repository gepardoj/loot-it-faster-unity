using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Gothic 1-like, simple lockpicking system, with left-to-right A D moves to guess correct combination of a lock
/// </summary>
/// <remarks>
/// Self-efficient class, other parts of codebase do not need to know about it. It's activated by placed in a scene as a GameObject
/// </remarks>
public class GothicLockpickingSystem : MonoBehaviour
{
  private class LockCombination
  {
    public Chest chest;
    public byte size;
    public ushort code;
    public byte currentPos;
  }

  private Chest _currentChest;
  private readonly Dictionary<Chest, LockCombination> _chestsCombinationsMap = new();


  private void Start()
  {
    var chests = ChestGenerator.Instance.GetComponentsInChildren<Chest>();
    foreach (var chest in chests)
    {
      chest.Locked = true;
      _chestsCombinationsMap.Add(chest, CreateLockCombination(chest));
    }
    InputManager.Instance.InputActions.Lockpicking.Move.performed += OnLockpicking;
    InputManager.Instance.InputActions.UI.Click.started += OnPressedLockpick;
    InputManager.Instance.InputActions.UI.Click.canceled += OnReleasedLockpick;
  }

  private void OnDisable()
  {
    InputManager.Instance.InputActions.Lockpicking.Move.performed -= OnLockpicking;
    InputManager.Instance.InputActions.UI.Click.started -= OnPressedLockpick;
    InputManager.Instance.InputActions.UI.Click.canceled -= OnReleasedLockpick;
  }

  private LockCombination CreateLockCombination(Chest chest)
  {
    var combination = new LockCombination
    {
      chest = chest,
      size = 4,
      code = 0b0011,
      currentPos = 0,
    };
    return combination;
  }

  private void OnPressedLockpick(InputAction.CallbackContext ctx)
  {
    if (InventoryManager.Instance.Open) return;
    Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
    var found = Physics.Raycast(ray, out RaycastHit hit, PlayerInteractor.Instance.InteractDistance, PlayerInteractor.Instance.InteractLayer);
    if (!found) return;
    var isLock = hit.collider.TryGetComponent<ChestLock>(out var chestLock);
    var isLockpick = hit.collider.TryGetComponent<Lockpick>(out var lockpick);
    if (isLock && chestLock.HasLockpick)
    {
      InputManager.Instance.InputActions.Player.Disable();
      _currentChest = chestLock.Chest;
    }
    else if (isLockpick)
    {
      InputManager.Instance.InputActions.Player.Disable();
      _currentChest = lockpick.ChestLock.Chest;
    }
  }

  private void OnReleasedLockpick(InputAction.CallbackContext ctx)
  {
    if (InventoryManager.Instance.Open) return;
    InputManager.Instance.InputActions.Player.Enable();
    _currentChest = null;
  }

  private void OnLockpicking(InputAction.CallbackContext ctx)
  {
    if (_currentChest == null || _currentChest.Locked == false) return;

    var move = ctx.ReadValue<Vector2>();
    var combination = _chestsCombinationsMap[_currentChest];
    var direction = -1;
    if (move.x == -1) direction = 0;
    if (move.x == 1) direction = 1;

    if (direction == 0 || direction == 1)
    {
      var trueDirection = (combination.code >> combination.currentPos) & 1; // we get a bit 0 or 1 by the current index
      if (direction == trueDirection) // correct turn
      {
        print("correct turn");
        combination.currentPos++;
        if (combination.currentPos == combination.size)
        {
          OnChestUnlocked();
        }
        else
        {
          OnContinueLockpicking();
        }
      }
      else
      {
        combination.currentPos = 0;
        OnIncorrectTurn();
      }
    }
  }

  private void OnChestUnlocked()
  {
    print("unlocked!");
    _currentChest.Locked = false;
  }

  private void OnContinueLockpicking()
  {

  }

  private void OnIncorrectTurn()
  {
    print("incorrect turn, from the beginning");
    if (Random.Range(0, 4) == 0) // 1/4
    {
      print("damn the lockpick has broken");
      _currentChest.BreakLockpick();
    }
  }
}