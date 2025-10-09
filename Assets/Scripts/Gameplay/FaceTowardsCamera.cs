using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.ReloadAttribute;

public class FaceTowardsCamera : MonoBehaviour
{
    [SerializeField] private float _lerpSpeed;
    [SerializeField] private Quaternion _offset;
    void Update()
    {
        transform.rotation =
            Quaternion.Slerp(transform.rotation,
            (Quaternion.LookRotation(Camera.main.transform.position - transform.position, Vector3.up) * _offset).normalized, 
            Time.deltaTime * _lerpSpeed);
    }
}
