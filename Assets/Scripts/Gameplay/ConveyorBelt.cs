using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private Transform _root;
    [SerializeField] private float _speed;
    [SerializeField] private float _lerpSpeed;
    private Vector3 _dir;


    private void OnCollisionStay(Collision col)
    {
        var body = col.rigidbody;
        if (col.rigidbody == null) 
            return;
        col.rigidbody.linearVelocity = Vector3.Lerp(col.rigidbody.linearVelocity, _dir * _speed, Time.deltaTime * _lerpSpeed);
    }


    private void Update()
    {
        _dir = transform.TransformDirection(_root.forward);
    }
}
