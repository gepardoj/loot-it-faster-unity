using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
  public static PlayerInteractor Instance { get; private set; }
  [field: SerializeField] public float InteractDistance { get; private set; } = 2f;
  [field: SerializeField] public LayerMask InteractLayer { get; private set; }


  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    InputManager.Instance.InputActions.UI.Interact.performed += OnInteract;
  }

  private void OnDisable()
  {
    InputManager.Instance.InputActions.UI.Interact.performed -= OnInteract;
  }

  private void OnInteract(InputAction.CallbackContext context)
  {
    Ray ray = new(Camera.main.transform.position, Camera.main.transform.forward);
    if (Physics.Raycast(ray, out RaycastHit hit, InteractDistance, InteractLayer)
        && hit.collider.TryGetComponent<Chest>(out var chest))
    {
      chest.Open();
    }
  }

  private void OnDrawGizmosSelected()
  {
    if (Camera.main == null) return;
    Gizmos.color = Color.red;
    Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * InteractDistance);
  }
}
