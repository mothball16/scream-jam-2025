using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLooker : MonoBehaviour
{
    // sscuffed target priority system. if no target check target base otherwise check target
    private Transform _targetBase;
    private Transform _target;

    private float _targetFOV;

    private List<IDisposable> _disposables;
    private InputSystem_Actions _inputActions;

    [SerializeField] private Transform[] _targets;
    [SerializeField] private int _targetIndex;
    [SerializeField] private float _unzoomFOV;
    [SerializeField] private float _zoomFOV;
    [SerializeField] private float _lerpSpeed;
    [SerializeField] private float _sens;


    private void Awake()
    {
        _inputActions = new();
    }

    private void OnEnable()
    {
        _disposables = new()
        {
            EventBus.Subscribe<ChangeCameraLookerTargetEvent>(OnTargetChange)
        };
        _targetFOV = _unzoomFOV;
        _inputActions.Player.Enable();
        _inputActions.Player.Interact.started += OnInteract;
        _inputActions.Player.Interact.canceled += OnInteractEnded;
        _inputActions.Player.Move.started += OnMove;
        _targetBase = _targets[_targetIndex];

    }

    private void OnDisable()
    {
        _disposables.ForEach(disposable => disposable.Dispose());
        _disposables.Clear();
        _inputActions.Player.Interact.started -= OnInteract;
        _inputActions.Player.Interact.canceled -= OnInteractEnded;
        _inputActions.Player.Move.started -= OnMove;
    }

    private void OnTargetChange(ChangeCameraLookerTargetEvent msg)
    {
        _target = msg.Target;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        _targetFOV = _zoomFOV;
    }

    private void OnInteractEnded(InputAction.CallbackContext ctx)
    {
        _targetFOV = _unzoomFOV;
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        var value = ctx.ReadValue<Vector2>();
        if(value.x > 0)
        {
            _targetIndex = Math.Min(_targetIndex + 1, _targets.Length - 1);
        } 
        else if(value.x < 0)
        {
            _targetIndex = Math.Max(_targetIndex - 1, 0);
        }
        _targetBase = _targets[_targetIndex];
    }


    // Update is called once per frame
    void Update()
    {
        var actualTarget = _target != null ? _target : _targetBase;
        var mouseXDiff = (float)(Mouse.current.position.x.ReadValue() / Screen.width - 0.5) * 2;
        var mouseYDiff = (float)(Mouse.current.position.y.ReadValue() / Screen.height - 0.5) * 2;
        var sens = 60 * _sens / Camera.main.fieldOfView;
        var diff = actualTarget.transform.position - Camera.main.transform.position;
        var rot = Quaternion.LookRotation(diff) * Quaternion.Euler(-mouseYDiff * sens, mouseXDiff * sens, 0);

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _targetFOV, Time.deltaTime * _lerpSpeed);
        Camera.main.transform.rotation = Quaternion.Slerp(
            Camera.main.transform.rotation, rot, 
            Time.deltaTime * _lerpSpeed);
    }

}
