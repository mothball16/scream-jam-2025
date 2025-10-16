using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PackageInfo : MonoBehaviour
{
    public bool Processed = false;
    public Dictionary<Type, IPackageAttribute> data = new();
    public Dictionary<Type, GameObject> attachPoints = new();
}
