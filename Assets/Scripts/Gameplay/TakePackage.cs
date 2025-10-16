using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TakePackage : MonoBehaviour
{

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.TryGetComponent<PackageInfo>(out var info) && !info.Processed)
        {
            info.Processed = true;
            EventBus.Publish<DecisionMadeEvent>(new(true));
        }
    }
}
