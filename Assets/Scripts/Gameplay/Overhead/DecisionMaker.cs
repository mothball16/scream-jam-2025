using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// TO BE SCRAPPED - Handles the decision making on whether to pass or deny a package.
/// </summary>
public class DecisionMaker : MonoBehaviour
{
    private InputSystem_Actions _inputActions;


    private void Awake()
    {
        _inputActions = new();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
     //   _inputActions.Player.Accept.started += OnAccept;
        _inputActions.Player.Deny.started += OnDeny;
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
       // _inputActions.Player.Accept.started -= OnAccept;
        _inputActions.Player.Deny.started -= OnDeny;
    }

    private void OnAccept(InputAction.CallbackContext ctx)
    {
        //something with the accept button should flash here
        EventBus.Publish<DecisionMadeEvent>(new(true));
    }

    private void OnDeny(InputAction.CallbackContext ctx)
    {
        // something with the deny button should flash here
        EventBus.Publish<DecisionMadeEvent>(new(false));
    }
}
