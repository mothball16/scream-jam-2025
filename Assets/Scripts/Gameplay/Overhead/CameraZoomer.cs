using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoomer : MonoBehaviour
{
    [SerializeField] private float _unzoomFOV;
    [SerializeField] private float _zoomFOV;
    [SerializeField] private float _lerpSpeed;
    private float _targetFOV;
    private InputSystem_Actions _inputActions;
  

    private void Awake()
    {
        _inputActions = new();
    }

    private void OnEnable()
    {
        _targetFOV = _unzoomFOV;
        _inputActions.Player.Enable();
        _inputActions.Player.Interact.started += OnInteract;
        _inputActions.Player.Interact.canceled += OnInteractEnded;
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
        _inputActions.Player.Interact.started -= OnInteract;
        _inputActions.Player.Interact.canceled -= OnInteractEnded;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        _targetFOV = _zoomFOV;
    }

    private void OnInteractEnded(InputAction.CallbackContext ctx)
    {
        _targetFOV = _unzoomFOV;
    }


    // Update is called once per frame
    void Update()
    {
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _targetFOV, Time.deltaTime * _lerpSpeed);
    }
}
