using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class PlayerInteractor : MonoBehaviour
{
  private Camera _cam;

  public const float InteractDistance = 2f;

  [SerializeField] private InputActionAsset _keyActions;
  [SerializeField] private LayerMask _interactLayer;



  private void Awake()
  {
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
    if (Physics.Raycast(ray, out RaycastHit hit, InteractDistance, _interactLayer))
    {
      if (hit.collider.TryGetComponent<Chest>(out var chest))
      {
        chest.Open();
      }
    }
  }

  private void OnDrawGizmosSelected()
  {
    if (_cam == null) return;
    Gizmos.color = Color.red;
    Gizmos.DrawLine(_cam.transform.position, _cam.transform.position + _cam.transform.forward * InteractDistance);
  }
}
