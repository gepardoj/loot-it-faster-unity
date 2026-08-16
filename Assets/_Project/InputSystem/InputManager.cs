using UnityEngine;

public class InputManager : MonoBehaviour
{
  public static InputManager Instance { get; private set; }
  public InputActions InputActions { get; private set; }


  private void Awake()
  {
    Instance = this;
    InputActions = new InputActions();
  }

  private void OnEnable()
  {
    InputActions.Enable();
  }

  private void OnDisable()
  {
    InputActions.Disable();
  }

  private void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }
}
