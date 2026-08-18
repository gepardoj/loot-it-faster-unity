using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class PlayerInteractor : MonoBehaviour
{
  public static PlayerInteractor Instance;
  [SerializeField] private InputActionAsset _keyActions;
  [field: SerializeField] public float InteractDistance { get; private set; } = 2f;
  [field: SerializeField] public LayerMask InteractLayer { get; private set; }
  private Camera _cam;


  private void Awake()
  {
    Instance = this;
    _cam = GetComponent<Camera>();
  }

  private void OnEnable()
  {
    InputManager.Instance.InputActions.UI.Interact.performed += OnInteract;
  }

  private void OnDisable()
  {
    InputManager.Instance.InputActions.UI.Interact.performed -= OnInteract;
  }

  private void OnInteract(InputAction.CallbackContext context)
  {
    Ray ray = new(_cam.transform.position, _cam.transform.forward);
    if (Physics.Raycast(ray, out RaycastHit hit, InteractDistance, InteractLayer)
        && hit.collider.TryGetComponent<Chest>(out var chest))
    {
      chest.Open();
    }
  }

  private void OnDrawGizmosSelected()
  {
    if (_cam == null) return;
    Gizmos.color = Color.red;
    Gizmos.DrawLine(_cam.transform.position, _cam.transform.position + _cam.transform.forward * InteractDistance);
  }
}
