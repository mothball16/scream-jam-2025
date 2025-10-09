using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLooker : MonoBehaviour
{
    // consider priorities rather than overwriting the target w/ every assignment
    public Transform target;
    private List<IDisposable> _disposables;

    [SerializeField] private Transform _defaultTarget;
    [SerializeField] private float _lerpSpeed;
    [SerializeField] private float _sens;

    private void OnEnable()
    {
        _disposables = new()
        {
            EventBus.Subscribe<ChangeCameraLookerTargetEvent>(OnTargetChange)
        };
    }

    private void OnDisable()
    {
        _disposables.ForEach(disposable => disposable.Dispose());
        _disposables.Clear();
        
    }

    private void OnTargetChange(ChangeCameraLookerTargetEvent msg)
    {
        target = msg.Target;
    }

    // Update is called once per frame
    void Update()
    {
        var actualTarget = target != null ? target : _defaultTarget;
        var mouseXDiff = (float)(Mouse.current.position.x.ReadValue() / Screen.width - 0.5) * 2;
        var mouseYDiff = (float)(Mouse.current.position.y.ReadValue() / Screen.height - 0.5) * 2;
        var sens = 60 * _sens / Camera.main.fieldOfView;
        var diff = actualTarget.transform.position - Camera.main.transform.position;
        var rot = Quaternion.LookRotation(diff) * Quaternion.Euler(-mouseYDiff * sens, mouseXDiff * sens, 0);

        Camera.main.transform.rotation = Quaternion.Slerp(
            Camera.main.transform.rotation, rot, 
            Time.deltaTime * _lerpSpeed);
    }
}
