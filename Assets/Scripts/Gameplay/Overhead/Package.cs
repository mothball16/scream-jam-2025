using System;
using UnityEngine;

public record Package(bool Valid, Vector2 WeightPair, string From, string ShipTo, string Date, string Remark, string ID, string Shipper,
    Action<GameObject, bool> OnProcessedCallback = null, Action<GameObject> OnSpawnedCallback = null);