using System;
using UnityEngine;

public record Package(bool Valid, Vector2 WeightPair, string From, string ShipTo, string Date, string Remark, string Shipper, string ID,
    Action<GameObject, bool> OnProcessedCallback = null, 
    Action<GameObject> OnSpawnedCallback = null,
    Action<GameObject, bool> OnPickedUpCallback = null, 
    Action<GameObject, bool> OnTelephonePickedUpCallback = null,
    bool PickedUp = false, bool NumberCalled = false);