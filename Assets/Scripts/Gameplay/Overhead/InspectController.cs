using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.VolumeComponent;

public enum InteractionState
{
    InspectingPackage,
    PackageAccepted,
    PackageRejected,
    Idle,
    Disabled
}

/// <summary>
/// Handles the physical manipulation of packages by the player.
/// </summary>
public class InspectController : MonoBehaviour
{
    [SerializeField] private float _lerpSpeed;
    [SerializeField] private Transform _holdingSpot;
    [SerializeField] private Transform _releaseSpot;
    [SerializeField] private float _holdDistMin;
    [SerializeField] private float _holdDistMax;
    [SerializeField] private float _holdDist;

    private List<IDisposable> _disposables;

    private InputSystem_Actions _inputActions;
    private GameObject _package;
    private InteractionState _state;
    private float _stateTime;


    private void Awake()
    {
        _inputActions = new();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Attack.started += OnAction;
        _inputActions.Player.MoveHoldDistance.performed += OnScroll;
        _inputActions.Player.Deny.started += OnDeny;

        ChangeState(InteractionState.Idle);

        _disposables = new()
        {
            EventBus.Subscribe<DecisionMadeEvent>(OnDecisionMade)
        };
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
        _inputActions.Player.Attack.started -= OnAction;
        _inputActions.Player.MoveHoldDistance.performed -= OnScroll;
        _inputActions.Player.Deny.started -= OnDeny;

        ChangeState(InteractionState.Disabled);

        _disposables.ForEach(disposable => disposable.Dispose());
        _disposables.Clear();
    }

    private void ChangeState(InteractionState newState)
    {
        _stateTime = 0;
        _state = newState;
    }

    private void OnDecisionMade(DecisionMadeEvent e)
    {
        if (_state != InteractionState.InspectingPackage) return;
        if (e.Accepted)
        {
            ChangeState(InteractionState.PackageAccepted);
        } else
        {
            ChangeState(InteractionState.PackageRejected);
            // nobody gaf about the package anymore, so we dispose here
           // Destroy(_package, 0); EDIT: centralize this to levelmanager
        }
    }

    private void OnScroll(InputAction.CallbackContext ctx)
    {
        var scroll = ctx.ReadValue<Vector2>().y;
        _holdDist = Math.Clamp(_holdDist + scroll, _holdDistMin, _holdDistMax);
    }

    private void OnDeny(InputAction.CallbackContext ctx)
    {
        if (_state != InteractionState.InspectingPackage) return;
        EventBus.Publish<DecisionMadeEvent>(new(false));
    }

    private void OnAction(InputAction.CallbackContext ctx)
    {
        if (!GameManager.Inst.GetFlag(StoryFlags.EnablePickup)) return;
        if (_state == InteractionState.InspectingPackage)
        {
            DropPackage();
            ChangeState(InteractionState.Idle);
            return;
        } else if (_state == InteractionState.Idle)
        {
            //int layerMask = LayerMask.GetMask("Interactible");

            float scaleX = (float)Screen.width / Camera.main.targetTexture.width;
            float scaleY = (float)Screen.height / Camera.main.targetTexture.height;


            var ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x / scaleX, Input.mousePosition.y / scaleY, Input.mousePosition.z));
            if (Physics.Raycast(ray, out var info, 100f))
            {
                Debug.DrawRay(ray.origin, ray.direction * info.distance, Color.green);

                if (info.collider.gameObject.TryGetComponent<PackageInfo>(out var pkg) && !pkg.Processed)
                {
                    PickupPackage(info.collider.gameObject);
                } else if(info.collider.gameObject.TryGetComponent<Telephone>(out var telly))
                {
                    EventBus.Publish<TelephonePickupEvent>(new());
                }
            }
        }
    }

    /// <summary>
    /// releases the package from player control
    /// </summary>
    private void DropPackage()
    {
        if (_package == null) return;
        EventBus.Publish(new PackageDroppedEvent(_package));
        EventBus.Publish(new ChangeCameraLookerTargetEvent(null));

        var rigidbody = _package.GetComponent<Rigidbody>();

        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.useGravity = true;

        _package.GetComponent<FaceTowardsCamera>().enabled = false;
        _package = null;


    }

    /// <summary>
    /// puts the package in control of the player
    /// </summary>
    /// <param name="package"></param>
    public void PickupPackage(GameObject package)
    {

        _package = package;
        var rigidbody = _package.GetComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        rigidbody.useGravity = false;
        _package.GetComponent<FaceTowardsCamera>().enabled = true;
        
        ChangeState(InteractionState.InspectingPackage);

        EventBus.Publish(new PackagePickedUpEvent(_package));
    }

    private void Update()
    {
        var rigidbody = _package ? _package.GetComponent<Rigidbody>() : null;
        _stateTime += Time.deltaTime;
        switch (_state)
        {
            case InteractionState.PackageRejected:
                // instantly release and switch back to idle
                DropPackage();
                ChangeState(InteractionState.Idle);
                rigidbody.AddForce(Camera.main.transform.forward.normalized * 5000, ForceMode.Force);
                break;
            case InteractionState.PackageAccepted:
                //this was intended to be a state with lifetime but changes were made
                //no point in cleaning up architecture 12hrs before the submission deadline tho lol
                DropPackage();
                ChangeState(InteractionState.Idle);
                break;
        }
    }

    void FixedUpdate()
    {
        var rigidbody = _package ? _package.GetComponent<Rigidbody>() : null;
        if(rigidbody == null)
        {
            ChangeState(InteractionState.Idle);
        }
        switch (_state)
        {
            case InteractionState.InspectingPackage:
                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.MovePosition(Vector3.Lerp(_package.transform.position, Camera.main.transform.position + Camera.main.transform.forward.normalized * _holdDist, 0.05f * _lerpSpeed));
                break;
                /*
            case InteractionState.PackageAccepted:
                rigidbody.MovePosition(Vector3.Lerp(_package.transform.position, _releaseSpot.position, 0.1f * _lerpSpeed));
                if (_stateTime > 1)
                {
                    Destroy(_package, 5);
                    DropPackage();
                    ChangeState(InteractionState.Idle);
                }
                break;*/
        }
    }
}
