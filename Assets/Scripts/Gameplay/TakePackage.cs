using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TakePackage : MonoBehaviour
{

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.TryGetComponent<PackageInfo>(out var info))
        {
            Destroy(info);
            EventBus.Publish<DecisionMadeEvent>(new(true));
        }
    }
}
